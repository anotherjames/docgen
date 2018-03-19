import treeify from 'treeify-paths'

class TreeNode {
    path: string
    name: string
    children: Array<TreeNode>
}

export class MenuItem {
    path: string
    title: string
    selected: boolean
    active: boolean
    isEmptyParent: true
    children: Array<MenuItem>
}

class GatsbyNodeInternal {
    type: string
}

class GatsbyNodeContext {
    title: string
}

class GatsbyNode {
    path: string
    context: GatsbyNodeContext
    internal: GatsbyNodeInternal
}

const normalizePath = (p: string) => {
    if(p.endsWith('/'))  {
        return p.substr(0, p.length - 1);
    }
    return p;
}

export function buildMenuFromNodes(nodes: Array<GatsbyNode>, selectedPath: string): Array<MenuItem> {
    // let menuItems = nodes
    //     .filter(x => x.internal.type == 'SitePage')
    //     .map(x => {
    //         let menuItem = new MenuItem();
    //         menuItem.path = x.path;
    //         menuItem.title = x.context.title;
    //         menuItem.selected = normalizePath(menuItem.path) == normalizePath(selectedPath);
    //         return menuItem;
    //     });
    // let tree = treeify(menuItems.map(x => normalizePath(x.path))) as TreeNode;
    
    // var rootNode = tree.children[0];

    // for(let child of rootNode.children) {
    //     console.log(child);
    // }

    let r = [
        {
            path: "/requirements",
            title: "Requirements",
            selected: false,
            active: true,
            isEmptyParent: true,
            children: [
                {
                    path: "/requirements/req1",
                    title: "Req 1",
                    selected: true,
                    active: true
                },
                {
                    path: "/requirements/req2",
                    title: "Req 2",
                    selected: false,
                    active: false
                }
            ]
        }
    ];
    return r as Array<MenuItem>;
}