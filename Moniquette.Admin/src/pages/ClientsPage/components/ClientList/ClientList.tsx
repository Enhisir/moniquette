import type { Client } from "@/entities/client/types";
import { ClientListItem } from "./ClientListItem";

type Props = {
  clients: Client[];
  selectedSessionId?: string;
  onSelect: (client: Client) => void;
};

export const ClientList = ({
  clients,
  selectedSessionId,
  onSelect,
}: Props) => {
  return (
    <div
      className="
        w-full
        max-w-[400px]
        flex flex-col
        flex-1
        min-h-0
        bg-gray-100
        rounded-lg
        p-2
      "
    >
      <div
        className="
          flex flex-col
          flex-1
          min-h-0
          overflow-y-auto
          divide-y divide-gray-300
          scrollbar-hide
        "
      >
        {clients.map(client => (
          <ClientListItem
            key={client.sessionId}
            client={client}
            isSelected={client.sessionId === selectedSessionId}
            onClick={() => onSelect(client)}
          />
        ))}
      </div>
    </div>
  );
};
