const assert = require('assert');
const mock = require('mock-fs');
const { buildReqContent, buildTestContent } = require('./utils');

describe('requirements', () => {
  describe('parseRequirementDir', () => {

    const { parseRequirementDir } = require('../requirements');

    afterEach(() => {
      mock.restore();
    });

    it('should parse valid requirements directory', async() => {
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
      var result = await parseRequirementDir('.');
      assert.equal(result.length, 1);
      assert.equal(result[0].id, "1.0.0");
      assert.equal(result[0].number, "1.0.0");
      assert.equal(result[0].tests.length, 2);
      assert.equal(result[0].tests[0].number, "2.0.0");
      assert.equal(result[0].tests[1].number, "3.0.0");
      assert.equal(result[0].productRequirements.length, 1);
      assert.equal(result[0].productRequirements[0].id, "1.0.0-4.0.0");
      assert.equal(result[0].productRequirements[0].number, "4.0.0");
      assert.equal(result[0].productRequirements[0].tests.length, 2);
      assert.equal(result[0].productRequirements[0].tests[0].number, "5.0.0");
      assert.equal(result[0].productRequirements[0].tests[1].number, "6.0.0");
      assert.equal(result[0].productRequirements[0].softwareRequirements.length, 1);
      assert.equal(result[0].productRequirements[0].softwareRequirements[0].id, "1.0.0-4.0.0-7.0.0");
      assert.equal(result[0].productRequirements[0].softwareRequirements[0].number, "7.0.0");
      assert.equal(result[0].productRequirements[0].softwareRequirements[0].tests.length, 2);
      assert.equal(result[0].productRequirements[0].softwareRequirements[0].tests[0].number, "8.0.0");
      assert.equal(result[0].productRequirements[0].softwareRequirements[0].tests[1].number, "9.0.0");
    });

  });
});