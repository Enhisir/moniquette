import type { ProcessInfo } from "@/entities/process/types";

type Props = {
  processes: ProcessInfo[];
};

export const ActiveProcesses = ({ processes }: Props) => {
  const active = processes.filter(
    (p) => p.title && p.title.trim() !== ""
  );

  if (active.length === 0) {
    return (
      <div className="text-sm text-gray-500">
        Активные окна не обнаружены
      </div>
    );
  }

  return (
    <div className="flex flex-col gap-2">
      {active.map((process) => (
        <div
          key={process.pid}
          className="rounded-md border border-gray-200 bg-gray-50 px-3 py-2"
        >
          <div className="text-sm font-medium text-black">
            {process.title}
          </div>

          <div className="mt-1 flex justify-between text-xs text-gray-500">
            <span>{process.name}</span>
            <span>PID: {process.pid}</span>
          </div>
        </div>
      ))}
    </div>
  );
};