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
const ts = __importStar(require("typescript"));
const util_1 = require("lib/util");
const CS = puertsRequire('csharp');
function compile(fileNames, options) {
    CS.UnityEditor.EditorUtility.DisplayProgressBar('compile ts', 'compiling typescript', 0);
    let program = ts.createProgram(fileNames, options);
    let emitResult = program.emit();
    let allDiagnostics = ts
        .getPreEmitDiagnostics(program)
        .concat(emitResult.diagnostics);
    let n = 0;
    allDiagnostics.forEach(diagnostic => {
        CS.UnityEditor.EditorUtility.DisplayProgressBar('compile ts', `compiling ${diagnostic.file}`, (n += 1) / fileNames.length);
        if (diagnostic.file) {
            let { line, character } = ts.getLineAndCharacterOfPosition(diagnostic.file, diagnostic.start);
            let message = ts.flattenDiagnosticMessageText(diagnostic.messageText, '\n');
            console.log(`${diagnostic.file.fileName} (${line + 1},${character + 1}): ${message}`);
        }
        else {
            console.log(ts.flattenDiagnosticMessageText(diagnostic.messageText, '\n'));
        }
    });
    CS.UnityEditor.EditorUtility.ClearProgressBar();
}
/*[ `${ getTsProjectPath() }/src/QuickStart.ts` ]*/
compile([
    //`${ getTsProjectPath() }/src/QuickStart.ts` 
    `${CS.UnityEngine.Application.dataPath}/Examples/src/QuickStart.ts`
], (0, util_1.getCompilerOptions)());
//# sourceMappingURL=compile.js.map