import * as path from 'path';
import Test from 'docgen-requirements/test'
import { UserNeed, ProductReq, SoftwareSpec } from 'docgen-requirements/req-structs'
import * as slash from 'slash'
import { createFilePath } from 'gatsby-source-filesystem'
import { GraphQLInt } from 'graphql'

export interface PluginOptions {
    baseUrl: string
}

type GraphqlRunner = (query:string, context?: any) => Promise<any>;

export const createLayouts = ({ graphql, boundActionCreators }) => {
    const { createLayout } = boundActionCreators;
    const defaultLayout = slash(path.resolve(path.join(__dirname, 'layouts/index.tsx')));
    createLayout({
        component: defaultLayout,
        id: 'index',
    })
};

const createRequirementPages = async(boundActionCreators: any, graphql: GraphqlRunner, pluginOptions: PluginOptions) => {
    const { createPage } = boundActionCreators;

    if(!pluginOptions.baseUrl) {
        pluginOptions.baseUrl = '/';
    }

    if(!pluginOptions.baseUrl.startsWith('/')) {
        pluginOptions.baseUrl = '/' + pluginOptions.baseUrl;
    }

    const userNeedTemplate = slash(path.join(__dirname, 'templates/user-need.tsx'));
    const productReqTemplate = slash(path.join(__dirname, 'templates/product-req.tsx'));
    const softwareSpecTemplate = slash(path.join(__dirname, 'templates/software-spec.tsx'));
    const testTemplate = slash(path.join(__dirname, 'templates/test.tsx'));

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
    if(!result.errors) {
        (result.data.allUserNeed.edges as Array<any>).map(x => x.node as UserNeed).forEach((userNeed, userNeedIndex) => {
            createPage({
                path: path.join(pluginOptions.baseUrl, userNeed.path),
                component: userNeedTemplate,
                context: {
                    slug: path.join(pluginOptions.baseUrl, userNeed.path),
                    title: userNeed.title,
                    order: userNeedIndex
                }
            });
            userNeed.productReqs.forEach((productReq, productReqIndex) => {
                createPage({
                    path: path.join(pluginOptions.baseUrl, productReq.path),
                    component: productReqTemplate,
                    context: {
                        slug: path.join(pluginOptions.baseUrl, productReq.path),
                        title: userNeed.title,
                        order: productReqIndex
                    }
                });
                productReq.softwareSpecs.forEach((softwareSpec, softwareSpecIndex) => {
                    createPage({
                        path: path.join(pluginOptions.baseUrl, softwareSpec.path),
                        component: softwareSpecTemplate,
                        context: {
                            slug: path.join(pluginOptions.baseUrl, softwareSpec.path),
                            title: userNeed.title,
                            order: softwareSpecIndex
                        }
                    });
                });
            });
        });
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

    if (!result.errors) {
        (result.data.allTest.edges as Array<any>).map(x => x.node as Test).forEach((test, testIndex) => {
            createPage({
                path: path.join(pluginOptions.baseUrl, test.path),
                component: testTemplate,
                context: {
                    slug: path.join(pluginOptions.baseUrl, test.path),
                    title: test.number,
                    order: testIndex
                }
            });
        });
    }
};

const createMarkdownPages = async(boundActionCreators: any, graphql: GraphqlRunner) => {
    const { createPage } = boundActionCreators;

    let result = await graphql(
        `
        {
            allMarkdownRemark {
                edges {
                    node {
                        fields {
                            slug
                        }
                        frontmatter {
                            title
                        }
                        order
                    }
                }
            }
        }
        `
        );
    const pageTemplate = slash(path.join(__dirname, 'templates/page.tsx'));
    for (let page of (result.data.allMarkdownRemark.edges as Array<any>).map(x => x.node)) {
        createPage({
            path: page.fields.slug,
            component: pageTemplate,
            context: {
                slug: page.fields.slug,
                title: page.frontmatter.title,
                order: page.order
            },
        });
    }
};

export const createPages = async ({ graphql, boundActionCreators, reporter }: {graphql: GraphqlRunner, boundActionCreators: any, reporter: any}, pluginOptions: PluginOptions) => {
    const { createPage } = boundActionCreators;

    // Create our static pages
    createPage({
        path: '/',
        component: slash(path.join(__dirname, 'pages/index.tsx'))
    });
    createPage({
        path: '/404',
        component: slash(path.join(__dirname, 'pages/404.tsx'))
    });

    // Create pages from our requirements.
    await createRequirementPages(boundActionCreators, graphql, pluginOptions);

    // Create pages from the markdown files.
    await createMarkdownPages(boundActionCreators, graphql);
};

export const onCreateNode = ({ node, boundActionCreators, getNode }) => {
    const { createNodeField } = boundActionCreators
  
    if (node.internal.type === 'MarkdownRemark') {
        const value = createFilePath({ node, getNode });
        createNodeField({
            name: 'slug',
            node,
            value,
        });
    }

    if (node.internal.type == 'SitePage') {
        if(node.context && node.context.order) {
            createNodeField({
                name: 'order',
                node,
                value: node.context.order
            });
        }
    }
}

export const setFieldsOnGraphQLNodeType = async({ type }) => {
    if (type.name === 'MarkdownRemark') {
        return {
            order: {
                type: GraphQLInt,
                resolve: node => {
                    if(node.frontmatter.order) {
                        return node.frontmatter.order;
                    }
                    return 0;
                }
            }
        };
    }

    return {};
};