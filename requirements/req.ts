import Test from './test'

export default class Req {
    id: string
    number: string
    title: string
    category: string
    description: string
    validation: string
    tests: Test[]
    parent: Req
    children: Req[]
    constructor() {
        this.tests = [];
        this.children = [];
    }
}