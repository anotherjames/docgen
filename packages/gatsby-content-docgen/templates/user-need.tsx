import * as React from "react";
import Helmet from 'react-helmet'
import Root from './root'

export default class UserNeed extends Root {
    content() {
        let userNeed = this.props.data.userNeed;
        let siteTitle = this.props.data.site.siteMetadata.title;
        return (
            <div>
                <Helmet title={`${userNeed.title} | ${siteTitle}`} />
                <section className="content-header">
                    <h1>{userNeed.title}</h1>
                </section>
                <section className="content">
                    <h2>Description</h2>
                    <div dangerouslySetInnerHTML={{ __html: userNeed.descriptionHtml }} />
                    <h2>Validation</h2>
                    <div dangerouslySetInnerHTML={{ __html: userNeed.validationHtml }} />
                </section>
            </div>
        );
    }
}

export const sd = graphql`
    query UserNeedBySlug($slug: String!, $userNeedId: String!) {
        ...pageTree
        site {
            siteMetadata {
                title
            }
        }
        userNeed(id: {eq: $userNeedId}) {
            id
            title
            descriptionHtml
            validationHtml
        }
    }
`;