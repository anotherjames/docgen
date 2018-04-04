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
        var result = await parseTest(buildTestContent({}));
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
                buildTestContent({ number: null })
            );
        }, 'Number is required');
        await assertThrows(async() => {
            await parseTest(
                buildTestContent({ number: '' })
            );
        }, 'Number is required');
    });

    it('throws exception with invalid number', async() => {
        await assertThrows(async() => {
            await parseTest(
                buildTestContent({ number: '1.0.ds0' })
            );
        }, 'Invalid number 1.0.ds0')
    });

    it('throws exception with empty response type', async() => {
        await assertThrows(async() => {
            await parseTest(
                buildTestContent({ responseType: '' })
            );
        }, 'Response type is required');
    });

    it('throws exception with invalid response type', async() => {
        await assertThrows(async() => {
            await parseTest(
                buildTestContent({ responseType: 'df' })
            );
        }, 'Invalid response type df');
    });

    it('throws exception with empty validation type', async() => {
        await assertThrows(async() => {
            await parseTest(
                buildTestContent({ validationType: '' })
            );
        }, 'Validation type is required');
    });

    it('throws exception with invalid validation type', async() => {
        await assertThrows(async() => {
            await parseTest(
                buildTestContent({ validationType: 'dfg' })
            );
        }, 'Invalid validation type dfg');
    });

    it('throws exception with empty test type', async() => {
        await assertThrows(async() => {
            await parseTest(
                buildTestContent({ type: '' })
            );
        }, 'Test type is required');
    });

    it('throws exception with invalid test type', async() => {
        await assertThrows(async() => {
            await parseTest(
                buildTestContent({ type: 'sdf' })
            );
        }, 'Invalid test type sdf');
    });

    it('throws exception with empty action', async() => {
        await assertThrows(async() => {
            await parseTest(
                buildTestContent({ action: '' })
            );
        }, 'Action is required');
    });

    it('throws exception with empty expected', async() => {
        await assertThrows(async() => {
            await parseTest(
                buildTestContent({ expected: '' })
            );
        }, 'Expected is required');
    });

    it('parses multi line correctly', async() => {
        let action = `

Action line 1

Action line 2
Action line 3

`
        let expected = `

Expected line 1

Expected line 2
Expected line 3

`
        let test = await parseTest(buildTestContent({
            action,
            expected
        }));

        let actionExpected = `Action line 1

Action line 2
Action line 3`
        let expectedExpected = `Expected line 1

Expected line 2
Expected line 3`

        assert.equal(test.action, actionExpected);
        assert.equal(test.expected, expectedExpected);
    });

  });

});