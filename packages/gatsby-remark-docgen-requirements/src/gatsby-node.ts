export type GraphqlRunner = (query:string, context?: any) => Promise<any>;

let storedGraphqlQuery:GraphqlRunner = null;

export const createPages = async ({ graphql, boundActionCreators, reporter }: {graphql: GraphqlRunner, boundActionCreators: any, reporter: any}) => {
    const { createPage } = boundActionCreators;
    storedGraphqlQuery = graphql;
}

export const getGraphqlRunner = () => {
    return storedGraphqlQuery;
}