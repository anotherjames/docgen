import * as React from "react";
import Link from 'gatsby-link'
import MenuItem from '../../../gatsby-plugin-page-tree/menu-item'

export default class SideMenuNode extends React.Component<MenuItem, {}> {
    public render() {
        return (
            <li>
                <Link to={this.props.path}>
                    {this.props.title}
                </Link>
                {this.props.children && this.props.children.length > 0 && 
                    <ul>
                        {this.props.children.map(x =>
                            <SideMenuNode {...x} key={x.path} />
                        )}
                    </ul>
                }
            </li>
        )
    }
}