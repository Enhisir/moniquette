import type { HardwareBriefInfo } from "../hardware/types";

export type Session = {
  id: string;
  firstName: string;
  middleName: string;
  lastName: string;
  hardwareInfo: HardwareBriefInfo;
};