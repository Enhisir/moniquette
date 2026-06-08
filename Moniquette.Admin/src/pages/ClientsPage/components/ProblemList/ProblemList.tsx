import { ThreatType, type Threat } from "@/entities/problems/types";
import { ProblemItem } from "./ProblemItem";

type Props = {
  problems: Threat[];
};

export const ProblemList = ({ problems }: Props) => {
  return (
    <div className="mt-6 space-y-2">
      {problems.sort(compareProblems).map((problem, index) => (
        <ProblemItem key={`${problem.details}-${index}`} problem={problem} />
      ))}
    </div>
  );
};

const compareProblems = (a: Threat, b: Threat) =>
  a.type === ThreatType.Threat
    ? -1
    : b.type === ThreatType.Threat
      ? 1
      : a.details.localeCompare(b.details);
