import { GraphQLScalarType } from 'graphql';
import * as util from 'util'
import * as fs from 'fs'
import * as path from 'path'
import MenuItem from './menu-item'
import { buildMenuFromNodes } from './menu-builder'

const copyFile = util.promisify(fs.copyFile);

const buildTreeForPath = async(pagePath: string, getNodes: GetNodes, ignorePaths: string[]) => {
    return buildMenuFromNodes(getNodes(), pagePath, ignorePaths);
};

export const setFieldsOnGraphQLNodeType = async({ type, getNodes }: {type: any, getNodes: GetNodes}, pluginOptions: PluginOptions) => {
    if(!pluginOptions.ignorePaths) {
        pluginOptions.ignorePaths = [
            "/404",
            "/dev-404-page"
        ];
    }
    
    if (type.name !== "SitePage") {
      return {};
    }

    return {
        menu: {
            type: new GraphQLScalarType({
                name: 'Menu',
                serialize(value) {
                    return value;
                }
            }),
            resolve: node => {
                return buildTreeForPath(node.path, getNodes, pluginOptions.ignorePaths);
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
};
