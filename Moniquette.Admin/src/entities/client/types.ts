import type { Hardware } from "@/entities/hardware/types"
import type { Problem } from "@/entities/problems/types"
import type { Student } from "@/entities/student/types"

export type Client = {
    sessionId: string
    ipAddress: string
    student: Student
    hardware: Hardware
    problems: Problem[]
}