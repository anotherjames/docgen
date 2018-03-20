import * as React from "react";
import SideMenu from '../components/SideMenu'

export default (props: any) => {
    return (
        <SideMenu {...props} />
    );
};

export const sd = graphql`
  query UserNeedBySlug($slug: String!) {
    ...pageTree
    site {
      siteMetadata {
        title
      }
    }
  }
`;