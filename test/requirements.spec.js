const assert = require('assert');
const mock = require('mock-fs');

describe('requirements', function() {
  describe('parseUserNeed', function() {

    const { parseUserNeed } = require('../requirements');

    afterEach(function() {
      mock.restore();
    });

    it('should parse valid user need', function(done) {
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
      parseUserNeed('userneed.md')
        .then(function(userNeed) {
          assert.equal(userNeed.number, "1.0.0");
          assert.equal(userNeed.title, "Test title");
          assert.equal(userNeed.category, "Category");
          assert.equal(userNeed.description, "User need content...");
          assert.equal(userNeed.validationMethod, "Validation content...");
          done();
        }, function(error) {
          console.log(error)
          done(error);
        });
    });



  });
});