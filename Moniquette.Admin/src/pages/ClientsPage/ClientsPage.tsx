import { useEffect, useMemo, useRef, useState } from "react";
import type { ClientSession } from "@/entities/client/types";
import { ClientList } from "./components/ClientList/ClientList";
import { ClientDetailsPanel } from "./components/ClientDetailsPanel/ClientDetailsPanel";
import { monitoringApi, mergeReport, replaceThreats } from "@/shared/api/monitoringApi";
import {
  MonitoringRealtimeClient,
  type RealtimeStatus,
} from "@/shared/realtime/monitoringRealtime";

export const ClientsPage = () => {
  const [clients, setClients] = useState<ClientSession[]>([]);
  const [selectedSessionId, setSelectedSessionId] = useState<string>();
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [realtimeStatus, setRealtimeStatus] =
    useState<RealtimeStatus>("disconnected");
  const realtimeRef = useRef<MonitoringRealtimeClient | null>(null);

  const selectedClient = useMemo(
    () =>
      clients.find((client) => client.session.id === selectedSessionId) ?? null,
    [clients, selectedSessionId]
  );

  useEffect(() => {
    const abortController = new AbortController();

    const loadSessions = async () => {
      try {
        setIsLoading(true);
        setError(null);
        const loadedClients = await monitoringApi.getSessions(
          abortController.signal
        );
        setClients(loadedClients);
        setSelectedSessionId((current) => current ?? loadedClients[0]?.session.id);
      } catch (caught) {
        if (!abortController.signal.aborted) {
          setError(caught instanceof Error ? caught.message : "Не удалось загрузить клиентов");
        }
      } finally {
        if (!abortController.signal.aborted) {
          setIsLoading(false);
        }
      }
    };

    void loadSessions();
    return () => abortController.abort();
  }, []);

  useEffect(() => {
    if (clients.length === 0) {
      return;
    }

    const realtime = new MonitoringRealtimeClient({
      onStatusChange: setRealtimeStatus,
      onReportReceived: (report) => {
        setClients((current) =>
          current.map((client) =>
            client.session.id === report.sessionId
              ? mergeReport(client, report)
              : client
          )
        );
      },
      onReportAnalyzed: (event) => {
        setClients((current) =>
          current.map((client) =>
            client.session.id === event.sessionId
              ? replaceThreats(client, event.threats)
              : client
          )
        );
      },
      onThreatsUpdated: (sessionId, threats) => {
        setClients((current) =>
          current.map((client) =>
            client.session.id === sessionId
              ? replaceThreats(client, threats)
              : client
          )
        );
      },
    });

    realtimeRef.current = realtime;
    void realtime.start(clients.map((client) => client.session.id)).catch((caught) => {
      setRealtimeStatus("disconnected");
      setError(caught instanceof Error ? caught.message : "Не удалось подключить realtime");
    });

    return () => {
      realtimeRef.current = null;
      void realtime.stop();
    };
  }, [clients.length]);

  return (
    <div className="flex h-full min-h-0 flex-col gap-3 p-6">
      <div className="flex items-center justify-between text-sm">
        <div className="text-gray-500">
          Realtime: <span className={getRealtimeClassName(realtimeStatus)}>{getRealtimeText(realtimeStatus)}</span>
        </div>
        {error && <div className="text-red-600">{error}</div>}
      </div>

      <div className="flex min-h-0 flex-1 gap-6">
        {isLoading ? (
          <div className="flex flex-1 items-center justify-center text-gray-400">
            Загрузка клиентов...
          </div>
        ) : clients.length === 0 ? (
          <div className="flex flex-1 items-center justify-center text-gray-400">
            Нет активных сессий
          </div>
        ) : (
          <>
            <ClientList
              clients={clients}
              selectedSessionId={selectedSessionId}
              onSelect={(client) => setSelectedSessionId(client.session.id)}
            />
            <ClientDetailsPanel client={selectedClient} />
          </>
        )}
      </div>
    </div>
  );
};

const getRealtimeText = (status: RealtimeStatus) => {
  switch (status) {
    case "connected":
      return "подключено";
    case "connecting":
      return "подключение";
    case "reconnecting":
      return "переподключение";
    case "disconnected":
      return "нет соединения";
  }
};

const getRealtimeClassName = (status: RealtimeStatus) =>
  status === "connected"
    ? "text-green-700"
    : status === "reconnecting" || status === "connecting"
      ? "text-yellow-700"
      : "text-red-700";
