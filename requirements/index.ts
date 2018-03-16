import * as matter from 'gray-matter'
import * as fs from 'fs'
import * as path from 'path';
import * as util from 'util'
import './test'
import Test, { TestResponseType, TestType, TestValidationType } from './test';
import Req from './req';

const readdir = util.promisify(fs.readdir)
const readfile = util.promisify(fs.readFile)
const lstat = util.promisify(fs.lstat);

async function getChildDirectories(dir: string): Promise<string[]> {
    let childDirs = await readdir(dir);
    let result: string[] = [];

    for(let childDir of childDirs) {
        var stat = await lstat(path.join(dir, childDir));
        if (stat.isDirectory()) {
            result.push(childDir);
        }
    }

    return result;
}

async function getChildFiles(dir: string): Promise<string[]> {
    let childDirs = await readdir(dir);
    let result: string[] = [];

    for(let childDir of childDirs) {
        var stat = await lstat(path.join(dir, childDir));
        if (!stat.isDirectory()) {
            result.push(childDir);
        }
    }

    return result;
}

export function parseReq(content: string): Req {
    let d = matter(content);
    let lines = (d.content as string).split('\n');
    
    let description: string[] = []
    let validation: string[] = [];
    let current: string;

    lines.forEach(line => {
        if (line == "# Description") {
            current = "description";
        } else if (line == "# Validation") {
            current = "validation";
        } else {
            switch(current) {
            case "description":
                description.push(line);
                break;
            case "validation":
                validation.push(line);
                break;
            default:
                // TODO: error
                break;
            }
        }
    });

    let req = new Req();
    req.number = d.data.number;
    req.title = d.data.title;
    req.category = d.data.category;
    req.description = description.join();
    req.validation = validation.join();

    return req;
}

export async function loadReq(path: string): Promise<Req> {
    console.log(path);
    return parseReq(await readfile(path, "utf8"));
}

export function parseTest(content: string): Test {
    let d = matter(content);
    let lines = (d.content as string).split('\n');
    
    let action: string[] = []
    let expected: string[] = [];
    let current: string;

    lines.forEach(line => {
        if (line == "# Action") {
            current = "action";
        } else if (line == "# Expected") {
            current = "expected";
        } else {
            switch(current) {
            case "action":
                action.push(line);
                break;
            case "expected":
                expected.push(line);
                break;
            default:
                // TODO: error
                break;
            }
        }
    });

    let test = new Test();
    test.number = d.data.number;
    test.responseType = d.data.responseType as TestResponseType;
    test.validationType = d.data.validationType as TestValidationType;
    test.type = d.data.type as TestType;
    test.action = action.join();
    test.expected = expected.join();

    return test;
}

export async function loadTest(path: string): Promise<Test> {
    return parseTest(await readfile(path, "utf8"));
}

export async function loadTests(baseDir: string, dir: string, parent: Req) {
    for (let childFile of (await getChildFiles(path.join(baseDir, dir)))) {
        let test = await loadTest(path.join(baseDir, dir, childFile));
        test.path = path.join(dir, path.parse(childFile).name);
        test.id = parent.id + "-test-" + path.parse(childFile).name;
        parent.tests.push(test);
    }
}

async function loadReqWithChildren(baseDir: string, dir: string, parent: Req): Promise<Req> {
    let req = await loadReq(path.join(baseDir, dir, "index.md"));
    req.path = dir;
    if(parent != null) {
        req.id = parent.id + "-" + req.number;
    } else {
        req.id = req.number;
    }
    for (let childDir of (await getChildDirectories(path.join(baseDir, dir)))) {
        if (childDir == "tests") {
            await loadTests(baseDir, path.join(dir, childDir), req);
        } else {
            req.children.push(await loadReqWithChildren(baseDir, path.join(dir, childDir), req));
        }
    }
    return req;
}

export async function loadReqDir(dir: string) {
    let reqs: Req[] = [];
    for (let childDir of (await getChildDirectories(dir))) {
        if (childDir == "tests") {
            // Not supported
        } else {
            reqs.push(await loadReqWithChildren(dir, childDir, null));
        }
    }
    return reqs;
}