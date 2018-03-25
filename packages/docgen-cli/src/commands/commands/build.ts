import * as yargs from 'yargs'
import { buildDirectory } from 'docgen-gatsby'
import * as reporter from 'gatsby-cli/lib/reporter'

let command = 'build';
export { command }

export async function handler(argv: yargs.Argv) {
    try {
        await buildDirectory();
    } catch(err) {
        reporter.panic(err);
    }
    process.exit(0);
}