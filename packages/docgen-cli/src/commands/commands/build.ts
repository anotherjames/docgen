import * as yargs from 'yargs'
import { buildDirectory } from 'docgen-gatsby'
import * as reporter from 'gatsby-cli/lib/reporter'

let command = 'build';
export { command }

export async function handler(argv: yargs.Argv) {
    //await buildDirectory(process.cwd());
    //reporter.panic('test');
    process.exit(0);
}