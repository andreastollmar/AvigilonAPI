export type MediaRequestContract = {
    camera: string,
    isImg: boolean,
    requestBody: MediaRequestBodyContract[]
}

export type MediaRequestBodyContract = {
    date: string,
    time: string
}