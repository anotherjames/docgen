import * as React from "react";
import Helmet from 'react-helmet'
import Root from './root'

export default class SoftwareSpec extends Root {
    content() {
        let softwareSpec = this.props.data.softwareSpec;
        let siteTitle = this.props.data.site.siteMetadata.title;
        return (
            <div>
                <Helmet title={`${softwareSpec.title} | ${siteTitle}`} />
                <section className="content-header">
                    <h1>{softwareSpec.title}</h1>
                </section>
                <section className="content">
                    <h2>Description</h2>
                    <div dangerouslySetInnerHTML={{ __html: softwareSpec.descriptionHtml }} />
                    <h2>Validation</h2>
                    <div dangerouslySetInnerHTML={{ __html: softwareSpec.validationHtml }} />
                </section>
            </div>
        );
    }
}

export const pageQuery = graphql`
    query SoftwareSpecQuery($slug: String!, $softwareSpecId: String!) {
        ...pageTree
        site {
            siteMetadata {
                title
            }
        }
        softwareSpec(id: {eq: $softwareSpecId}) {
            id
            title
            descriptionHtml
            validationHtml
        }
    }
`;