import * as yargs from 'yargs'

let command = 'build';
export { command }

export function handler(argv: yargs.Argv) {
    console.log("building");
}