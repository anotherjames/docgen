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

const childDirs = async(dir) => {
    var paths = await promisify(fs.readdir)(dir);
    var stats = await Promise.all(paths.map(async(p) => {
        return {
            stat: await promisify(fs.lstat)(path.join(dir, p)),
            path: path.join(dir, p)
        };
    }));
    return stats.filter(stat => stat.stat.isDirectory())
        .map(stat => stat.path);
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
    return userNeed;
}

const parseRequirementDir = async(dir) => {
    var r = await Promise.all((await childDirs(dir))
        .map(parseUserNeedDir));
    return r;
}

module.exports = {
    parseRequirement: parseRequirement,
    parseRequirementDir: parseRequirementDir
}