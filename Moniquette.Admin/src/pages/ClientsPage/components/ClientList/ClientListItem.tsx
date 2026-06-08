import type { ClientSession } from "@/entities/client/types";
import type { Threat } from "@/entities/problems/types";
import { ThreatType } from "@/entities/problems/types";
import { getClientColorClasses } from "./getClientColorClasses";

type Props = {
  client: ClientSession;
  isSelected?: boolean;
  onClick: () => void;
};

export const ClientListItem = ({
  client,
  isSelected,
  onClick,
}: Props) => {
  const { session, threats } = client;

  const criticalCount = threats.filter(
    (p: Threat) => p.type === ThreatType.Threat
  ).length;

  const noteCount = threats.filter(
    (p: Threat) => p.type === ThreatType.Warning
  ).length;

  const totalCount = threats.length;

  const status: ThreatType | null =
    criticalCount > 0
      ? ThreatType.Threat
      : noteCount > 0
        ? ThreatType.Warning
        : null;

  const colorClasses = getClientColorClasses(status);

  const problemBgText =
    status === ThreatType.Threat
      ? "Проблемы:"
      : status === ThreatType.Warning
        ? "Замечания:"
        : "Нет замечаний";

  return (
    <div
      onClick={onClick}
      className={`
        ${colorClasses.bg}
        flex cursor-pointer items-center gap-3 px-4 py-3
        transition-colors
      `}
    >
      <div className="flex w-2 justify-center">
        {isSelected && <span className="h-2 w-2 rounded-full bg-blue-500" />}
      </div>

      <div className="flex flex-1 flex-col">
        <div className="flex items-center justify-between">
          <div className="text-sm font-medium text-gray-900">
            {session.lastName} {session.firstName} {session.middleName}
          </div>

          <div className="flex items-center gap-2 text-xs font-medium">
            <span className={colorClasses.text}>{problemBgText}</span>
            {totalCount > 0 && (
              <span className={`rounded-full px-2 py-0.5 text-gray-500 ${colorClasses.badge}`}>
                {totalCount}
              </span>
            )}
          </div>
        </div>

        <div className="mt-1 flex justify-end text-xs text-gray-600">
          <span>{client.ipAddress}</span>
        </div>
      </div>
    </div>
  );
};
