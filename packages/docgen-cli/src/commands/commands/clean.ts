import * as yargs from 'yargs'
import { cleanDirectory } from 'docgen-gatsby'
import * as reporter from 'gatsby-cli/lib/reporter'

let command = 'clean';
export { command }

export async function handler(argv: yargs.Argv) {
    try {
        await cleanDirectory();
    } catch(err) {
        reporter.panic(err);
    }
}