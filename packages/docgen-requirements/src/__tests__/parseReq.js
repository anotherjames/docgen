const assert = require('assert');
const mock = require('mock-fs');
const { buildReqContent } = require('./utils');
const assertThrows = require('assert-throws-async'); 

describe('requirements', () => {

  describe('parseReq', () => {

    const { parseReq } = require('docgen-requirements');

    afterEach(() => {
      mock.restore();
    });

    it('should parse valid req', async() => {
        var result = await parseReq(buildReqContent({}));
        assert.equal(result.number, '1.0.0');
        assert.equal(result.title, 'title');
        assert.equal(result.category, 'category');
        assert.equal(result.description, 'description');
        assert.equal(result.validation, 'validation');
    });

    it('throws exception with empty number', async() => {
        await assertThrows(async() => {
            await parseReq(
                buildReqContent({ number: null })
            );
        }, 'Number is required');
        await assertThrows(async() => {
            await parseReq(
                buildReqContent({ number: '' })
            );
        }, 'Number is required');
    });

    it('throws exception with invalid number', async() => {
        await assertThrows(async() => {
            await parseReq(
                buildReqContent({ number: '1.0.ds0' })
            );
        }, 'Invalid number 1.0.ds0')
    });

    it('throws exception with empty title', async() => {
        await assertThrows(async() => {
            await parseReq(
                buildReqContent({ title: '' })
            );
        }, 'Title is required');
    });

    it('throws exception with empty category', async() => {
        await assertThrows(async() => {
            await parseReq(
                buildReqContent({ category: '' })
            );
        }, 'Category is required');
    });

    it('throws exception with empty description', async() => {
        await assertThrows(async() => {
            await parseReq(
                buildReqContent({ description: '' })
            );
        }, 'Description is required');
    });

    it('throws exception with empty validation', async() => {
        await assertThrows(async() => {
            await parseReq(
                buildReqContent({ validation: '' })
            );
        }, 'Validation is required');
    });

  });

});