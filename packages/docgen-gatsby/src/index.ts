import * as build from 'gatsby/dist/commands/build'
import * as bootstrap from 'gatsby/dist/bootstrap';
import * as fs from 'fs'
import * as fsExtra from 'fs-extra'
import * as path from 'path';
import * as util from 'util'

const fileExists = util.promisify(fs.exists);
const fileUnlink = util.promisify(fs.unlink);
const copyFile = util.promisify(fsExtra.copy);

export async function buildDirectory(dir: string): Promise<void> {
    let sourceConfigFileLocation = path.join(__dirname, 'default-gatsby-config.js');
    let destConfigLocation = path.join(dir, 'gatsby-config.js');

    if (await fileExists(destConfigLocation)) {
        await fileUnlink(destConfigLocation);
    }

    await copyFile(sourceConfigFileLocation, destConfigLocation);

    await build({
        directory: dir,
        sitePackageJson: {
            name: "docgen"
        },
        useYarn: true
    });

    return Promise.resolve();
}