export enum ThreatType {
  Unknown = 0,
  Warning = 1,
  Threat = 2,
}

export type Threat = {
  id: string;
  timestamp: string;
  type: ThreatType;
  sessionId: string;
  reportId: string;
  details: string;
};
