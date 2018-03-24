
interface GatsbyNodeInternal {
    type: string
}

interface GatsbyNodeContext {
    title: string
}

interface GatsbyNode {
    path: string
    context: GatsbyNodeContext
    internal: GatsbyNodeInternal
}

type GetNodes = () => GatsbyNode[];

interface PluginOptions {
    ignorePaths: string[]
}