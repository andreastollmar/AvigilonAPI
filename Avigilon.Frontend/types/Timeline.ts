export type Timelines = {
    timelines: CameraTimeline[]
}

export type CameraTimeline = {
    cameraId: string,
    record: Record[],
    motion: Record[]
}

export type Record = {
    start: string,
    end: string
}