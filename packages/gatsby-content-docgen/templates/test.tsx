import * as React from "react";
import SideMenu from '../components/SideMenu'
import Master from './master'

export default (props: any) => {
  return (
    <div>
      <Master
        content={
          <div>
            product req
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
  query TestQuery($slug: String!) {
    ...pageTree
  }
`;