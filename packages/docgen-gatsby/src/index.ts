import * as build from 'gatsby/dist/commands/build'
import * as bootstrap from 'gatsby/dist/bootstrap'
import * as serve from 'gatsby/dist/commands/serve'
import * as develop from 'gatsby/dist/commands/develop'
import * as fs from 'fs'
import * as fsExtra from 'fs-extra'
import * as path from 'path';
import * as util from 'util'

const exists = util.promisify(fs.exists);
const unlink = util.promisify(fs.unlink);
const copy = util.promisify(fsExtra.copy);
const remove = util.promisify(fsExtra.remove);

export async function buildDirectory(): Promise<void> {
    let sourceConfigFileLocation = path.join(__dirname, 'default-gatsby-config.js');
    let destConfigLocation = path.join(process.cwd(), 'gatsby-config.js');

    if (await exists(destConfigLocation)) {
        await unlink(destConfigLocation);
    }

    await copy(sourceConfigFileLocation, destConfigLocation);

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
        port: 8000,
        open: true
    });
}

export function developDirectory() {
    develop({
        directory: process.cwd(),
        sitePackageJson: {
            name: "docgen"
        },
        port: 8000,
        open: true
    });
}

export async function cleanDirectory() {
    let cacheDir = path.join(process.cwd(), '.cache');
    let buildDir = path.join(process.cwd(), 'public');
    if(await exists(cacheDir)) {
        remove(cacheDir);
    }
    if(await exists(buildDir)) {
        remove(buildDir);
    }
}