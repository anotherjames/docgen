import * as path from 'path';
import { loadReqDir } from '../../requirements'
import Req from '../../requirements/req'
import Test from '../../requirements/test'
import * as util from 'util'
import * as fs from 'fs'
import * as crypto from 'crypto'
import * as slash from 'slash'

const fileExists = util.promisify(fs.exists)

class PluginOptions {
    path: string
}

class ReqProcelain {
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

type GraphqlRunner = (query:string, context?: any) => Promise<any>;

export const createPages = async ({ graphql, boundActionCreators }: {graphql: GraphqlRunner, boundActionCreators: any}) => {
    const { createPage } = boundActionCreators
    const blogPostTemplate = slash(path.resolve("src/templates/user-need.js"));
    let result = await graphql(
        `
        {
            allUserNeed {
                edges {
                    node {
                        id
                        title
                        number
                        path
                        productReqs {
                            id
                            title
                            number
                            path
                            softwareSpecs {
                                id
                                title
                                number
                                path
                            }
                        }
                    }
                }
            }	
        }
        `
    );
    for (let userNeed of (result.data.allUserNeed.edges as Array<any>).map(x => x.node as UserNeed)) {
        createPage({
            path: "/" + userNeed.path,
            component: blogPostTemplate
        });
        for (let productReq of userNeed.productReqs) {
            createPage({
                path: "/" + productReq.path,
                component: blogPostTemplate
            });
            for (let softwareSpec of productReq.softwareSpecs) {
                createPage({
                    path: "/" + softwareSpec.path,
                    component: blogPostTemplate
                });
            }
        }
    }
};