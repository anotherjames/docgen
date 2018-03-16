module.exports = {
  siteMetadata: {
    title: 'Gatsby Default Starter'
  },
  plugins: [
    'gatsby-plugin-react-helmet',
    {
      resolve: 'gatsby-plugin-docgen-requirements',
      options: {
        path: `${__dirname}/src/requirements/`,
        baseUrl: '/requirements/'
      }
    }
  ]
};
