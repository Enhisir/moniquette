import type { Session } from "@/entities/session/types";
import type { Report } from "@/entities/report/types";
import type { Threat } from "@/entities/problems/types";
import type { ProcessInfo } from "../process/types";

export type ClientSession = {
  session: Session;
  lastReport?: Report;
  threats: Threat[];
  ipAddress: string;
  processes?: ProcessInfo[]
};