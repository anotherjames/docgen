import * as React from "react";
import PropTypes from 'prop-types'
import Helmet from 'react-helmet'
import Link from 'gatsby-link'
import Header from '../components/Header'
import './assets/styles.less'

interface LayoutProps {
  data: {
    site: {
      siteMetadata: {
        title: string
      }
    }
  };
  children: any;
}

interface LayoutState {
  sidebarOpen: boolean
}

export default class DefaultLayout extends React.Component<LayoutProps, LayoutState> {
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
    let bodyClass = 'skin-blue'
    if(!this.state.sidebarOpen) {
      bodyClass += ' sidebar-collapse';
    } else {
      bodyClass += ' sidebar-open';
    }
    return (
      <div>
        <Helmet title={this.props.data.site.siteMetadata.title}>
          <body className={bodyClass} />
        </Helmet>
        <header className="main-header">
          <Link to="/" className="logo">
            <span className="logo-lg"><b>Doc</b>Gen</span>
          </Link>
          <nav className="navbar navbar-static-top">
            <a href="javasript:void(0);" onClick={this.onBarsClick} className="sidebar-toggle"></a>
          </nav>
        </header>
        <aside className="main-sidebar">
        sidebar
        </aside>
        <div className="content-wrapper">
          {this.props.children()}
        </div>
      </div>
    );
  }
}

export const query = graphql`
  query LayoutQuery {
    site {
      siteMetadata {
        title
      }
    }
  }
`