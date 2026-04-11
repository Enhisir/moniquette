export enum ThreatType {
  Unknown = 0,
  Note = 1,
  Critical = 2,
}

export type Threat = {
  id: string;
  timestamp: string;
  type: ThreatType;
  details: string;
};