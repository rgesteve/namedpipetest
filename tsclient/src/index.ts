import * as cp from 'child_process';

let proc : cp.ChildProcess = cp.spawn('wc', ['-c'], {'stdio' : ['pipe']});

proc.stdout.on('data', (data) => { console.log(`Received some data (${data}) from subprocess`); })
proc.stdout.on('error', (data) => { console.log("Got an error"); })

proc.stdin.write('hello world');
proc.stdin.end();		

console.log("Done!");
