import * as yargs from 'yargs'

let command = 'content <command>';
export { command }

export function builder(argv: yargs.Argv): yargs.Argv {
    return argv.commandDir('./commands').strict().demandCommand();
}