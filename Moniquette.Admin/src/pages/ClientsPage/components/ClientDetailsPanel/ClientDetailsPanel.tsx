import type { ClientSession } from "@/entities/client/types";
import { ProblemList } from "@/pages/ClientsPage/components/ProblemList/ProblemList";
import { HardwareSummary } from "./HardwareSummary";
import { DetailsSection } from "./DetailsSection";
import { ActiveProcesses } from "../Processes/ActiveProcesses";

type Props = {
  client: ClientSession | null;
};

export const ClientDetailsPanel = ({ client }: Props) => {
  if (!client) {
    return (
      <div className="flex flex-1 items-center justify-center border-l-2 border-gray-300 text-gray-400">
        Выберите клиента
      </div>
    );
  }

  const { session, threats, ipAddress } = client;

  return (
    <div
      className="
        flex-1
        min-h-0
        overflow-y-auto
        border-l-2 border-gray-300
        px-10 py-6
        scrollbar-hide
      "
    >
      <h2 className="text-2xl font-bold text-black">
        {session.lastName} {session.firstName} {session.middleName}
      </h2>

      <div className="mt-2 text-xs text-gray-500">{ipAddress}</div>

      <div className="mt-6 flex flex-col gap-4">
        <DetailsSection title="Оборудование">
          <HardwareSummary hardware={session.hardwareInfo} />
        </DetailsSection>

        <DetailsSection title="Активные окна" defaultOpen>
          <ActiveProcesses processes={client.processes ?? []} />
        </DetailsSection>

        <DetailsSection title="Угрозы" forceOpen>
          <ProblemList problems={threats} />
        </DetailsSection>
      </div>
    </div>
  );
};