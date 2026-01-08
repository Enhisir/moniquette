type ToggleInputProps = {
  label: string;
  value: boolean;
  onChange: (v: boolean) => void;
  input?: React.ReactNode;
  suffix?: string;
};

export const ToggleInput = ({ label, value, onChange, input, suffix }: ToggleInputProps) => (
  <div className="flex items-center gap-3">
    <input
      type="checkbox"
      className="w-5 h-5 border border-black rounded"
      checked={value}
      onChange={(e) => onChange(e.target.checked)}
    />
    <span className="text-base text-black">{label}</span>
    {input && input}
    {suffix && <span className="text-base text-black">{suffix}</span>}
  </div>
);
