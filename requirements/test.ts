export default class Test {
    number: string
    responseType: TestResponseType
    validationType: TestValidationType
    type: TestType
    action: string
    expected: string
}

export enum TestResponseType {
    Ok = "ok",
    PassFail = "passFail"
}

export enum TestValidationType {
    Verification = "verification",
    Validation = "validation"
}

export enum TestType {
    Software = "software",
    Hardware = "hardware"
}