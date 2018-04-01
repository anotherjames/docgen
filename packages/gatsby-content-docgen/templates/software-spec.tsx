import * as React from "react";
import Helmet from 'react-helmet'
import Root from './root'

export default class SoftwareSpec extends Root {
    content() {
        return (
            <div>
                Software spec
            </div>
        );
    }
}

export const pageQuery = graphql`
    query SoftwareSpecQuery($slug: String!) {
        ...pageTree
    }
`;