const assert = require('assert');
const mock = require('mock-fs');
const { buildTestContent, buildReqContent } = require('./utils');
const assertThrows = require('assert-throws-async');

describe('requirements', () => {

  describe('loadReqDir', () => {

    const { loadReqDir } = require('docgen-requirements');

    afterEach(() => {
        mock.restore();
    });

    it('should load valid req dir', async() => {
        mock({
        userNeed1 : {
            'index.md': buildReqContent({ number: '1.0.0' }),
            tests: {
                'test1.md': buildTestContent({ number: '2.0.0' }),
                'test2.md': buildTestContent({ number: '3.0.0' })
            },
            productRequirement: {
                'index.md': buildReqContent({ number: '4.0.0' }),
                tests: {
                    'test1.md': buildTestContent({ number: '5.0.0' }),
                    'test2.md': buildTestContent({ number: '6.0.0' })
                },
                softwareRequirement: {
                    'index.md': buildReqContent({ number: '7.0.0' }),
                    tests: {
                        'test1.md': buildTestContent({ number: '8.0.0' }),
                        'test2.md': buildTestContent({ number: '9.0.0' })
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


    it('should load reqs sorted by number', async() => {
        mock({
            userNeed1 : {
                'index.md': buildReqContent({ number: '1.0.0' })
            },
            userNeed2 : {
                'index.md': buildReqContent({ number: '3.0.0' })
            },
            userNeed3 : {
                'index.md': buildReqContent({ number: '2.0.0' }),
                productRequirement1: {
                    'index.md': buildReqContent({ number: '1.0.0' })
                },
                productRequirement2: {
                    'index.md': buildReqContent({ number: '3.0.0' })
                },
                productRequirement3: {
                    'index.md': buildReqContent({ number: '2.0.0' })
                }
            }
        });
        var result = await loadReqDir('.');
        assert.equal(result.length, 3);
        assert.equal(result[0].number, '1.0.0');
        assert.equal(result[1].number, '2.0.0');
        assert.equal(result[2].number, '3.0.0');
        assert.equal(result[1].children[0].number, '1.0.0');
        assert.equal(result[1].children[1].number, '2.0.0');
        assert.equal(result[1].children[2].number, '3.0.0');
    });

    it('throw exception with duplicate numbers', async() => {
        mock({
            userNeed1 : {
                'index.md': buildReqContent({ number: '1.0.0' })
            },
            userNeed2 : {
                'index.md': buildReqContent({ number: '1.0.0' })
            }
        });
        await assertThrows(async() => {
            await loadReqDir('.');
        }, 'Duplicate req number 1.0.0');
    });

    it('throw exception with duplicate numbers nested', async() => {
        mock({
            userNeed1 : {
                'index.md': buildReqContent({ number: '1.0.0' }),
                productRequirement1: {
                    'index.md': buildReqContent({ number: '2.0.0' })
                },
                productRequirement2: {
                    'index.md': buildReqContent({ number: '2.0.0' })
                }
            }
        });
        await assertThrows(async() => {
            await loadReqDir('.');
        }, 'Duplicate req number 2.0.0');
    });

  });

});