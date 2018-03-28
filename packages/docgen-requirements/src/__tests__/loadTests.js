const assert = require('assert');
const mock = require('mock-fs');
const { buildTestContent, buildReqContent } = require('./utils');

describe('requirements', () => {

  describe('loadTests', () => {

    const { loadTests } = require('docgen-requirements');
    afterEach(() => {
      mock.restore();
    });

    it('sorts by number', async() => {
      mock({
        'test1.md': buildTestContent('1.0.0', 'passFail', 'verification', 'software', 'action', 'expected'),
        'test2.md': buildTestContent('3.0.0', 'passFail', 'verification', 'software', 'action', 'expected'),
        'test3.md': buildTestContent('2.0.0', 'passFail', 'verification', 'software', 'action', 'expected')
      });
      var req = { id: 'id', tests: [] };
      await loadTests('.', '.', req);
      assert.equal(req.tests.length, 3);
      assert.equal(req.tests[0].number, '1.0.0');
      assert.equal(req.tests[1].number, '2.0.0');
      assert.equal(req.tests[2].number, '3.0.0');
    });

  });

});