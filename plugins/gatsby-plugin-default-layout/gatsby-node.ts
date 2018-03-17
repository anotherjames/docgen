export const onCreatePage = ({ page, boundActionCreators }, pluginOptions) => {
    const { createPage, deletePage } = boundActionCreators;
    let newPage = page || {};
    if (!newPage.layout) {
        newPage.layout = 'index';
        deletePage(newPage);
        createPage(newPage);
    }
};

export const sourceNodes = async ({ boundActionCreators }, pluginOptions) => {
    console.log(pluginOptions);
}