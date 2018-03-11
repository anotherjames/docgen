const fs = require('fs');
const grayMatter = require('gray-matter')
const { promisify } = require('util');
const path = require('path');

const parseRequirement = async(file) => {
    var data = await promisify(fs.readFile)(file, "utf8");
    matter = grayMatter(data);
    lines = matter.content.split('\n')

    var description = "";
    var validation = "";
    var current = null;
    lines.forEach(line => {
        if (line == "# Description") {
            current = "description";
        } else if (line == "# Validation") {
            current = "validation";
        } else {
            switch(current) {
            case "description":
                description += line;
                break;
            case "validation":
                validation += line;
                break;
            default:
                // TODO: error
                break;
            }
        }
    });

    matter.data.description = description;
    matter.data.validation = validation;

    return matter.data;
}

const parseTest = async(file) => {
    var data = await promisify(fs.readFile)(file, "utf8");
    matter = grayMatter(data);
    lines = matter.content.split('\n')

    var action = "";
    var expected = "";
    var current = null;
    lines.forEach(line => {
        if (line == "# Action") {
            current = "action";
        } else if (line == "# Expected") {
            current = "expected";
        } else {
            switch(current) {
            case "action":
                action += line;
                break;
            case "expected":
                expected += line;
                break;
            default:
                // TODO: error
                break;
            }
        }
    });

    matter.data.action = action;
    matter.data.expected = expected;

    return matter.data;
}

const childDirs = async(dir) => {
    var paths = await promisify(fs.readdir)(dir);
    var stats = await Promise.all(paths.map(async(p) => {
        return {
            stat: await promisify(fs.lstat)(path.join(dir, p)),
            path: path.join(dir, p)
        };
    }));
    return stats.filter(stat => stat.stat.isDirectory())
        // Don't show tests, they are parsed differently.
        .filter(stat => path.basename(stat.path) != "tests")
        .map(stat => stat.path);
}

const childFiles = async(dir) => {
    var paths = await promisify(fs.readdir)(dir);
    var stats = await Promise.all(paths.map(async(p) => {
        return {
            stat: await promisify(fs.lstat)(path.join(dir, p)),
            path: path.join(dir, p)
        };
    }));
    return stats.filter(stat => stat.stat.isFile())
        .map(stat => stat.path);
}

const parseTests = async(dir) => {
    var tests = [];
    exists = await promisify(fs.exists)(path.join(dir, 'tests'));
    if(!exists) {
        return tests;
    }
    var tests = (await childFiles(path.join(dir, "tests")))
        .filter(test => path.extname(test) == '.md')
        .map(test => parseTest(test));
    
    return Promise.all(tests);
}

const parseSoftwareReqDir = async(dir) => {
    var softwareReq = await parseRequirement(path.join(dir, 'index.md'), "utf8");
    return softwareReq;
}

const parseProductReqDir = async(dir) => {
    var productReq = await parseRequirement(path.join(dir, 'index.md'), "utf8");
    productReq.softwareRequirements = await Promise.all((await childDirs(dir))
        .map(parseSoftwareReqDir));
    return productReq;
}

const parseUserNeedDir = async(dir) => {
    var userNeed = await parseRequirement(path.join(dir, 'index.md'), "utf8");
    userNeed.productRequirements = await Promise.all((await childDirs(dir))
        .map(parseProductReqDir));
    userNeed.tests = await parseTests(dir);
    return userNeed;
}

const parseRequirementDir = async(dir) => {
    var userNeeds = await Promise.all((await childDirs(dir))
        .map(parseUserNeedDir));
    return userNeeds;
}

module.exports = {
    parseRequirement: parseRequirement,
    parseTest: parseTest,
    parseRequirementDir: parseRequirementDir
}