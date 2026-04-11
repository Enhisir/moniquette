type ToggleInputProps = {
  label: string;
  value: boolean;
  onChange: (v: boolean) => void;
  input?: React.ReactNode;
  suffix?: string;
};

export const ToggleInput = ({
  label,
  value,
  onChange,
  input,
  suffix,
}: ToggleInputProps) => (
  <div className="flex flex-wrap items-center gap-3">
    <input
      type="checkbox"
      className="h-5 w-5 rounded border border-black"
      checked={value}
      onChange={(e) => onChange(e.target.checked)}
    />
    <span className="text-base text-black">{label}</span>
    {input && input}
    {suffix && <span className="text-base text-black">{suffix}</span>}
  </div>
);