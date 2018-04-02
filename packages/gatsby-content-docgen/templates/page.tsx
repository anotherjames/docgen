import * as React from "react";
import Helmet from 'react-helmet'
import Root from './root'

export default class Page extends Root {
    content() {
        let post = this.props.data.markdownRemark;
        let siteTitle = this.props.data.site.siteMetadata.title;
        return (
            <div>
                <Helmet title={`${post.frontmatter.title} | ${siteTitle}`} />
                <section className="content-header">
                    <h1>{post.frontmatter.title}</h1>
                </section>
                <section className="content">
                    <div dangerouslySetInnerHTML={{ __html: post.html }} />
                </section>
            </div>
        );
    }
}

export const pageQuery = graphql`
    query PageBySlug($slug: String!) {
        ...pageTree
        site {
            siteMetadata {
                title
            }
        }
        markdownRemark(fields: { slug: { eq: $slug } }) {
            id
            html
            frontmatter {
                title
            }
        }
    }
`;