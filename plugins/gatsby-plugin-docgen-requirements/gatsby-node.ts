import * as path from 'path';
import { loadReqDir } from '../../requirements'
import Req from '../../requirements/req'
import Test from '../../requirements/test'
import * as util from 'util'
import * as fs from 'fs'
import * as crypto from 'crypto'

const fileExists = util.promisify(fs.exists)

class PluginOptions {
    path: string
}

class ReqProcelain {
    id: string
    number: string
    title: string
    category: string
    description: string
    validation: string
    tests: Test[]
    constructor(req: Req) {
        this.id = req.id;
        this.number = req.number;
        this.title = req.title;
        this.category = req.category;
        this.description = req.description;
        this.validation = req.validation;
        this.tests = req.tests;
    }
}

class UserNeed extends ReqProcelain {
    constructor(req: Req) {
        super(req);
        this.productReqs = [];
        for(let child of req.children) {
            this.productReqs.push(new ProductReq(child));
        }
    }
    productReqs: ProductReq[];
}

class ProductReq extends ReqProcelain {
    constructor(req: Req) {
        super(req);
        this.softwareSpecs = [];
        for(let child of req.children) {
            this.softwareSpecs.push(new SoftwareSpec(child));
        }
    }
    softwareSpecs: SoftwareSpec[];
}

class SoftwareSpec extends ReqProcelain {
    constructor(req: Req) {
        super(req);
    }
}

export const sourceNodes = async ({ boundActionCreators, reporter }: any, pluginOptions: PluginOptions) => {
    const { createNode } = boundActionCreators

    if (!(pluginOptions && pluginOptions.path)) {
      reporter.panic('Path is required.');
    }
  
    if (!await fileExists(pluginOptions.path)) {
      reporter.panic('Path does not exist.');
    }
  
    for(let req of (await loadReqDir(pluginOptions.path))) {
        var userNeed = new UserNeed(req);
        createNode({
            ...userNeed,
            parent: null,
            children: [],
            internal: {
                type: 'userNeed',
                contentDigest: crypto.createHash('md5').update(userNeed.id).digest('hex')
            }
        });
        for(let productReq of userNeed.productReqs) {
            createNode({
                ...productReq,
                parent: userNeed.id,
                children: [],
                internal : {
                    type: 'productReq',
                    contentDigest: crypto.createHash('md5').update(productReq.id).digest('hex')
                }
            });
            for(let softwareSpec of productReq.softwareSpecs) {
                createNode({
                    ...softwareSpec,
                    parent: userNeed.id,
                    children: [],
                    internal : {
                        type: 'softwareSpec',
                        contentDigest: crypto.createHash('md5').update(softwareSpec.id).digest('hex')
                    }
                });
            }
        }
    }
};