import type { RunningDockerImage } from "@/entities/docker/types";
import type { HardwareBriefInfo } from "@/entities/hardware/types";
import type { NetworkConnection } from "@/entities/network/types";
import type { ProcessInfo } from "@/entities/process/types";

export type Report = {
  sessionId: string;
  timestamp: string;
  processes: ProcessInfo[];
  hardwareInfo: HardwareBriefInfo;
  connections: NetworkConnection[];
  windowsRegistry?: Record<string, string> | null;
  isDockerEnabled: boolean;
  dockerContainers?: RunningDockerImage[] | null;
};