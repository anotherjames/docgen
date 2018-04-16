import * as fs from 'fs'
import * as fsExtra from 'fs-extra'
import * as path from 'path';
import * as util from 'util'
import * as md5 from 'md5-file/promise'
import { spawn } from 'child_process';

const exists = util.promisify(fs.exists);
const unlink = util.promisify(fs.unlink);
const copy = util.promisify(fsExtra.copy);

export async function copyFile(sourceFile: string, destinationFile: string): Promise<boolean> {

    let t = await md5(sourceFile);

    if(!(await exists(destinationFile))) {
        await copy(sourceFile, destinationFile);
        return true;
    }

    let sourceHash = await md5(sourceFile);
    let destHash = await md5(destinationFile);

    if(sourceFile == destHash) {
        return false;
    }

    await unlink(destinationFile);
    copy(sourceFile, destinationFile);

    return true;
}

export async function cleanDirectory() {
    let cacheDir = path.join(process.cwd(), '.cache');
    let buildDir = path.join(process.cwd(), 'public');
    if(await exists(cacheDir)) {
        await fsExtra.remove(cacheDir);
    }
    if(await exists(buildDir)) {
        await fsExtra.remove(buildDir);
    }
}

async function prepareDirectory() {
    let currentDirectory = process.cwd();
    let wasChanged = await copyFile(path.join(__dirname, 'default-gatsby-config.js'), path.join(currentDirectory, 'gatsby-config.js'));
    wasChanged = wasChanged || await copyFile(path.join(__dirname, 'default-package.json'), path.join(currentDirectory, 'package.json'));
    if (wasChanged) {
        // Remove existing node modules.
        if(await exists('node_modules')) {
            await fsExtra.remove(path.join(currentDirectory, 'node_modules'));
        }
        // Install node modules again.
        await spawn('npm', ['install']);
    }
    await cleanDirectory();
}

export async function buildDirectory() {
    await prepareDirectory();
    await spawn('./node_modules/.bin/gatsby build');
}

export async function serveDirectory() {
    await prepareDirectory();
    await spawn('./node_modules/.bin/gatsby serve');
}

export async function developDirectory() {
    await prepareDirectory();
    await spawn('./node_modules/.bin/gatsby develop');
}