import type { ClientSession } from "@/entities/client/types";
import { ClientListItem } from "./ClientListItem";

type Props = {
  clients: ClientSession[];
  selectedSessionId?: string;
  onSelect: (client: ClientSession) => void;
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
        flex flex-1 flex-col
        min-h-0
        rounded-lg
        bg-gray-100
        p-2
      "
    >
      <div
        className="
          flex flex-1 flex-col
          min-h-0
          divide-y divide-gray-300
          overflow-y-auto
          scrollbar-hide
        "
      >
        {clients.map((client) => (
          <ClientListItem
            key={client.session.id}
            client={client}
            isSelected={client.session.id === selectedSessionId}
            onClick={() => onSelect(client)}
          />
        ))}
      </div>
    </div>
  );
};