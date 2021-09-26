"use strict";
var __createBinding = (this && this.__createBinding) || (Object.create ? (function(o, m, k, k2) {
    if (k2 === undefined) k2 = k;
    Object.defineProperty(o, k2, { enumerable: true, get: function() { return m[k]; } });
}) : (function(o, m, k, k2) {
    if (k2 === undefined) k2 = k;
    o[k2] = m[k];
}));
var __setModuleDefault = (this && this.__setModuleDefault) || (Object.create ? (function(o, v) {
    Object.defineProperty(o, "default", { enumerable: true, value: v });
}) : function(o, v) {
    o["default"] = v;
});
var __importStar = (this && this.__importStar) || function (mod) {
    if (mod && mod.__esModule) return mod;
    var result = {};
    if (mod != null) for (var k in mod) if (k !== "default" && Object.prototype.hasOwnProperty.call(mod, k)) __createBinding(result, mod, k);
    __setModuleDefault(result, mod);
    return result;
};
Object.defineProperty(exports, "__esModule", { value: true });
const fs = __importStar(require("fs"));
const ts = __importStar(require("typescript"));
const path = __importStar(require("path"));
const mkdirp = __importStar(require("mkdirp"));
const util_1 = require("./lib/util");
const debugger_1 = require("./lib/debugger");
const cs = puertsRequire('csharp');
const watcher = new class Watcher {
    cache = [];
    press(filename) {
        //打印出事件和相关文件
        let file = `${(0, util_1.getTsProjectPath)()}/Examples/src/${filename}`.replace(/\\/g, '/');
        let compilerOptions = (0, util_1.getCompilerOptions)();
        let oldDir = compilerOptions.outDir;
        compilerOptions.outDir = path.resolve(compilerOptions.outDir, 'Examples/src', path.dirname(filename));
        //delete compilerOptions.outDir;
        let outFile = filename.replace(/\.ts$/g, '.js');
        watch(global.$needCompile = [file], global.$compilerOptions = compilerOptions);
        let src = path.resolve(compilerOptions.outDir, outFile);
        //`${ getTsProjectPath() }/Scripts/dist~/Examples/src/${ filename.replace(/\.ts/g,
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
        let root = `${(0, util_1.getTsProjectPath)()}/Examples/src`;
        log('puerts hot reload: ' + root);
        let list = JsMain.getTsFiles(root);
        let files = JSON.parse(list);
        files.forEach(name => {
            console.log(name);
            if ($this.cache.indexOf(name) == -1) {
                $this.cache.push(name);
                $this.press(name);
            }
        });
        fs.watch(root, {
            persistent: true,
            recursive: true,
        }, function (event, filename) {
            console.log(event, filename);
            if (filename.match(/\.ts$/g)) {
                let index = $this.cache.indexOf(filename);
                if (index == -1) {
                    if (fs.existsSync(root + '/' + filename)) {
                        $this.cache.push(filename);
                        $this.press(filename);
                    }
                }
                else if (!fs.existsSync(root + '/' + filename)) {
                    $this.cache.slice(index, 1);
                }
            }
        });
    }
    debuggers = {};
    addDebugger(debuggerPort) {
        this.debuggers[debuggerPort] = new debugger_1.Debugger({
            checkOnStartup: true, trace: true,
        });
        this.debuggers[debuggerPort].open('127.0.0.1', debuggerPort);
    }
    removeDebugger(debuggerPort) {
        if (this.debuggers[debuggerPort]) {
            this.debuggers[debuggerPort].close();
            delete this.debuggers[debuggerPort];
        }
    }
    emitFileChanged(filepath) {
        Object.keys(this.debuggers).forEach(key => {
            try {
                console.log(`文件更新[${key}]: ${filepath}`);
                this.debuggers[key].update(filepath);
            }
            catch (e) {
                console.error(e.stack);
            }
        });
    }
};
watcher.begin();
function watch(rootFileNames, options) {
    const files = {};
    // initialize the list of files
    rootFileNames.forEach(fileName => {
        files[fileName] = { version: 0 };
    });
    // Create the language service host to allow the LS to communicate with the host
    const servicesHost = {
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
    function emitFile(fileName) {
        let output = services.getEmitOutput(fileName);
        if (!output.emitSkipped) {
            console.log('compiled ts:', fileName);
        }
        else {
            logErrors(fileName);
        }
        output.outputFiles.forEach(o => {
            mkdirp.sync(path.dirname(o.name));
            fs.writeFileSync(o.name, o.text, 'utf8');
            watcher.emitFileChanged(o.name);
        });
    }
    function logErrors(fileName) {
        let allDiagnostics = services
            .getCompilerOptionsDiagnostics()
            .concat(services.getSyntacticDiagnostics(fileName))
            .concat(services.getSemanticDiagnostics(fileName));
        allDiagnostics.forEach(diagnostic => {
            let message = ts.flattenDiagnosticMessageText(diagnostic.messageText, '\n');
            if (diagnostic.file) {
                let { line, character } = diagnostic.file.getLineAndCharacterOfPosition(diagnostic.start);
                console.log(`  Error ${diagnostic.file.fileName} (${line + 1},${character + 1}): ${message}`);
            }
            else {
                console.log(`  Error: ${message}`);
            }
        });
    }
}
// Start the watcher
exports.default = watcher;
//# sourceMappingURL=watch.js.map