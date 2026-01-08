import { msiKatana17HardwareMock, virtualBoxHardwareMock } from "@/entities/hardware/mock";
import { criticalWrongApplication, criticalWrongConnection, criticalWrongDockerContainer, noteWrongDevices, noteWrongRam } from "@/entities/problems/mock";
import { realStudent, virtualStudent } from "@/entities/student/mock";
import type { Client } from "./types";

export const realClient: Client = {
    sessionId: "353e9144-085a-4e04-8620-a03c967a36c7",
    ipAddress: "192.168.31.214",
    student: realStudent,
    hardware: msiKatana17HardwareMock,
    problems: [criticalWrongConnection,]
}

export const defaultClient: Client = {
    sessionId: "6fb2031c-9765-407c-9396-dba65e645c1d",
    ipAddress: "0.0.0.0",
    student: virtualStudent,
    hardware: virtualBoxHardwareMock,
    problems: []
}

export const clientWithNotes: Client = {
    sessionId: "6fb2031c-9765-407c-9396-dba65e645c1e",
    ipAddress: "0.0.0.0",
    student: virtualStudent,
    hardware: virtualBoxHardwareMock,
    problems: [
        noteWrongRam,
        noteWrongDevices,
    ]
}

export const clientWithCriticals: Client = {
    sessionId: "6fb2031c-9765-407c-9396-dba65e645c1f",
    ipAddress: "0.0.0.0",
    student: virtualStudent,
    hardware: virtualBoxHardwareMock,
    problems: [
        criticalWrongConnection,
        criticalWrongApplication,
        criticalWrongDockerContainer,
        noteWrongRam,
        noteWrongDevices,
    ]
}