import * as path from 'path';
import Test from '../../requirements/test'
import { UserNeed, ProductReq, SoftwareSpec } from '../../requirements/req-structs'
import * as slash from 'slash'

class PluginOptions {
    baseUrl: string
}

type GraphqlRunner = (query:string, context?: any) => Promise<any>;

export const createLayouts = ({ graphql, boundActionCreators }) => {
    const { createLayout } = boundActionCreators;
    const defaultLayout = slash(path.resolve(path.join(__dirname, 'layouts/index.js')));
    createLayout({
        component: defaultLayout,
        id: 'index',
    })
};

export const createPages = async ({ graphql, boundActionCreators, reporter }: {graphql: GraphqlRunner, boundActionCreators: any, reporter: any}, pluginOptions: PluginOptions) => {
    const { createPage } = boundActionCreators

    // Create our static pages
    createPage({
        path: '/',
        component: slash(path.join(__dirname, 'pages/index.js'))
    });
    createPage({
        path: '/404',
        component: slash(path.join(__dirname, 'pages/404.js'))
    });

    if(!pluginOptions.baseUrl) {
        pluginOptions.baseUrl = '/';
    }

    if(!pluginOptions.baseUrl.startsWith('/')) {
        pluginOptions.baseUrl = '/' + pluginOptions.baseUrl;
    }

    const userNeedTemplate = slash(path.join(__dirname, 'templates/user-need.js'));
    const productReqTemplate = slash(path.join(__dirname, 'templates/product-req.js'));
    const softwareSpecTemplate = slash(path.join(__dirname, 'templates/software-spec.js'));
    const testTemplate = slash(path.join(__dirname, 'templates/test.js'));

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
            path: path.join(pluginOptions.baseUrl, userNeed.path),
            component: userNeedTemplate
        });
        for (let productReq of userNeed.productReqs) {
            createPage({
                path: path.join(pluginOptions.baseUrl, productReq.path),
                component: productReqTemplate
            });
            for (let softwareSpec of productReq.softwareSpecs) {
                createPage({
                    path: path.join(pluginOptions.baseUrl, softwareSpec.path),
                    component: softwareSpecTemplate
                });
            }
        }
    }

    result = await graphql(
        `
        {
            allTest {
                edges {
                    node {
                        id
                        path
                    }
                }
            }	
        }
        `
    );

    for (let test of (result.data.allTest.edges as Array<any>).map(x => x.node as Test)) {
        createPage({
            path: path.join(pluginOptions.baseUrl, test.path),
            component: testTemplate
        });
    }
};