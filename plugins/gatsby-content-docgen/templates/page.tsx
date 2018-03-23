import * as React from "react";
import Helmet from 'react-helmet'
import SideMenu from '../components/SideMenu'
import Master from './master'

export default (props) => {
  const post = props.data.markdownRemark;
  const siteTitle = props.data.site.siteMetadata.title;
  return (
    <div>
      <Helmet title={`${post.frontmatter.title} | ${siteTitle}`} />
      <Master
        content={
          <div>
            <h1>{post.frontmatter.title}</h1>
            <div dangerouslySetInnerHTML={{ __html: post.html }} />
          </div>
        }
        sidebar={
          <SideMenu {...props} />
        }
      />
    </div>
  );
};

export const pageQuery = graphql`
  query PageBySlug($slug: String!) {
    ...pageTree
    site {
      siteMetadata {
        title
      }
    }
    markdownRemark(fields: { slug: { eq: $slug } }) {
      id
      html
      frontmatter {
        title
      }
    }
  }
`;