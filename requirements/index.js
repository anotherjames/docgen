const Promise = require('bluebird');
const fs = require('fs');
const grayMatter = require('gray-matter')

const parseUserNeed = function(file) {
    return new Promise(function (resolve, reject) {
        fs.readFile(file, "utf8", (err, data) => {
            if (err) {
                reject(err);
            } else {
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

                resolve(matter.data);
            }
        });
    });
}

module.exports = {
    parseUserNeed: parseUserNeed
}