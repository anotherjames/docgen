export const pageTreeFragement = graphql`
  fragment pageTree on RootQueryType {
    currentPage: allSitePage(filter: {path: {eq:$slug}}) {
        edges {
          node {
            menu
            path
          }
        }
      }
  }
`