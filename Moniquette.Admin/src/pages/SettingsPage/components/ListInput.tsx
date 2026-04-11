import { useState } from "react";

type ListInputProps = {
  label: string;
  list: string[];
  setList: (v: string[]) => void;
  enabled: boolean;
  onToggle: (v: boolean) => void;
};

export const ListInput = ({
  label,
  list,
  setList,
  enabled,
  onToggle,
}: ListInputProps) => {
  const [value, setValue] = useState("");

  const addItem = () => {
    const trimmed = value.trim().toLowerCase();

    if (!trimmed) return;
    if (list.includes(trimmed)) {
      setValue("");
      return;
    }

    setList([...list, trimmed]);
    setValue("");
  };

  const removeItem = (index: number) => {
    setList(list.filter((_, i) => i !== index));
  };

  return (
    <div className="flex flex-col gap-3">
      <div className="flex items-center gap-3">
        <input
          type="checkbox"
          className="h-5 w-5 rounded border border-black"
          checked={enabled}
          onChange={(e) => onToggle(e.target.checked)}
        />
        <span className="text-base text-black">{label}</span>
      </div>

      <div className={`${!enabled ? "opacity-50" : ""} flex max-w-[520px] gap-2`}>
        <input
          type="text"
          className="flex-1 rounded border border-black px-3 py-2"
          value={value}
          onChange={(e) => setValue(e.target.value)}
          disabled={!enabled}
          onKeyDown={(e) => {
            if (e.key === "Enter") {
              e.preventDefault();
              addItem();
            }
          }}
        />
        <button
          type="button"
          className="rounded bg-gray-200 px-4 py-2 transition hover:bg-gray-300 disabled:opacity-50"
          onClick={addItem}
          disabled={!enabled}
        >
          Добавить
        </button>
      </div>

      <ul className="flex max-w-[520px] flex-col gap-2">
        {list.map((item, idx) => (
          <li
            key={`${item}-${idx}`}
            className={`${!enabled ? "opacity-50" : ""} flex items-center justify-between rounded border border-gray-300 bg-gray-50 px-3 py-2`}
          >
            <span>{item}</span>
            <button
              type="button"
              className="text-xl font-bold text-red-400 hover:text-red-600 disabled:opacity-50"
              onClick={() => removeItem(idx)}
              disabled={!enabled}
            >
              ×
            </button>
          </li>
        ))}
      </ul>
    </div>
  );
};