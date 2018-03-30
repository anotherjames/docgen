import * as React from "react";
import Helmet from 'react-helmet'
import SideMenu from '../components/SideMenu'
import Master from './master'
import MenuItem from 'gatsby-plugin-page-tree/menu-item'

export default (props) => {
    const post = props.data.markdownRemark;
    const siteTitle = props.data.site.siteMetadata.title;
    let menu:MenuItem[] | null;
    if (props.data.currentPage) {
        let currentPageNode = props.data.currentPage.edges.find(x => true);
        if (currentPageNode) {
            menu = currentPageNode.node.menu;
        }
    }
    return (
        <div>
            <Helmet title={`${post.frontmatter.title} | ${siteTitle}`} />
            <Master
                content={
                    <div>
                        <h1>{post.frontmatter.title}</h1>
                        <div dangerouslySetInnerHTML={{ __html: post.html }} />
                    </div>
                }
                sidebar={
                    <SideMenu {...props} />
                }
                menu={menu}
            />
        </div>
    );
};

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