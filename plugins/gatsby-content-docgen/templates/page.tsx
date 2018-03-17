import * as React from "react";
import Test from '../../../requirements/test'
declare const graphql: (query: TemplateStringsArray) => void;

export default (props) => {
    let t = new Test();
    t.number = "324";
    return (
        <div>page {JSON.stringify(props)} {JSON.stringify(t)}</div>
    );
};

export const pageQuery = graphql`
  query IndexQuery {
    allMarkdownRemark {
        edges {
          node {
            frontmatter {
              title
            }
            fields {
              slug
            }
          }
        }
      }
  }
`;