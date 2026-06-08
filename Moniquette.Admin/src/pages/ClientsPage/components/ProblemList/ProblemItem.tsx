import { ThreatType, type Threat } from "@/entities/problems/types";

type Props = {
  problem: Threat;
};

export const ProblemItem = ({ problem }: Props) => {
  const bgClass =
    problem.type === ThreatType.Threat
      ? "bg-red-300"
      : problem.type === ThreatType.Warning
        ? "bg-yellow-300"
        : "bg-gray-200";

  return (
    <div className={`${bgClass} rounded-md px-5 py-2 text-xs text-black`}>
      {problem.details}
    </div>
  );
};
