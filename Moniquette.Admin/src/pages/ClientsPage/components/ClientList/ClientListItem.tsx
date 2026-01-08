import type { Client } from "@/entities/client/types";
import type { Problem } from "@/entities/problems/types";
import { ProblemType } from "@/entities/problems/types";
import { getClientColorClasses } from "./getClientColorClasses";

type Props = {
  client: Client;
  isSelected?: boolean;
  onClick: () => void;
}

export const ClientListItem = ({
  client,
  isSelected,
  onClick,
}: Props) => {
  const { student, problems } = client;

  const criticalCount = problems.filter(
    (p: Problem) => p.type === ProblemType.Critical
  ).length;

  const noteCount = problems.filter(
    (p: Problem) => p.type === ProblemType.Note
  ).length;

  const totalCount = problems.length;

  const status: ProblemType | null = criticalCount > 0
    ? ProblemType.Critical
    : noteCount > 0
      ? ProblemType.Note
      : null

  const colorClasses = getClientColorClasses(status)

  const problemBgText = status === ProblemType.Critical
    ? "Проблемы:"
    : status === ProblemType.Note
      ? "Замечания:"
      : "Нет замечаний"

  return (
    <div
      onClick={onClick}
      className={`
        ${colorClasses.bg}
        px-4 py-3
        cursor-pointer
        transition-colors
        flex
        items-center
        gap-3
      `}
    >
      <div className="w-2 flex justify-center">
        {isSelected && <span className="w-2 h-2 rounded-full bg-blue-500"></span>}
      </div>

      <div className="flex-1 flex flex-col">

        <div className="flex items-center justify-between">
          <div className="text-sm font-medium text-gray-900">
            {student.surname} {student.name} {student.middlename}
          </div>

          <div className="flex items-center gap-2 text-xs font-medium">
            <span className={colorClasses.text}>{problemBgText}</span>
            {totalCount > 0 && (
              <span className={`px-2 py-0.5 rounded-full ${colorClasses.badge} text-gray-500`}>
                {totalCount}
              </span>
            )}
          </div>
        </div>

        <div className="mt-1 flex justify-between text-xs text-gray-600">
          <span>{student.number}</span>
          <span>{client.ipAddress}</span>
        </div>
      </div>
    </div>
  )
}
