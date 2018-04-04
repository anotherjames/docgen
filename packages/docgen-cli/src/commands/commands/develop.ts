import * as yargs from 'yargs'
import { developDirectory } from 'docgen-gatsby'
import * as reporter from 'gatsby-cli/lib/reporter'

let command = 'develop';
export { command }

export async function handler(argv: yargs.Argv) {
    try {
        await developDirectory();
    } catch(err) {
        reporter.panic(err);
    }
}