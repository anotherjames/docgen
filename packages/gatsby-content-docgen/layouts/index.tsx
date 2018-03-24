import * as React from "react";
import Helmet from 'react-helmet'
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

export default class DefaultLayout extends React.Component<LayoutProps, {}> {
  render() {
    return (
      <div>
        <Helmet title={this.props.data.site.siteMetadata.title} />
        {this.props.children()}
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