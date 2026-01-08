import { ProblemType } from "@/entities/problems/types";

type Props = {
  problem: {
    type: ProblemType;
    message: string;
  };
};

export const ProblemItem = ({ problem }: Props) => {
  const bgClass =
    problem.type === ProblemType.Critical
      ? "bg-red-300"
      : "bg-yellow-300";

  return (
    <div
      className={`
        ${bgClass}
        rounded-md
        px-5 py-2
        text-xs
        text-black
      `}
    >
      {problem.message}
    </div>
  );
};
