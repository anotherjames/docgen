import * as React from "react";
import PropTypes from 'prop-types'
import Helmet from 'react-helmet'

import Header from '../components/Header'
import './index.css'

export default (props) => {
  return (
    <div>
      <Helmet title={props.data.site.siteMetadata.title} />
      <Header />
      {props.children()}
    </div>
  );
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
