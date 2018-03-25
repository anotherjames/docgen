import * as yargs from 'yargs'

let command = 'serve';
export { command }

export function handler(argv: yargs.Argv) {
    console.log("serving");
}