import * as React from "react";
import SideMenu from './components/SideMenu'
import Master from './master'
import MenuItem from 'gatsby-plugin-page-tree/menu-item'

export default (props: any) => {
    let menu:MenuItem[] | null;
    if (props.data.currentPage) {
        let currentPageNode = props.data.currentPage.edges.find(x => true);
        if (currentPageNode) {
            menu = currentPageNode.node.menu;
        }
    }
    return (
        <div>
            <Master
                content={
                    <div>
                    product req
                    </div>
                }
                sidebar={
                    <SideMenu {...props} />
                }
                menu={menu}
            />
        </div>
    );
};

export const sd = graphql`
    query UserNeedBySlug($slug: String!) {
        ...pageTree
        site {
            siteMetadata {
                title
            }
        }
    }
`;