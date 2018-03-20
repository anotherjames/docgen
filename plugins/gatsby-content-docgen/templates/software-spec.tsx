import * as React from "react";
import SideMenu from '../components/SideMenu'

export default (props: any) => {
    return (
        <SideMenu {...props} />
    );
};

export const pageQuery = graphql`
  query SoftwareSpecQuery($slug: String!) {
    ...pageTree
  }
`;