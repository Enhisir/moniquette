export type ProcessInfo = {
  pid: number;
  name: string;
  title?: string | null;
  executablePath: string;
  signature?: number[] | null;
};