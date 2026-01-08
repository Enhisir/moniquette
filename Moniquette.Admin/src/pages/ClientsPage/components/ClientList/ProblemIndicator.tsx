type Props = {
  hasCritical: boolean;
  hasNotes: boolean;
};

export const ProblemIndicator = ({
  hasCritical,
  hasNotes,
}: Props) => {
  if (hasCritical) {
    return (
      <span className="text-xs font-medium text-red-600">
        Проблема
      </span>
    );
  }

  if (hasNotes) {
    return (
      <span className="text-xs font-medium text-yellow-600">
        Замечание
      </span>
    );
  }

  return (
    <span className="text-xs text-gray-400">
      Нет замечаний
    </span>
  );
};
