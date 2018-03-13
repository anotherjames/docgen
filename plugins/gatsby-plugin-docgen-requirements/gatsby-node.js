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
    createNode({
      ...userNeed,
      parent: null,
      children: [],
      internal: {
        type: 'userNeed',
        contentDigest: crypto.createHash('md5').update(userNeed.id).digest('hex')
      }
    });
    userNeed.productRequirements.forEach(productRec => {
      createNode({
        ...productRec,
        parent: userNeed.id,
        children: [],
        internal : {
          type: 'productSpec',
          contentDigest: crypto.createHash('md5').update(productRec.id).digest('hex')
        }
      });
      productRec.softwareRequirements.forEach(softwareReq => {
        createNode({
          ...productRec,
          parent: userNeed.id,
          children: [],
          internal : {
            type: 'softwareReq',
            contentDigest: crypto.createHash('md5').update(softwareReq.id).digest('hex')
          }
        });
      });
    });
  });
}
