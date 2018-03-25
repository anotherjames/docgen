import * as yargs from 'yargs'

let command = 'generate <command>';
export { command }

export function builder(argv: yargs.Argv): yargs.Argv {
    return argv.commandDir('./commands').strict().demandCommand();
}