const assert = require('assert');
const mock = require('mock-fs');
const { buildTestContent } = require('./utils');

describe('requirements', () => {
  describe('parseTest', () => {

    const { parseTest } = require('../requirements');

    afterEach(() => {
      mock.restore();
    });

    it('should parse valid test', async() => {
      mock({
        'test.md': buildTestContent('1.0.0', 'passFail', 'verification', 'software', 'action', 'expected')
      });
      var result = await parseTest('test.md');
      assert.equal(result.number, '1.0.0');
      assert.equal(result.responseType, "passFail");
      assert.equal(result.validationType, "verification");
      assert.equal(result.type, "software");
      assert.equal(result.action, "action");
      assert.equal(result.expected, "expected");
    });

  });
});