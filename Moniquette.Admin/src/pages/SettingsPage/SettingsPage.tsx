import { useState } from "react";
import { ToggleInput } from "./components/ToggleInput";
import { ListInput } from "./components/ListInput";

export const SettingsPage = () => {
  const [suspiciousRam, setSuspiciousRam] = useState(true);
  const [ramThreshold, setRamThreshold] = useState(4);

  const [checkDocker, setCheckDocker] = useState(true);
  const [equipmentEnabled, setEquipmentEnabled] = useState(false);
  const [equipmentKeywords, setEquipmentKeywords] = useState<string[]>(['virtualbox', 'vmware', 'qemu', 'parallels']);
  const [windowEnabled, setWindowEnabled] = useState(false);
  const [windowKeywords, setWindowKeywords] = useState<string[]>(['cursor', 'gpt', 'qwen']);

  return (
    <div className="flex flex-col gap-6 p-6">
      <h1 className="text-2xl font-bold text-black">Настройки</h1>

      <ToggleInput
        label="Помечать подозрительными устройства с менее"
        value={suspiciousRam}
        onChange={setSuspiciousRam}
        input={
          <input
            type="number"
            className="w-16 border border-black rounded px-1 py-0.5"
            value={ramThreshold}
            onChange={(e) => setRamThreshold(Number(e.target.value))}
          />
        }
        suffix="Гб ОЗУ"
      />

      <ToggleInput
        label="Проверять на наличие подозрительных докер-контейнеров"
        value={checkDocker}
        onChange={setCheckDocker}
      />

      <ListInput
        label="Ключевые слова в названии оборудования"
        list={equipmentKeywords}
        setList={setEquipmentKeywords}
        enabled={equipmentEnabled}
        onToggle={setEquipmentEnabled}
      />

      <ListInput
        label="Ключевые слова заголовках окон"
        list={windowKeywords}
        setList={setWindowKeywords}
        enabled={windowEnabled}
        onToggle={setWindowEnabled}
      />
    </div>
  );
};
