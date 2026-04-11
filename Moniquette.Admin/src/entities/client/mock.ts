import { msiKatana17HardwareMock, virtualBoxHardwareMock } from "@/entities/hardware/mock";
import {
  criticalWrongApplication,
  criticalWrongConnection,
  criticalWrongDockerContainer,
  noteWrongDevices,
  noteWrongRam,
} from "@/entities/problems/mock";
import type { ClientSession } from "./types";
import { mockProcesses } from "../process/mock";

export const realClient: ClientSession = {
  session: {
    id: "353e9144-085a-4e04-8620-a03c967a36c7",
    firstName: "Матвей",
    middleName: "Игоревич",
    lastName: "Сергеев",
    hardwareInfo: msiKatana17HardwareMock,
  },
  ipAddress: "192.168.31.214",
  threats: [criticalWrongConnection],
  processes: mockProcesses,
};  

export const defaultClient: ClientSession = {
  session: {
    id: "6fb2031c-9765-407c-9396-dba65e645c1d",
    firstName: "Иван",
    middleName: "Иванович",
    lastName: "Иванов",
    hardwareInfo: virtualBoxHardwareMock,
  },
  ipAddress: "0.0.0.0",
  threats: [],
};

export const clientWithNotes: ClientSession = {
  session: {
    id: "6fb2031c-9765-407c-9396-dba65e645c1e",
    firstName: "Пётр",
    middleName: "Сергеевич",
    lastName: "Петров",
    hardwareInfo: virtualBoxHardwareMock,
  },
  ipAddress: "0.0.0.0",
  threats: [noteWrongRam, noteWrongDevices],
};

export const clientWithCriticals: ClientSession = {
  session: {
    id: "6fb2031c-9765-407c-9396-dba65e645c1f",
    firstName: "Никита",
    middleName: "Олегович",
    lastName: "Кузнецов",
    hardwareInfo: virtualBoxHardwareMock,
  },
  ipAddress: "0.0.0.0",
  threats: [
    criticalWrongConnection,
    criticalWrongApplication,
    criticalWrongDockerContainer,
    noteWrongRam,
    noteWrongDevices,
  ],
};