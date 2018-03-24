export const pageTreeFragement = graphql`
  fragment pageTree on RootQueryType {
    menu: allSitePage(filter: {path: {eq:$slug}}) {
        edges {
          node {
            path
          }
        }
      }
  }
`