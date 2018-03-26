module.exports = {
    siteMetadata: {
        title: 'Docgen'
    },
    plugins: [
        'gatsby-plugin-react-helmet',
        'gatsby-plugin-default-layout',
        // Soure in the product requirements as data nodes.
        {
            resolve: 'gatsby-source-docgen-requirements',
            options: {
                path: `${__dirname}/src/requirements/`
            }
        },
        // Source in the markdown files.
        {
            resolve: 'gatsby-source-filesystem',
            options: {
                path: `${__dirname}/src/content`,
                name: 'content',
            },
        },
        // Transform the markdown files to html.
        {
            resolve: 'gatsby-transformer-remark',
            options: {
                plugins: [
                    {
                        resolve: 'gatsby-remark-docgen-requirements'
                    }
                ]
            }
        },
        // Create pages based on the requirements and markdown files.
        {
            resolve: 'gatsby-content-docgen',
            options: {
                baseUrl: '/requirements/',
            }
        },
        'gatsby-plugin-page-tree',
        'gatsby-plugin-typescript',
        'gatsby-plugin-less'
    ]
};
//# sourceMappingURL=default-gatsby-config.js.map