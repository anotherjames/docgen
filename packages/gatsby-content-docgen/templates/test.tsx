import * as React from "react";
import Helmet from 'react-helmet'
import Root from './root'

export default class Test extends Root {
    content() {
        return (
            <div>
                Test
            </div>
        );
    }
}

export const pageQuery = graphql`
    query TestQuery($slug: String!) {
          ...pageTree
    }
`;