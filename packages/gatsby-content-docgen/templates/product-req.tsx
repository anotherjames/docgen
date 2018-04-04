import * as React from "react";
import Helmet from 'react-helmet'
import Root from './root'

export default class ProductReq extends Root {
    content() {
        let productReq = this.props.data.productReq;
        let siteTitle = this.props.data.site.siteMetadata.title;
        return (
            <div>
                <Helmet title={`${productReq.title} | ${siteTitle}`} />
                <section className="content-header">
                    <h1>{productReq.title}</h1>
                </section>
                <section className="content">
                    <h2>Description</h2>
                    <div dangerouslySetInnerHTML={{ __html: productReq.descriptionHtml }} />
                    <h2>Validation</h2>
                    <div dangerouslySetInnerHTML={{ __html: productReq.validationHtml }} />
                </section>
            </div>
        );
    }
}

export const pageQuery = graphql`
    query ProductReqQuery($slug: String!, $productReqId: String!) {
        ...pageTree
        site {
            siteMetadata {
                title
            }
        }
        productReq(id: {eq: $productReqId}) {
            id
            title
            descriptionHtml
            validationHtml
        }
    }
`;