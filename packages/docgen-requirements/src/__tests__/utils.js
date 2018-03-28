const buildReqContent = function buildReqContent(content) {
    var o = {
        number: "1.0.0",
        title: "title",
        category: "category",
        description: "description",
        validation: "validation"
    };
    for(var k in content) {
        o[k] = content[k];
    }
    return `---
number: ${o.number}
title: ${o.title}
category: ${o.category}
---
# Description
${o.description}
# Validation
${o.validation}`;
}

const buildTestContent = function buildReqContent(content) {
    var o = {
        number: "1.0.0",
        responseType: "passFail",
        validationType: "verification",
        type: "software",
        action: "action",
        expected: "expected"
    };
    for(var k in content) {
        o[k] = content[k];
    }
    return `---
number: ${o.number}
responseType: ${o.responseType}
validationType: ${o.validationType}
type: ${o.type}
---
# Action
${o.action}
# Expected
${o.expected}`;
}

module.exports = {
    buildReqContent: buildReqContent,
    buildTestContent: buildTestContent
}