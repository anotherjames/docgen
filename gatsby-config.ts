module.exports = {
  siteMetadata: {
    title: 'Gatsby Default Starter'
  },
  plugins: [
    'gatsby-plugin-react-helmet',
    {
      resolve: 'gatsby-source-docgen-requirements',
      options: {
        path: `${__dirname}/src/requirements/`
      }
    },
    {
      resolve: 'gatsby-content-docgen',
      options: {
        baseUrl: '/requirements/'
      }
    }
  ]
};
