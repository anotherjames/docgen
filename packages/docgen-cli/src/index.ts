#!/usr/bin/env node

import * as yargs from 'yargs'
import { command } from './commands/generate';

let args = yargs
    .usage('usage: $0 <command>')
    .commandDir('./commands')
    .strict()
    .demandCommand();

args.parse();