#!/usr/bin/env node

const { execSync, spawn } = require('child_process');

function runPackage(package) {
    var module = require(package);

    // Skip the first entry of argv (which is the name of the current process).
    var args = [];
    process.argv.forEach((val, index) => {
        console.log(index);
        if(index > 1) {
            console.log("pushing " + val);
            args.push(val);
        }
    });

    const child = spawn(module.path, args, {
        stdio: 'inherit'
    });
    
    child.on("exit", code => process.exit(code));
}

if (process.platform === "darwin") {
    runPackage("docgen-cli-osx-xsixtyfour");
} else if(process.platform === "win32") {
    runPackage("docgen-cli-win-xsixtyfour");
} else if(process.platform === "linux") {
    runPackage("docgen-cli-linux-xsixtyfour");
} else {
    console.log("Unsupported platform: " + process.platform);
    process.exit(1);
}