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

const buildTestContent = function buildReqContent(number, responseType, validationType, type, action, expected) {
    return `---
number: ${number}
responseType: ${responseType}
validationType: ${validationType}
type: ${type}
---
# Action
${action}
# Expected
${expected}`;
}

module.exports = {
    buildReqContent: buildReqContent,
    buildTestContent: buildTestContent
}