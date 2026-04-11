import { useEffect, useState } from "react";
import { DockerImagesInput } from "./components/DockerImagesInput";
import { ListInput } from "./components/ListInput";
import { SettingsTabs } from "./components/SettingsTabs";
import { ToggleInput } from "./components/ToggleInput";
import type { RunningDockerImage } from "@/entities/docker/types";
import type { SettingsTab } from "@/entities/settings/types";

const DOCKER_STORAGE_KEY = "moniquette_suspicious_docker_images";

export const SettingsPage = () => {
  const [activeTab, setActiveTab] = useState<SettingsTab>("system");

  const [suspiciousRam, setSuspiciousRam] = useState(true);
  const [ramThreshold, setRamThreshold] = useState(4);

  const [lowResolutionEnabled, setLowResolutionEnabled] = useState(true);
  const [resolutionThreshold, setResolutionThreshold] = useState(720);

  const [equipmentEnabled, setEquipmentEnabled] = useState(false);
  const [registryEnabled, setRegistryEnabled] = useState(false);
  const [equipmentKeywords, setEquipmentKeywords] = useState<string[]>([
    "virtualbox",
    "vmware",
    "qemu",
    "parallels",
  ]);

  const [windowEnabled, setWindowEnabled] = useState(false);
  const [windowKeywords, setWindowKeywords] = useState<string[]>([
    "cursor",
    "gpt",
    "qwen",
  ]);

  const [checkDocker, setCheckDocker] = useState(true);
  const [dockerImages, setDockerImages] = useState<RunningDockerImage[]>([
    {
      name: "Qwen Runtime",
      imageName: "qwen-local-runtime",
      imageDigest:
        "sha256:8f3a1d92c47b24f7b13dd844a2db7b9f4c59f96f72e12b08f9b4d10ac1ef4b55",
    },
    {
      name: "Mistral Inference",
      imageName: "mistral-inference",
      imageDigest:
        "sha256:4cb91d11aa93ef0df0a8df7cc9f1a552a7f90e0e5ed92eb8d9fcb1db8a21cc11",
    },
    {
      name: "Llama Worker",
      imageName: "llama-worker",
      imageDigest:
        "sha256:2be4d57f13ac2fbf84f7b6f20d4f6a69f431ca7fd12f0c36a1a6cf06f53c771a",
    },
  ]);

  useEffect(() => {
    const savedDockerImages = localStorage.getItem(DOCKER_STORAGE_KEY);

    if (!savedDockerImages) return;

    try {
      const parsed = JSON.parse(savedDockerImages);
      if (Array.isArray(parsed)) {
        setDockerImages(parsed);
      }
    } catch {
      console.error("Не удалось прочитать список Docker-образов из localStorage");
    }
  }, []);

  useEffect(() => {
    localStorage.setItem(DOCKER_STORAGE_KEY, JSON.stringify(dockerImages));
  }, [dockerImages]);

  return (
    <div className="h-full overflow-y-auto p-6">
      <div className="flex min-h-full flex-col gap-6 pb-8">
        <h1 className="text-2xl font-bold text-black">Настройки</h1>

        <SettingsTabs activeTab={activeTab} onChange={setActiveTab} />

        {activeTab === "system" && (
          <section className="max-w-[900px] rounded-xl border border-gray-300 bg-white p-5 shadow-sm">
            <div className="mb-5">
              <h2 className="text-xl font-semibold text-black">
                Оборудование и система
              </h2>
              <p className="mt-1 text-sm text-gray-500">
                Параметры выявления подозрительных конфигураций устройств и
                активности в системе.
              </p>
            </div>

            <div className="flex flex-col gap-6">
              <ToggleInput
                label="Помечать подозрительными устройства с менее"
                value={suspiciousRam}
                onChange={setSuspiciousRam}
                input={
                  <input
                    type="number"
                    min={1}
                    className="w-16 rounded border border-black px-2 py-1"
                    value={ramThreshold}
                    onChange={(e) => setRamThreshold(Number(e.target.value))}
                  />
                }
                suffix="Гб ОЗУ"
              />

              <ToggleInput
                label="Помечать подозрительными устройства с разрешением менее"
                value={lowResolutionEnabled}
                onChange={setLowResolutionEnabled}
                input={
                  <input
                    type="number"
                    min={144}
                    step={1}
                    className="w-20 rounded border border-black px-2 py-1"
                    value={resolutionThreshold}
                    onChange={(e) => setResolutionThreshold(Number(e.target.value))}
                  />
                }
                suffix="p"
              />

              <ToggleInput
                label="Поиск ключевых слов в реестре Windows"
                value={registryEnabled}
                onChange={setRegistryEnabled}
              />

              <ListInput
                label="Ключевые слова в названии оборудования"
                list={equipmentKeywords}
                setList={setEquipmentKeywords}
                enabled={equipmentEnabled}
                onToggle={setEquipmentEnabled}
              />

              <ListInput
                label="Ключевые слова в заголовках окон"
                list={windowKeywords}
                setList={setWindowKeywords}
                enabled={windowEnabled}
                onToggle={setWindowEnabled}
              />
            </div>
          </section>
        )}

        {activeTab === "docker" && (
          <section className="max-w-[900px] rounded-xl border border-gray-300 bg-white p-5 shadow-sm">
            <div className="mb-5">
              <h2 className="text-xl font-semibold text-black">Docker</h2>
              <p className="mt-1 text-sm text-gray-500">
                Выявление подозрительных Docker-образов выполняется по digest
                образа.
              </p>
            </div>

            <div className="flex flex-col gap-6">
              <ToggleInput
                label="Проверять на наличие подозрительных докер-контейнеров"
                value={checkDocker}
                onChange={setCheckDocker}
              />

              <DockerImagesInput
                list={dockerImages}
                setList={setDockerImages}
                enabled={checkDocker}
              />
            </div>
          </section>
        )}
      </div>
    </div>
  );
};