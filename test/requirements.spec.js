const assert = require('assert');
const mock = require('mock-fs');

describe('requirements', () => {
  describe('parseUserNeed', () => {

    const { parseUserNeed } = require('../requirements');

    afterEach(() => {
      mock.restore();
    });

    it('should parse valid user need', async() => {
      mock({
        'userneed.md': 
`---
number: 1.0.0
title: Test title
category: Category
---
# User need
User need content...
# Validation method
Validation content...`
      });
      var result = await parseUserNeed('userneed.md');
      assert.equal(result.number, "1.0.0");
      assert.equal(result.title, "Test title");
      assert.equal(result.category, "Category");
      assert.equal(result.description, "User need content...");
      assert.equal(result.validationMethod, "Validation content...");
    });

  });
});