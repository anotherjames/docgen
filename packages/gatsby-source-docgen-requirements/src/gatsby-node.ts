import * as path from 'path';
import { loadReqDir } from 'docgen-requirements'
import Req from 'docgen-requirements/req'
import Test from 'docgen-requirements/test'
import { UserNeed, ProductReq, SoftwareSpec } from 'docgen-requirements/req-structs'
import * as util from 'util'
import * as fs from 'fs'
import * as crypto from 'crypto'
import * as slash from 'slash'
import { GraphQLString } from 'graphql'
import * as remark from 'remark'
import * as extendRemarkNode from 'gatsby-transformer-remark/extend-node-type'

const fileExists = util.promisify(fs.exists)

export interface PluginOptions {
    path: string,
    remarkOptions: {
        plugins: any[] | null
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
            ...(JSON.parse(JSON.stringify(userNeed))),
            parent: null,
            children: [],
            internal: {
                type: 'UserNeed',
                contentDigest: crypto.createHash('md5').update(userNeed.path).digest('hex')
            }
        });
        for(let test of userNeed.tests) {
            createNode({
                ...(JSON.parse(JSON.stringify(test))),
                parent: userNeed.id,
                children: [],
                internal: {
                    type: 'Test',
                    contentDigest: crypto.createHash('md5').update(test.path).digest('hex')
                }
            });
        }
        for(let productReq of userNeed.productReqs) {
            createNode({
                ...(JSON.parse(JSON.stringify(productReq))),
                parent: userNeed.id,
                children: [],
                internal : {
                    type: 'ProductReq',
                    contentDigest: crypto.createHash('md5').update(productReq.path).digest('hex')
                }
            });
            for(let test of productReq.tests) {
                createNode({
                    ...(JSON.parse(JSON.stringify(test))),
                    parent: productReq.id,
                    children: [],
                    internal: {
                        type: 'Test',
                        contentDigest: crypto.createHash('md5').update(test.path).digest('hex')
                    }
                });
            }
            for(let softwareSpec of productReq.softwareSpecs) {
                createNode({
                    ...(JSON.parse(JSON.stringify(softwareSpec))),
                    parent: productReq.id,
                    children: [],
                    internal : {
                        type: 'SoftwareSpec',
                        contentDigest: crypto.createHash('md5').update(softwareSpec.path).digest('hex')
                    }
                });
                for(let test of softwareSpec.tests) {
                    createNode({
                        ...(JSON.parse(JSON.stringify(test))),
                        parent: softwareSpec.id,
                        children: [],
                        internal: {
                            type: 'Test',
                            contentDigest: crypto.createHash('md5').update(test.path).digest('hex')
                        }
                    });
                }
            }
        }
    }
};

export const setFieldsOnGraphQLNodeType = async(args, pluginOptions: PluginOptions) => {
    if(!pluginOptions.remarkOptions) {
        pluginOptions.remarkOptions = {
            plugins: []
        };
    }
    if(!pluginOptions.remarkOptions.plugins) {
        pluginOptions.remarkOptions.plugins = [];
    }
    if(args.type.name === 'UserNeed'
        || args.type.name === 'ProductReq'
        || args.type.name === 'SoftwareSpec'
        || args.type.name === 'Test') {
        // Create a new type that looks like a markdown document.
        // Then, get's it's graphql properties.
        let newArgs = {...args}
        let newType = {...args.type};
        newType.name = 'MarkdownRemark';
        newArgs.type = newType;
        let newTypeProperties = await extendRemarkNode(newArgs, pluginOptions.remarkOptions);

        let createMarkdownProperty = (propertyName: string): any => {
            return {
                type: GraphQLString,
                resolve: (node) => {
                    let newNode = {
                        ...node
                    }
                    newNode.internal = {
                        ...node.internal
                    }
                    newNode.internal.content = node[propertyName];
                    newNode.internal.contentDigest = `${node.internal.contentDigest}-${propertyName}`;
                    return newTypeProperties.html.resolve(newNode);
                }
            }
        }

        // if(args.type.name === 'UserNeed'
        // || args.type.name === 'ProductReq'
        // || args.type.name === 'SoftwareSpec'
        // || args.type.name === 'Test')

        if(args.type.name === 'UserNeed'
            || args.type.name === 'ProductReq'
            || args.type.name === 'SoftwareSpec') {
            return {
                descriptionHtml: createMarkdownProperty('description'),
                validationHtml: createMarkdownProperty('validation')
            }
        } else if(args.type.name === 'Test') {
            return {
                actionHtml: createMarkdownProperty('action'),
                expectedHtml: createMarkdownProperty('expected')
            }
        }
    }

    return {}
}