import { useState } from "react";

type ListInputProps = {
  label: string;
  list: string[];
  setList: (v: string[]) => void;
  enabled: boolean;
  onToggle: (v: boolean) => void;
};

export const ListInput = ({ label, list, setList, enabled, onToggle }: ListInputProps) => {
  const [value, setValue] = useState("");

  const addItem = () => {
    if (value.trim() !== "") {
      setList([...list, value.trim()]);
      setValue("");
    }
  };

  const removeItem = (index: number) => {
    setList(list.filter((_, i) => i !== index));
  };

  return (
    <div className="flex flex-col gap-2">
      {/* Toggle всегда активен */}
      <div className="flex items-center gap-3">
        <input
          type="checkbox"
          className="w-5 h-5 border border-black rounded"
          checked={enabled}
          onChange={(e) => onToggle(e.target.checked)}
        />
        <span className="text-base text-black">{label}</span>
      </div>

      {/* Список и поле ввода только если включено */}
      <div className={`${!enabled ? "opacity-50" : ""} max-w-[400px] flex gap-2`}>
        <input
          type="text"
          className="border border-black rounded px-2 py-1 flex-1"
          value={value}
          onChange={(e) => setValue(e.target.value)}
          disabled={!enabled} // реально блокируем ввод
        />
        <button
          className="bg-gray-300 px-3 rounded hover:bg-gray-400 disabled:opacity-50"
          onClick={addItem}
          disabled={!enabled} // кнопка тоже блокируется
        >
          Добавить
        </button>
      </div>

      <ul className="max-w-[400px] flex flex-col gap-1">
        {list.map((item, idx) => (
          <li
            key={idx}
            className={`${!enabled ? "opacity-50" : ""} flex justify-between items-center border border-gray-300 rounded px-2 py-1`}
          >
            <span>{item}</span>
            <button
              className="text-xl font-bold text-red-400"
              onClick={() => removeItem(idx)}
              disabled={!enabled} // удаление тоже блокируем
            >
              ×
            </button>
          </li>
        ))}
      </ul>
    </div>
  );
};
