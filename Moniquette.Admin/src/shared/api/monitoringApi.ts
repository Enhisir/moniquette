import type { ClientSession } from "@/entities/client/types";
import type { Report } from "@/entities/report/types";
import type { Threat } from "@/entities/problems/types";

const API_BASE_URL =
  import.meta.env.VITE_MONIQUETTE_API_URL ?? "http://localhost:8080";

export const monitoringApi = {
  async getSessions(signal?: AbortSignal): Promise<ClientSession[]> {
    const response = await fetch(`${API_BASE_URL}/api/v2/sessions`, { signal });
    if (!response.ok) {
      throw new Error(`Backend вернул ${response.status}`);
    }

    const states = (await response.json()) as SessionStateDto[];
    return states.map(mapSessionState);
  },
};

export type ReportAnalyzedEvent = {
  sessionId: string;
  reportId: string;
  threats: Threat[];
};

export type SessionStateDto = {
  session: ClientSession["session"];
  lastReport?: Report | null;
  threats: Threat[];
};

export const mapSessionState = (state: SessionStateDto): ClientSession => ({
  session: state.session,
  lastReport: state.lastReport ?? undefined,
  threats: dedupeThreats(state.threats),
  ipAddress: getPrimaryIpAddress(state.lastReport),
  processes: state.lastReport?.processes ?? [],
});

export const mergeReport = (
  client: ClientSession,
  report: Report
): ClientSession => ({
  ...client,
  lastReport: report,
  ipAddress: getPrimaryIpAddress(report),
  processes: report.processes,
  session: {
    ...client.session,
    hardwareInfo: report.hardwareInfo ?? client.session.hardwareInfo,
  },
});

export const replaceThreats = (
  client: ClientSession,
  threats: Threat[]
): ClientSession => ({
  ...client,
  threats: dedupeThreats(threats),
});

const getPrimaryIpAddress = (report?: Report | null): string =>
  report?.connections.find((connection) => connection.ipAddressString)
    ?.ipAddressString ?? "IP-адрес не получен";

const dedupeThreats = (threats: Threat[]): Threat[] => {
  const byId = new Map<string, Threat>();
  for (const threat of threats) {
    byId.set(threat.id, threat);
  }

  return [...byId.values()];
};
