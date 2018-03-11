const buildReqContent = function buildReqContent(number, title, category, description, validation) {
    return `---
number: ${number}
title: ${title}
category: ${category}
---
# Description
${description}
# Validation
${validation}`;
}

module.exports = {
    buildReqContent: buildReqContent
}