import * as build from 'gatsby/dist/commands/build'
import * as bootstrap from 'gatsby/dist/bootstrap';
import * as serve from 'gatsby/dist/commands/serve'
import * as fs from 'fs'
import * as fsExtra from 'fs-extra'
import * as path from 'path';
import * as util from 'util'

const fileExists = util.promisify(fs.exists);
const fileUnlink = util.promisify(fs.unlink);
const copyFile = util.promisify(fsExtra.copy);

export async function buildDirectory(): Promise<void> {
    let sourceConfigFileLocation = path.join(__dirname, 'default-gatsby-config.js');
    let destConfigLocation = path.join(process.cwd(), 'gatsby-config.js');

    if (await fileExists(destConfigLocation)) {
        await fileUnlink(destConfigLocation);
    }

    await copyFile(sourceConfigFileLocation, destConfigLocation);

    await build({
        directory: process.cwd(),
        sitePackageJson: {
            name: "docgen"
        },
        useYarn: true
    });

    return Promise.resolve();
}

export function serveDirectory() {
    serve({
        directory: process.cwd(),
        port: 8080,
        open: true
    });
}