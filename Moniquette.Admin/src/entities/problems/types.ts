export enum ProblemType {
    Note = 0, Critical = 1
}

export type Problem = {
    type: ProblemType
    message: string
}