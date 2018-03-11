const fs = require('fs');
const grayMatter = require('gray-matter')
const { promisify } = require('util');

const parseUserNeed = async function(file) {
    var data = await promisify(fs.readFile)(file, "utf8");
    matter = grayMatter(data);
    lines = matter.content.split('\n')

    var description = "";
    var validation = "";
    var current = null;
    lines.forEach(line => {
        if (line == "# User need") {
            current = "description";
        } else if (line == "# Validation method") {
            current = "validationMethod";
        } else {
            switch(current) {
            case "description":
                description += line;
                break;
            case "validationMethod":
                validation += line;
                break;
            default:
                // TODO: error
                break;
            }
        }
    });

    matter.data.description = description;
    matter.data.validationMethod = validation;

    return matter.data;
}

module.exports = {
    parseUserNeed: parseUserNeed
}