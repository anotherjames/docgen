import * as path from 'path';
import { loadReqDir } from '../../requirements'
import Req from '../../requirements/req'
import Test from '../../requirements/test'
import { UserNeed, ProductReq, SoftwareSpec } from '../../requirements/req-structs'
import * as util from 'util'
import * as fs from 'fs'
import * as crypto from 'crypto'
import * as slash from 'slash'

const fileExists = util.promisify(fs.exists)

class PluginOptions {
    path: string
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
                contentDigest: crypto.createHash('md5').update(userNeed.path).digest('hex')
            }
        });
        for(let test of userNeed.tests) {
            createNode({
                ...test,
                parent: userNeed.id,
                children: [],
                internal: {
                    type: 'test',
                    contentDigest: crypto.createHash('md5').update(test.path).digest('hex')
                }
            });
        }
        for(let productReq of userNeed.productReqs) {
            createNode({
                ...productReq,
                parent: userNeed.id,
                children: [],
                internal : {
                    type: 'productReq',
                    contentDigest: crypto.createHash('md5').update(productReq.path).digest('hex')
                }
            });
            for(let test of productReq.tests) {
                createNode({
                    ...test,
                    parent: productReq.id,
                    children: [],
                    internal: {
                        type: 'test',
                        contentDigest: crypto.createHash('md5').update(test.path).digest('hex')
                    }
                });
            }
            for(let softwareSpec of productReq.softwareSpecs) {
                createNode({
                    ...softwareSpec,
                    parent: productReq.id,
                    children: [],
                    internal : {
                        type: 'softwareSpec',
                        contentDigest: crypto.createHash('md5').update(softwareSpec.path).digest('hex')
                    }
                });
                for(let test of softwareSpec.tests) {
                    createNode({
                        ...test,
                        parent: softwareSpec.id,
                        children: [],
                        internal: {
                            type: 'test',
                            contentDigest: crypto.createHash('md5').update(test.path).digest('hex')
                        }
                    });
                }
            }
        }
    }
};