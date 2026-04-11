import { useState } from "react";
import type { ClientSession } from "@/entities/client/types";
import {
  defaultClient,
  clientWithCriticals,
  clientWithNotes,
  realClient,
} from "@/entities/client/mock";
import { ClientList } from "./components/ClientList/ClientList";
import { ClientDetailsPanel } from "./components/ClientDetailsPanel/ClientDetailsPanel";

export const ClientsPage = () => {
  const [clients] = useState<ClientSession[]>([
    realClient,
    // defaultClient,
    // clientWithNotes,
    // clientWithCriticals,
  ]);

  const [selectedClient, setSelectedClient] = useState<ClientSession | null>(null);

  return (
    <div className="flex h-full min-h-0 gap-6 p-6">
      <ClientList
        clients={clients}
        selectedSessionId={selectedClient?.session.id}
        onSelect={setSelectedClient}
      />
      <ClientDetailsPanel client={selectedClient} />
    </div>
  );
};