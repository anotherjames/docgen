import Test from './test'
import Req from './req'

export class ReqProcelain {
    id: string
    number: string
    path: string
    title: string
    category: string
    description: string
    validation: string
    tests: Test[]
    constructor(req: Req) {
        this.id = req.id;
        this.number = req.number;
        this.path = req.path;
        this.title = req.title;
        this.category = req.category;
        this.description = req.description;
        this.validation = req.validation;
        this.tests = req.tests || [];
    }
}

export class UserNeed extends ReqProcelain {
    constructor(req: Req) {
        super(req);
        this.productReqs = [];
        for(let child of req.children) {
            this.productReqs.push(new ProductReq(child));
        }
    }
    productReqs: ProductReq[];
}

export class ProductReq extends ReqProcelain {
    constructor(req: Req) {
        super(req);
        this.softwareSpecs = [];
        for(let child of req.children) {
            this.softwareSpecs.push(new SoftwareSpec(child));
        }
    }
    softwareSpecs: SoftwareSpec[];
}

export class SoftwareSpec extends ReqProcelain {
    constructor(req: Req) {
        super(req);
    }
}