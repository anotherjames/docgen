const assert = require('assert');
const mock = require('mock-fs');
const { buildTestContent, buildReqContent } = require('./utils');
const assertThrows = require('assert-throws-async');

describe('requirements', () => {

  describe('loadTests', () => {

    const { loadTests } = require('docgen-requirements');
    afterEach(() => {
      mock.restore();
    });

    it('sorts by number', async() => {
        mock({
            'test1.md': buildTestContent({ number: '1.0.0' }),
            'test2.md': buildTestContent({ number: '3.0.0' }),
            'test3.md': buildTestContent({ number: '2.0.0' })
        });
        var req = { id: 'id', tests: [] };
        await loadTests('.', '.', req);
        assert.equal(req.tests.length, 3);
        assert.equal(req.tests[0].number, '1.0.0');
        assert.equal(req.tests[1].number, '2.0.0');
        assert.equal(req.tests[2].number, '3.0.0');
    });

    it('throw exception with duplicate numbers', async() => {
        mock({
            'test1.md': buildTestContent({ number: '1.0.0' }),
            'test2.md': buildTestContent({ number: '1.0.0' })
        });
        await assertThrows(async() => {
            var req = { id: 'id', tests: [] };
            await loadTests('.', '.', req);
        }, 'Duplicate test number 1.0.0');
    });

  });

});