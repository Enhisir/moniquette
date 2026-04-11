import { ThreatType, type Threat } from "./types";

export const noteWrongRam: Threat = {
    type: ThreatType.Note,
    details: "Объем оперативной памяти устройства <= 4 гб. Возможно, клиент запущен на виртуальной машине.",
    id: "",
    timestamp: ""
}

export const noteWrongDevices: Threat = {
    type: ThreatType.Note,
    details: "Подозрительные устройства в конфигурации оборудования. Возможно, клиент запущен на виртуальной машине.",
    id: "",
    timestamp: ""
}

export const criticalWrongConnection: Threat = {
    type: ThreatType.Critical,
    details: "Обнаружено стороннее сетевое подключение.",
    id: "",
    timestamp: ""
}

export const criticalWrongApplication: Threat = {
    type: ThreatType.Critical,
    details: "Обнаружено подозрительное приложение.",
    id: "",
    timestamp: ""
}

export const criticalWrongDockerContainer: Threat = {
    type: ThreatType.Critical,
    details: "Обнаружен подозрительный Docker-контейнер.",
    id: "",
    timestamp: ""
}
