
export default function({files, markdownNode, markdownAST, pathPrefix, getNode, reporter}) {
    console.log('test');
    return;
}

export async function mutateSource({ markdownNode, files, getNode, reporter }, pluginOptions) {
    console.log("mutating...");
    getNode("sdf");
}