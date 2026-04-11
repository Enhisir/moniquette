import type { RunningDockerImage } from "@/entities/docker/types";
import { useMemo, useState } from "react";

type DockerImagesInputProps = {
  list: RunningDockerImage[];
  setList: (v: RunningDockerImage[]) => void;
  enabled: boolean;
};

type DockerFormState = {
  name: string;
  imageName: string;
  imageDigest: string;
};

const emptyForm: DockerFormState = {
  name: "",
  imageName: "",
  imageDigest: "",
};

export const DockerImagesInput = ({
  list,
  setList,
  enabled,
}: DockerImagesInputProps) => {
  const [form, setForm] = useState<DockerFormState>(emptyForm);

  const isDuplicate = useMemo(() => {
    const digest = form.imageDigest.trim().toLowerCase();
    if (!digest) return false;

    return list.some(
      (item) => item.imageDigest.trim().toLowerCase() === digest
    );
  }, [form.imageDigest, list]);

  const updateField = (field: keyof DockerFormState, value: string) => {
    setForm((prev) => ({
      ...prev,
      [field]: value,
    }));
  };

  const addItem = () => {
    const normalizedItem: RunningDockerImage = {
      name: form.name.trim(),
      imageName: form.imageName.trim(),
      imageDigest: form.imageDigest.trim().toLowerCase(),
    };

    if (
      !normalizedItem.name ||
      !normalizedItem.imageName ||
      !normalizedItem.imageDigest
    ) {
      return;
    }

    const alreadyExists = list.some(
      (item) =>
        item.imageDigest.trim().toLowerCase() === normalizedItem.imageDigest
    );

    if (alreadyExists) {
      return;
    }

    setList([...list, normalizedItem]);
    setForm(emptyForm);
  };

  const removeItem = (index: number) => {
    setList(list.filter((_, i) => i !== index));
  };

  return (
    <div className="flex flex-col gap-4">
      <div>
        <div className="text-base font-medium text-black">
          Подозрительные Docker-образы
        </div>
        <div className="mt-1 text-sm text-gray-500">
          Для каждого образа указываются отображаемое имя, имя образа и digest,
          по которому выполняется точная идентификация.
        </div>
      </div>

      <div
        className={`rounded-lg border border-gray-200 bg-gray-50 p-4 ${
          !enabled ? "opacity-50" : ""
        }`}
      >
        <div className="grid max-w-[820px] grid-cols-1 gap-3 md:grid-cols-3">
          <div className="flex flex-col gap-1">
            <label className="text-sm text-gray-700">Название</label>
            <input
              type="text"
              value={form.name}
              onChange={(e) => updateField("name", e.target.value)}
              placeholder="Например: Qwen Runtime"
              disabled={!enabled}
              className="rounded border border-black px-3 py-2"
            />
          </div>

          <div className="flex flex-col gap-1">
            <label className="text-sm text-gray-700">Image name</label>
            <input
              type="text"
              value={form.imageName}
              onChange={(e) => updateField("imageName", e.target.value)}
              placeholder="Например: qwen-local-runtime"
              disabled={!enabled}
              className="rounded border border-black px-3 py-2"
            />
          </div>

          <div className="flex flex-col gap-1">
            <label className="text-sm text-gray-700">Image digest</label>
            <input
              type="text"
              value={form.imageDigest}
              onChange={(e) => updateField("imageDigest", e.target.value)}
              placeholder="sha256:..."
              disabled={!enabled}
              className="rounded border border-black px-3 py-2"
            />
          </div>
        </div>

        <div className="mt-3 flex items-center gap-3">
          <button
            className="rounded bg-blue-600 px-4 py-2 text-white transition hover:bg-blue-700 disabled:cursor-not-allowed disabled:opacity-50"
            onClick={addItem}
            disabled={
              !enabled ||
              !form.name.trim() ||
              !form.imageName.trim() ||
              !form.imageDigest.trim() ||
              isDuplicate
            }
          >
            Добавить образ
          </button>

          {isDuplicate && (
            <span className="text-sm text-red-500">
              Образ с таким digest уже существует
            </span>
          )}
        </div>
      </div>

      <div className={`${!enabled ? "opacity-50" : ""} overflow-x-auto`}>
        <div className="min-w-[820px] rounded-lg border border-gray-200">
          <div className="grid grid-cols-[180px_220px_1fr_56px] border-b border-gray-200 bg-gray-100 px-3 py-2 text-sm font-medium text-gray-700">
            <div>Название</div>
            <div>Image name</div>
            <div>Image digest</div>
            <div></div>
          </div>

          {list.length === 0 ? (
            <div className="px-3 py-4 text-sm text-gray-500">
              Список подозрительных Docker-образов пуст
            </div>
          ) : (
            <ul className="flex flex-col">
              {list.map((item, idx) => (
                <li
                  key={`${item.imageDigest}-${idx}`}
                  className="grid grid-cols-[180px_220px_1fr_56px] items-center border-b border-gray-200 px-3 py-3 last:border-b-0"
                >
                  <div className="pr-3 text-sm text-black">{item.name}</div>
                  <div className="pr-3 text-sm text-black">{item.imageName}</div>
                  <div className="pr-3 font-mono text-xs text-gray-700 break-all">
                    {item.imageDigest}
                  </div>
                  <div className="flex justify-end">
                    <button
                      className="text-lg font-bold text-red-400 transition hover:text-red-600 disabled:opacity-50"
                      onClick={() => removeItem(idx)}
                      disabled={!enabled}
                      aria-label={`Удалить ${item.name}`}
                    >
                      ×
                    </button>
                  </div>
                </li>
              ))}
            </ul>
          )}
        </div>
      </div>
    </div>
  );
};