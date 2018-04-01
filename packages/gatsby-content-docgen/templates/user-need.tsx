import * as React from "react";
import Helmet from 'react-helmet'
import Root from './root'

export default class UserNeed extends Root {
    content() {
        return (
            <div>
                User need
            </div>
        );
    }
}

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