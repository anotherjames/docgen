import * as React from "react";
import Link from 'gatsby-link'
import SideMenuNode from './SideMenuNode'

export default (props) => {
    let currentPage = props.data.currentPage;
    if (!currentPage) {
        return null;
    }
    let currentPageNode = currentPage.edges.find(x => true);
    if (!currentPageNode) {
        return null;
    }
    return (
        <ul>
            {currentPageNode.node.menu.map(menuItem => 
                <SideMenuNode {...menuItem} key={menuItem.path} />
            )}
        </ul>
    )
};