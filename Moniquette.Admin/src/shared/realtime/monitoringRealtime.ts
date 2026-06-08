import {
  HubConnection,
  HubConnectionBuilder,
  HubConnectionState,
  LogLevel,
} from "@microsoft/signalr";
import type { Report } from "@/entities/report/types";
import type { Threat } from "@/entities/problems/types";
import type { ReportAnalyzedEvent } from "@/shared/api/monitoringApi";

const HUB_URL =
  import.meta.env.VITE_MONIQUETTE_HUB_URL ??
  "http://localhost:8080/monitoring-hub";

export type RealtimeStatus =
  | "disconnected"
  | "connecting"
  | "connected"
  | "reconnecting";

export type MonitoringRealtimeHandlers = {
  onStatusChange: (status: RealtimeStatus) => void;
  onReportReceived: (report: Report) => void;
  onReportAnalyzed: (event: ReportAnalyzedEvent) => void;
  onThreatsUpdated: (sessionId: string, threats: Threat[]) => void;
};

export class MonitoringRealtimeClient {
  private connection: HubConnection;
  private sessionIds: string[] = [];

  constructor(private handlers: MonitoringRealtimeHandlers) {
    this.connection = new HubConnectionBuilder()
      .withUrl(HUB_URL)
      .withAutomaticReconnect()
      .configureLogging(LogLevel.Warning)
      .build();

    this.connection.onreconnecting(() =>
      this.handlers.onStatusChange("reconnecting")
    );
    this.connection.onreconnected(() => {
      this.handlers.onStatusChange("connected");
      void this.joinSessions(this.sessionIds);
    });
    this.connection.onclose(() =>
      this.handlers.onStatusChange("disconnected")
    );

    this.connection.on("ReportReceived", this.handlers.onReportReceived);
    this.connection.on(
      "ReportAnalyzed",
      this.handlers.onReportAnalyzed
    );
    this.connection.on("ThreatsUpdated", (threats: Threat[]) => {
      const sessionId = threats[0]?.sessionId;
      if (sessionId) {
        this.handlers.onThreatsUpdated(sessionId, threats);
      }
    });
  }

  async start(sessionIds: string[]) {
    this.sessionIds = sessionIds;

    if (this.connection.state === HubConnectionState.Disconnected) {
      this.handlers.onStatusChange("connecting");
      await this.connection.start();
      this.handlers.onStatusChange("connected");
    }

    await this.joinSessions(sessionIds);
  }

  async joinSessions(sessionIds: string[]) {
    this.sessionIds = sessionIds;

    if (this.connection.state !== HubConnectionState.Connected) {
      return;
    }

    await Promise.all(
      sessionIds.map((sessionId) =>
        this.connection.invoke("JoinSessionGroup", sessionId)
      )
    );
  }

  async stop() {
    if (this.connection.state !== HubConnectionState.Disconnected) {
      await this.connection.stop();
    }
  }
}
