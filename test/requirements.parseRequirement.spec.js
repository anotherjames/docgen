const assert = require('assert');
const mock = require('mock-fs');
const { buildReqContent } = require('./utils');

describe('requirements', () => {
  describe('parseRequirement', () => {

    const { parseRequirement } = require('../requirements');

    afterEach(() => {
      mock.restore();
    });

    it('should parse valid requirement', async() => {
      mock({
        'requirement.md': buildReqContent('1.0.0', 'Test title', 'Category', 'Requirement content...', 'Validation content...')
      });
      var result = await parseRequirement('requirement.md');
      assert.equal(result.number, '1.0.0');
      assert.equal(result.title, 'Test title');
      assert.equal(result.category, 'Category');
      assert.equal(result.description, 'Requirement content...');
      assert.equal(result.validation, 'Validation content...');
    });

  });
});