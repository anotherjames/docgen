import * as yargs from 'yargs'
import { serveDirectory } from 'docgen-gatsby'
import * as reporter from 'gatsby-cli/lib/reporter'

let command = 'serve';
export { command }

export function handler(argv: yargs.Argv) {
    try {
        serveDirectory();
    } catch(err) {
        reporter.panic(err);
    }
}