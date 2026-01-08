import { useState } from "react";
import type { Client } from "@/entities/client/types";
import { defaultClient, clientWithCriticals, clientWithNotes, realClient } from "@/entities/client/mock";
import { ClientList } from "./components/ClientList/ClientList";
import { ClientDetailsPanel } from "./components/ClientDetailsPanel/ClientDetailsPanel";

export const ClientsPage = () => {

  const [clients, setClients] = useState<Client[]>([
    realClient,
    defaultClient,
    clientWithNotes,
    clientWithCriticals
  ]);
  const [selectedClient, setSelectedClient] = useState<Client | null>(null);

  return (
    <div className="h-full min-h-0 p-6 flex gap-6">
      <ClientList
        clients={clients}
        selectedSessionId={selectedClient?.sessionId}
        onSelect={setSelectedClient}
      />
      <ClientDetailsPanel client={selectedClient} />
    </div>
  );
};
