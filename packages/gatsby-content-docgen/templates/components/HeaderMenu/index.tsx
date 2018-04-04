import * as React from 'react';
import Link from 'gatsby-link'
import MenuItem from 'gatsby-plugin-page-tree/menu-item'

interface HeaderMenuProps {
    items: MenuItem[] | null
}

export default class HeaderMenu extends React.Component<HeaderMenuProps> {
    constructor(props) {
        super(props);
    }
    render() {
        if(!this.props.items) {
            return null;
        }
        if(this.props.items.length == 0) {
            return null;
        }
        return (
            <ul className="nav navbar-nav collapse navbar-collapse">
                {this.props.items.map(menuItem => 
                    <li key={menuItem.path} className={(menuItem.active || menuItem.selected) ? 'active' : ''}><Link to={menuItem.path}>{menuItem.title}</Link></li>
                )}
            </ul>
        );
    }
}