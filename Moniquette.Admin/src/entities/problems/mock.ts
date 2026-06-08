import { ThreatType, type Threat } from "./types";

const mockSessionId = "353e9144-085a-4e04-8620-a03c967a36c7";
const mockReportId = "906690d1-23da-4ea1-9ea8-4c8fc42e52ad";

export const noteWrongRam: Threat = {
    type: ThreatType.Warning,
    details: "Объем оперативной памяти устройства <= 4 гб. Возможно, клиент запущен на виртуальной машине.",
    id: "",
    timestamp: "",
    sessionId: mockSessionId,
    reportId: mockReportId
}

export const noteWrongDevices: Threat = {
    type: ThreatType.Warning,
    details: "Подозрительные устройства в конфигурации оборудования. Возможно, клиент запущен на виртуальной машине.",
    id: "",
    timestamp: "",
    sessionId: mockSessionId,
    reportId: mockReportId
}

export const criticalWrongConnection: Threat = {
    type: ThreatType.Threat,
    details: "Обнаружено стороннее сетевое подключение.",
    id: "",
    timestamp: "",
    sessionId: mockSessionId,
    reportId: mockReportId
}

export const criticalWrongApplication: Threat = {
    type: ThreatType.Threat,
    details: "Обнаружено подозрительное приложение.",
    id: "",
    timestamp: "",
    sessionId: mockSessionId,
    reportId: mockReportId
}

export const criticalWrongDockerContainer: Threat = {
    type: ThreatType.Threat,
    details: "Обнаружен подозрительный Docker-контейнер.",
    id: "",
    timestamp: "",
    sessionId: mockSessionId,
    reportId: mockReportId
}
