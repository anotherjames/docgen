const assert = require('assert');
const mock = require('mock-fs');
const { buildTestContent } = require('./utils');
const assertThrows = require('assert-throws-async'); 

describe('requirements', () => {

  describe('parseTest', () => {

    const { parseTest } = require('docgen-requirements');

    afterEach(() => {
      mock.restore();
    });

    it('should parse valid test', async() => {
        var result = await parseTest(
          buildTestContent('1.0.0',
            'passFail',
            'verification',
            'software',
            'action',
            'expected')
        );
        assert.equal(result.number, '1.0.0');
        assert.equal(result.responseType, "passFail");
        assert.equal(result.validationType, "verification");
        assert.equal(result.type, "software");
        assert.equal(result.action, "action");
        assert.equal(result.expected, "expected");
    });

    it('throws exception with empty number', async() => {
        await assertThrows(async() => {
            await parseTest(
                buildTestContent(null,
                    'passFail',
                    'verification',
                    'software',
                    'action',
                    'expected')
            );
        }, 'Number is required');
        await assertThrows(async() => {
            await parseTest(
                buildTestContent('',
                    'passFail',
                    'verification',
                    'software',
                    'action',
                    'expected')
            );
        }, 'Number is required');
    });

    it('throws exception with invalid number', async() => {
        await assertThrows(async() => {
            await parseTest(
                buildTestContent('1.0.sd0',
                    'passFail',
                    'verification',
                    'software',
                    'action',
                    'expected')
            );
        }, 'Invalid number 1.0.sd0')
    });

    it('throws exception with empty response type', async() => {
        await assertThrows(async() => {
            await parseTest(
                buildTestContent('1.0.0',
                    '',
                    'verification',
                    'software',
                    'action',
                    'expected')
            );
        }, 'Response type is required');
    });

    it('throws exception with invalid response type', async() => {
        await assertThrows(async() => {
            await parseTest(
                buildTestContent('1.0.0',
                    'df',
                    'verification',
                    'software',
                    'action',
                    'expected')
            );
        }, 'Invalid response type df');
    });

    it('throws exception with empty validation type', async() => {
        await assertThrows(async() => {
            await parseTest(
                buildTestContent('1.0.0',
                    'passFail',
                    '',
                    'software',
                    'action',
                    'expected')
            );
        }, 'Validation type is required');
    });

    it('throws exception with invalid validation type', async() => {
        await assertThrows(async() => {
            await parseTest(
                buildTestContent('1.0.0',
                    'passFail',
                    'dfg',
                    'software',
                    'action',
                    'expected')
            );
        }, 'Invalid validation type dfg');
    });

    it('throws exception with empty test type', async() => {
        await assertThrows(async() => {
            await parseTest(
                buildTestContent('1.0.0',
                    'passFail',
                    'verification',
                    '',
                    'action',
                    'expected')
            );
        }, 'Test type is required');
    });

    it('throws exception with invalid test type', async() => {
        await assertThrows(async() => {
            await parseTest(
                buildTestContent('1.0.0',
                    'passFail',
                    'verification',
                    'sdf',
                    'action',
                    'expected')
            );
        }, 'Invalid test type sdf');
    });

    it('throws exception with empty action', async() => {
        await assertThrows(async() => {
            await parseTest(
                buildTestContent('1.0.0',
                    'passFail',
                    'verification',
                    'software',
                    '',
                    'expected')
            );
        }, 'Action is required');
    });

    it('throws exception with empty expected', async() => {
        await assertThrows(async() => {
            await parseTest(
                buildTestContent('1.0.0',
                    'passFail',
                    'verification',
                    'software',
                    'action',
                    '')
            );
        }, 'Expected is required');
    });

  });

});