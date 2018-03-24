export const menuFragement = graphql`
  fragment menu on RootQueryType {
    menu: allSitePage(filter: {path: {eq:$slug}}) {
        edges {
          node {
            path
            menu
          }
        }
      }
  }
`