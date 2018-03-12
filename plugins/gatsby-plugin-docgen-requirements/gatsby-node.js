const fs = require('fs');
const crypto = require('crypto')
const { parseRequirementDir } = require('../../requirements');

exports.sourceNodes = async (
  { boundActionCreators, reporter },
  pluginOptions
) => {
  const { createNode } = boundActionCreators

  if (!(pluginOptions && pluginOptions.path)) {
    reporter.panic('Path is required.');
  }

  if (!fs.existsSync(pluginOptions.path)) {
    reporter.panic('Path does not exist.');
  }

  var userNeeds = await parseRequirementDir(pluginOptions.path);

  userNeeds.forEach(userNeed => {
    const contentDigest = crypto
      .createHash('md5')
      .update('test')
      .digest('hex');
    createNode({
      id: 'test',
      ...userNeed,
      parent: null,
      children: [],
      internal: {
        type: "requirement",
        contentDigest
      }
    })
  });

}
