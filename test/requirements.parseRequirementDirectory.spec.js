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
            productRequirement: {
                'index.md': buildReqContent('1.0.0', 'Test product req', '', '', ''),
                softwareRequirement: {
                    'index.md': buildReqContent('1.0.0', 'Test software req', '', '', '')
                }
            }
        }
      });
      var result = await parseRequirementDir('.');
      assert.equal(result.length, 1);
      assert.equal(result[0].title, "Test user need");
      assert.equal(result[0].productRequirements.length, 1);
      assert.equal(result[0].productRequirements[0].title, "Test product req");
      assert.equal(result[0].productRequirements[0].softwareRequirements.length, 1);
      assert.equal(result[0].productRequirements[0].softwareRequirements[0].title, "Test software req");
    });

    it('should parse reqs with tests', async() => {
      mock({
        userNeed1 : {
            'index.md': buildReqContent('1.0.0', 'Test user need', '', '', ''),
            tests: {
              'index.md': buildTestContent('1.0.0', "passFail", "verification", "software", "action", "expected")
            }
        }
      });
      var result = await parseRequirementDir('.');
      assert.equal(result.length, 1);
      assert.equal(result[0].title, "Test user need");
      assert.equal(result[0].tests.length, 1);
      assert.equal(result[0].tests[0].action, "action");
    });

  });
});