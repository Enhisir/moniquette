import { ProblemType, type Problem } from "./types";

export const noteWrongRam: Problem = {
    type: ProblemType.Note,
    message: "Объем оперативной памяти устройства <= 4 гб. Возможно, клиент запущен на виртуальной машине."
}

export const noteWrongDevices: Problem = {
    type: ProblemType.Note,
    message: "Подозрительные устройства в конфигурации оборудования. Возможно, клиент запущен на виртуальной машине."
}

export const criticalWrongConnection: Problem = {
    type: ProblemType.Critical,
    message: "Обнаружено стороннее сетевое подключение."
}

export const criticalWrongApplication: Problem = {
    type: ProblemType.Critical,
    message: "Обнаружено подозрительное приложение."
}

export const criticalWrongDockerContainer: Problem = {
    type: ProblemType.Critical,
    message: "Обнаружен подозрительный Docker-контейнер."
}
