import * as React from "react"
import PropTypes from 'prop-types'
import Helmet from 'react-helmet'
import Link from 'gatsby-link'
import HeaderMenu from './components/HeaderMenu'
import MenuItem from 'gatsby-plugin-page-tree/menu-item'
import SideMenu from './components/SideMenu'

interface RootState {
    sidebarOpen: boolean
}

export default class Root extends React.Component<any, RootState> {
    constructor(props) {
        super(props);
        this.state = {
            sidebarOpen: false
        }
    }
    onBarsClick = (e) => {
        this.setState({
            sidebarOpen: !this.state.sidebarOpen
        })
    }
    render() {
        let menu = this.getMenu();
        let sideBar = this.sideBar(menu);

        let bodyClass = 'skin-blue';
        if(sideBar) {
            if(!this.state.sidebarOpen) {
                bodyClass += ' sidebar-collapse';
            } else {
                bodyClass += ' sidebar-open';
            }
        } else {
            bodyClass += ' no-sidebar';
        }

        return (
            <div>
                <Helmet>
                    <body className={bodyClass} />
                </Helmet>
                <header className="main-header">
                <Link to="/" className="logo">
                    <span className="logo-lg"><b>Doc</b>Gen</span>
                </Link>
                <nav className="navbar navbar-static-top">
                    <a href="javasript:void(0);" onClick={this.onBarsClick} className="sidebar-toggle"></a>
                    <HeaderMenu items={menu} />
                </nav>
                </header>
                {sideBar &&
                    <aside className="main-sidebar">
                        <section className="sidebar">
                            {sideBar}
                        </section>
                    </aside>
                }
                <div className="content-wrapper">
                    <div className={!sideBar ? 'container' : ''}>
                        {this.content()}
                    </div>
                </div>
            </div>
        );
    }
    getMenu(): MenuItem[] | null {
        let menu:MenuItem[] | null;
        if (this.props.data.currentPage) {
            let currentPageNode = this.props.data.currentPage.edges.find(x => true);
            if (currentPageNode) {
                menu = currentPageNode.node.menu;
            }
        }
        return menu;
    }
    sideBar(menu: MenuItem[] | null) {
        if(!menu || menu.length == 0) {
            return null;
        }

        // We don't render the top level menu items.
        // We only render side menu when a root level item is selected/active.
        for(let menuItem of menu) {
            if(menuItem.active || menuItem.selected) {
                if(menuItem.children.length > 0) {
                    return <SideMenu items={menuItem.children} />;
                }
            }
        }

        return null;
    }
    content() {
        return null;
    }
}