import { ProblemType, type Problem } from "@/entities/problems/types";
import { ProblemItem } from "./ProblemItem";

type Props = {
  problems: Problem[];
};

export const ProblemList = ({ problems }: Props) => {
  return (
    <div className="mt-6 space-y-2">
      {problems
      .sort(compareProblems)
      .map((problem, index) => (
        <ProblemItem key={index} problem={problem} />
      ))}
    </div>
  );
};

const compareProblems = (a: Problem, b: Problem) => 
  a.type == ProblemType.Critical 
  ? -1 
  : b.type == ProblemType.Critical
    ? 1
    : a.message.localeCompare(b.message)