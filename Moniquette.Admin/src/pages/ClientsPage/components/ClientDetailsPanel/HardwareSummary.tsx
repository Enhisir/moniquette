import type { Hardware } from "@/entities/hardware/types";

type Props = {
  hardware: Hardware;
};

export const HardwareSummary = ({ hardware }: Props) => {
  const availableRAM = (hardware.availableRam / 1024 ** 3).toFixed(1)

  return (
    <div className="mt-6 bg-gray-100 rounded-md p-4">
      <div className="text-xs font-medium text-black mb-2">
        Конфигурация оборудования:
      </div>

      <ul className="text-xs text-gray-700 space-y-1">
        <li>ОС: {hardware.operatingSystem}</li>
        <li>ОЗУ: {availableRAM} ГБ</li>
        <li>
          CPU: {hardware.cpu.name} ({hardware.cpu.cores} ядер /{" "}
          {hardware.cpu.numberOfLogicalProcessors} потоков)
        </li>
        <li>
          Материнская плата:{" "}
          {hardware.motherboard.manufacturer}{" "}
          {hardware.motherboard.product}
        </li>
      </ul>
    </div>
  );
};
