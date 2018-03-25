#!/usr/bin/env node

import * as yargs from 'yargs'

let args = yargs
    .usage('usage: $0 <command>')
    .commandDir('./commands')
    .strict()
    .demandCommand();

args.parse();