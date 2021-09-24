import * as fs from 'fs';
import * as ts from 'typescript';
import * as path from 'path';
import * as mkdirp from 'mkdirp';
import { getCompilerOptions, getTsProjectPath } from './lib/util'
import { Debugger } from './lib/debugger';

declare const puertsRequire: any;
const cs = puertsRequire('csharp')

const watcher = new class Watcher {
    
    cache: string[] = []
    
    press(filename: string) {
        //打印出事件和相关文件
        let file = `${ getTsProjectPath() }/Examples/src/${ filename }`.replace(/\\/g, '/');
        let compilerOptions = getCompilerOptions();
        let oldDir = compilerOptions.outDir;
        compilerOptions.outDir = path.resolve(compilerOptions.outDir, 'Examples/src', path.dirname(filename));
        //delete compilerOptions.outDir;
        let outFile = filename.replace(/\.ts$/g, '.js');
        watch(global.$needCompile = [ file ], global.$compilerOptions = compilerOptions);
        let src = path.resolve(compilerOptions.outDir, outFile);
        //`${ getTsProjectPath() }/Scripts/tsc/dist~/Examples/src/${ filename.replace(/\.ts/g,
        // '.js') }`.replace(/\\/g, '/');
        fs.writeFileSync(src.replace('/dist~/', '/bundle/') + '.txt', fs.readFileSync(src));
        fs.writeFileSync(src.replace('/dist~/', '/bundle/') + '.map.txt', fs.readFileSync(src + '.map'));
        //fs.renameSync(src, src.replace(/\.js/g, '.cjs'))
        //fs.renameSync(src + '.map', src.replace(/\.js/g, '.cjs'))
        //$this.files.push(path);
    }
    
    constructor() {
        
        // setTimeout(() => {
        //     watch([ `${ getTsProjectPath() }/Examples/src/QuickStart.ts` ], getCompilerOptions());
        // }, 16);
    }
    
    begin() {
        let $this = this;
        let JsMain = cs.Runtime.JsMain;
        let log = cs.UnityEngine.Debug.Log;
        
        let root = `${ getTsProjectPath() }/Examples/src`;
        log('puerts hot reload: ' + root);
        
        let list = JsMain.getTsFiles(root);
        let files: string[] = JSON.parse(list);
        files.forEach(name => {
            console.log(name);
            if ($this.cache.indexOf(name) == -1) {
                $this.cache.push(name);
                $this.press(name);
            }
        });
        
        fs.watch(root, {
            persistent: true, //persistent为true表示持续监视
            recursive: true,
        }, function(event, filename) {
            console.log(event, filename);
            if (filename.match(/\.ts$/g) && $this.cache.indexOf(filename) == -1) {
                $this.cache.push(filename);
                $this.press(filename);
            }
        });
    }
    
    private debuggers: { [key: number]: Debugger } = {};
    
    addDebugger(debuggerPort: number) {
        this.debuggers[debuggerPort] = new Debugger({
            checkOnStartup: true, trace: true,
        });
        this.debuggers[debuggerPort].open('127.0.0.1', debuggerPort);
    }
    
    removeDebugger(debuggerPort: number) {
        if (this.debuggers[debuggerPort]) {
            this.debuggers[debuggerPort].close();
            delete this.debuggers[debuggerPort];
        }
    }
    
    emitFileChanged(filepath: string) {
        Object.keys(this.debuggers).forEach(key => {
            
            try {
                console.log(`文件更新[${ key }]: ${ filepath }`);
                this.debuggers[key].update(filepath);
            } catch (e) {
                console.error(e.stack)
            }
        })
    }
}

watcher.begin();

function watch(rootFileNames: string[], options: ts.CompilerOptions) {
    const files: ts.MapLike<{ version: number }> = {};
    
    // initialize the list of files
    rootFileNames.forEach(fileName => {
        files[fileName] = { version: 0 };
    });
    
    // Create the language service host to allow the LS to communicate with the host
    const servicesHost: ts.LanguageServiceHost = {
        getScriptFileNames: () => rootFileNames,
        getScriptVersion: fileName => files[fileName] && files[fileName].version.toString(),
        getScriptSnapshot: fileName => {
            if (!fs.existsSync(fileName)) {
                return undefined;
            }
            
            return ts.ScriptSnapshot.fromString(fs.readFileSync(fileName).toString());
        },
        getCurrentDirectory: () => process.cwd(),
        getCompilationSettings: () => options,
        getDefaultLibFileName: options => ts.getDefaultLibFilePath(options),
        fileExists: ts.sys.fileExists,
        readFile: ts.sys.readFile,
        readDirectory: ts.sys.readDirectory,
        directoryExists: ts.sys.directoryExists,
        getDirectories: ts.sys.getDirectories,
    };
    
    // Create the language service files
    const services = ts.createLanguageService(servicesHost, ts.createDocumentRegistry());
    
    // Now let's watch the files
    rootFileNames.forEach(fileName => {
        // First time around, emit all files
        emitFile(fileName);
        
        // Add a watch on the file to handle next change
        fs.watchFile(fileName, { persistent: true, interval: 150 }, (curr, prev) => {
            // Check timestamp
            if (+curr.mtime <= +prev.mtime) {
                return;
            }
            
            // Update the version to signal a change in the file
            files[fileName].version++;
            
            // write the changes to disk
            emitFile(fileName);
        });
    });
    
    function emitFile(fileName: string) {
        let output = services.getEmitOutput(fileName);
        
        if (!output.emitSkipped) {
            console.log('compiled ts:', fileName);
        } else {
            logErrors(fileName);
        }
        
        output.outputFiles.forEach(o => {
            mkdirp.sync(path.dirname(o.name));
            fs.writeFileSync(o.name, o.text, 'utf8');
            watcher.emitFileChanged(o.name);
        });
    }
    
    function logErrors(fileName: string) {
        let allDiagnostics = services
        .getCompilerOptionsDiagnostics()
        .concat(services.getSyntacticDiagnostics(fileName))
        .concat(services.getSemanticDiagnostics(fileName));
        
        allDiagnostics.forEach(diagnostic => {
            let message = ts.flattenDiagnosticMessageText(diagnostic.messageText, '\n');
            if (diagnostic.file) {
                let { line, character } = diagnostic.file.getLineAndCharacterOfPosition(diagnostic.start!);
                console.log(
                    `  Error ${ diagnostic.file.fileName } (${ line + 1 },${ character + 1 }): ${ message }`);
            } else {
                console.log(`  Error: ${ message }`);
            }
        });
    }
}

// Start the watcher
export default watcher;