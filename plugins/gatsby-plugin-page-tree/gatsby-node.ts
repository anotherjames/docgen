import { GraphQLString, GraphQLScalarType } from 'graphql';
import * as GraphQLJSON from 'graphql-type-json'

import * as util from 'util'
import * as fs from 'fs'
import * as path from 'path'
import { buildMenuFromNodes } from './menu-builder'

const copyFile = util.promisify(fs.copyFile);

const buildTreeForPath = async(pagePath: string, getNodes) => {
    return buildMenuFromNodes(getNodes(), pagePath);
};

export const setFieldsOnGraphQLNodeType = async({ type, getNodes }) => {
    if (type.name !== "SitePage") {
      return {};
    }

    console.log(getNodes());

    return {
      menu: {
        type: new GraphQLScalarType({
            name: 'Menu',
            serialize(value) {
                return value;
            }
        }),
        resolve: node => {
            return buildTreeForPath(node.path, getNodes);
        }
      }
    };
  };

export const onPreExtractQueries = async ({
    store,
    getNodes,
    boundActionCreators,
  }) => {
    // Copy the helper fragment used to query the current page and it's menu items.
    const program = store.getState().program;
    await copyFile(path.join(__dirname, "fragments.js"),
        `${program.directory}/.cache/fragments/page-tree.js`);
        console.log("copied");
};
