const assert = require('assert');
const mock = require('mock-fs');
const { buildTestContent, buildReqContent } = require('./utils');

describe('requirements', () => {

  describe('loadTest', () => {

    const { loadTest } = require('../requirements');

    afterEach(() => {
      mock.restore();
    });

    it('should parse valid test', async() => {
      mock({
        'test.md': buildTestContent('1.0.0', 'passFail', 'verification', 'software', 'action', 'expected')
      });
      var result = await loadTest('test.md');
      assert.equal(result.number, '1.0.0');
      assert.equal(result.responseType, "passFail");
      assert.equal(result.validationType, "verification");
      assert.equal(result.type, "software");
      assert.equal(result.action, "action");
      assert.equal(result.expected, "expected");
    });

  });

  describe('loadReq', () => {

    const { loadReq } = require('../requirements');

    afterEach(() => {
        mock.restore();
    });

    it('should load valid req', async() => {
        mock({
            'requirement.md': buildReqContent('1.0.0', 'Test title', 'Category', 'Requirement content...', 'Validation content...')
        });
        var result = await loadReq('requirement.md');
        assert.equal(result.number, '1.0.0');
        assert.equal(result.title, 'Test title');
        assert.equal(result.category, 'Category');
        assert.equal(result.description, 'Requirement content...');
        assert.equal(result.validation, 'Validation content...');
    });

  });

  describe('loadReqDir', () => {

    const { loadReqDir } = require('../requirements');

    afterEach(() => {
        mock.restore();
    });

    it('should load valid req dir', async() => {
        mock({
        userNeed1 : {
            'index.md': buildReqContent('1.0.0', 'Test user need', '', '', ''),
            tests: {
                'test1.md': buildTestContent('2.0.0', "passFail", "verification", "software", "action1", "expected1"),
                'test2.md': buildTestContent('3.0.0', "passFail", "verification", "software", "action2", "expected2")
            },
            productRequirement: {
                'index.md': buildReqContent('4.0.0', 'Test product req', '', '', ''),
                tests: {
                    'test1.md': buildTestContent('5.0.0', "passFail", "verification", "software", "action1", "expected1"),
                    'test2.md': buildTestContent('6.0.0', "passFail", "verification", "software", "action2", "expected2")
                },
                softwareRequirement: {
                    'index.md': buildReqContent('7.0.0', 'Test software req', '', '', ''),
                    tests: {
                        'test1.md': buildTestContent('8.0.0', "passFail", "verification", "software", "action1", "expected1"),
                        'test2.md': buildTestContent('9.0.0', "passFail", "verification", "software", "action2", "expected2")
                    }
                }
            }
        }
        });
        var result = await loadReqDir('.');
        assert.equal(result.length, 1);
        assert.equal(result[0].id, "1.0.0");
        assert.equal(result[0].number, "1.0.0");
        assert.equal(result[0].path, "userNeed1");
        assert.equal(result[0].tests.length, 2);
        assert.equal(result[0].tests[0].number, "2.0.0");
        assert.equal(result[0].tests[0].path, "userNeed1/tests/test1");
        assert.equal(result[0].tests[1].number, "3.0.0");
        assert.equal(result[0].tests[1].path, "userNeed1/tests/test2");
        assert.equal(result[0].children.length, 1);
        assert.equal(result[0].children[0].id, "1.0.0-4.0.0");
        assert.equal(result[0].children[0].number, "4.0.0");
        assert.equal(result[0].children[0].path, "userNeed1/productRequirement");
        assert.equal(result[0].children[0].tests.length, 2);
        assert.equal(result[0].children[0].tests[0].number, "5.0.0");
        assert.equal(result[0].children[0].tests[0].path, "userNeed1/productRequirement/tests/test1");
        assert.equal(result[0].children[0].tests[1].number, "6.0.0");
        assert.equal(result[0].children[0].tests[1].path, "userNeed1/productRequirement/tests/test2");
        assert.equal(result[0].children[0].children.length, 1);
        assert.equal(result[0].children[0].children[0].id, "1.0.0-4.0.0-7.0.0");
        assert.equal(result[0].children[0].children[0].number, "7.0.0");
        assert.equal(result[0].children[0].children[0].path, "userNeed1/productRequirement/softwareRequirement");
        assert.equal(result[0].children[0].children[0].tests.length, 2);
        assert.equal(result[0].children[0].children[0].tests[0].number, "8.0.0");
        assert.equal(result[0].children[0].children[0].tests[0].path, "userNeed1/productRequirement/softwareRequirement/tests/test1");
        assert.equal(result[0].children[0].children[0].tests[1].number, "9.0.0");
        assert.equal(result[0].children[0].children[0].tests[1].path, "userNeed1/productRequirement/softwareRequirement/tests/test2");
    });

  });

});