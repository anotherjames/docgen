import * as React from "react";
import Helmet from 'react-helmet'
import Root from './root'

export default class ProductReq extends Root {
    content() {
        return (
            <div>
                product req
            </div>
        );
    }
}

export const pageQuery = graphql`
    query ProductReqQuery($slug: String!) {
        ...pageTree
    }
`;