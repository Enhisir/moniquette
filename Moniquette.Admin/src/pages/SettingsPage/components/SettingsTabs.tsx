import type { SettingsTab } from "@/entities/settings/types";

type SettingsTabsProps = {
  activeTab: SettingsTab;
  onChange: (tab: SettingsTab) => void;
};

export const SettingsTabs = ({ activeTab, onChange }: SettingsTabsProps) => {
  return (
    <div className="flex items-center gap-2 border-b border-gray-200 pb-3">
      <button
        type="button"
        onClick={() => onChange("system")}
        className={`rounded-lg px-4 py-2 text-sm font-medium transition ${
          activeTab === "system"
            ? "bg-blue-600 text-white"
            : "border border-gray-300 bg-white text-gray-700 hover:bg-gray-50"
        }`}
      >
        Оборудование и система
      </button>

      <button
        type="button"
        onClick={() => onChange("docker")}
        className={`rounded-lg px-4 py-2 text-sm font-medium transition ${
          activeTab === "docker"
            ? "bg-blue-600 text-white"
            : "border border-gray-300 bg-white text-gray-700 hover:bg-gray-50"
        }`}
      >
        Docker
      </button>
    </div>
  );
};