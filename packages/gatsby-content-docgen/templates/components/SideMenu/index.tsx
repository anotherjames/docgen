import * as React from "react";
import Link from 'gatsby-link'
import SideMenuNode from './SideMenuNode'
import MenuItem from 'gatsby-plugin-page-tree/menu-item'

interface SideMenuProps {
    items: MenuItem[] | null
}

export default (props : SideMenuProps) => {
    return (
        <ul className="tree sidebar-menu">
            {props.items.map(menuItem => 
                <SideMenuNode {...menuItem} key={menuItem.path} />
            )}
        </ul>
    )
};