import type { Client } from "@/entities/client/types";
import { ProblemList } from "@/pages/ClientsPage/components/ProblemList/ProblemList";
import { HardwareSummary } from "./HardwareSummary";
type Props = {
  client: Client | null;
};

export const ClientDetailsPanel = ({ client }: Props) => {
  if (!client) {
    return (
      <div className="flex-1 border-l-2 border-gray-300 flex items-center justify-center text-gray-400">
        Выберите клиента
      </div>
    );
  }

  const { student, hardware, problems, ipAddress } = client;

  return (
    <div
      className="
        flex-1
        min-h-0
        border-l-2 border-gray-300
        px-10 py-6
        overflow-y-auto
        scrollbar-hide
      "
    >
      <h2 className="text-2xl font-bold text-black">
        {student.surname} {student.name} {student.middlename}
      </h2>
      <div className="mt-2 text-xs text-gray-500">{ipAddress}</div>

      <HardwareSummary hardware={hardware} />
      <ProblemList problems={problems} />
    </div>
  );
};
