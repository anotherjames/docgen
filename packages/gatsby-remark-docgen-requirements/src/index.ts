import { getGraphqlRunner } from 'gatsby-remark-docgen-requirements/gatsby-node'

export default function({files, markdownNode, markdownAST, pathPrefix, getNode, reporter}) {
    console.log('test');
    return;
}

export async function mutateSource({ markdownNode, files, getNode, reporter }, pluginOptions: any) {
    let o = getGraphqlRunner();
    
}