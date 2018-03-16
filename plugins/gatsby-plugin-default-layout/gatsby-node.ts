export const onCreatePage = ({ page, boundActionCreators }) => {
    const { createPage, deletePage } = boundActionCreators;
    let newPage = page || {};
    if (!newPage.layout) {
        newPage.layout = 'index';
        deletePage(newPage);
        createPage(newPage);
    }
};
