import * as ts from 'typescript';
import { getCompilerOptions, getTsProjectPath } from 'lib/util'

declare const puertsRequire: any;
const CS = puertsRequire('csharp')

function compile(fileNames: string[], options: ts.CompilerOptions): void {
    
    CS.UnityEditor.EditorUtility.DisplayProgressBar('compile ts', 'compiling typescript', 0);
    let program = ts.createProgram(fileNames, options);
    let emitResult = program.emit();
    
    let allDiagnostics = ts
    .getPreEmitDiagnostics(program)
    .concat(emitResult.diagnostics);
    
    let n = 0;
    allDiagnostics.forEach(diagnostic => {
        CS.UnityEditor.EditorUtility.DisplayProgressBar('compile ts', `compiling ${ diagnostic.file }`,
            (n += 1) / fileNames.length);
        if (diagnostic.file) {
            let { line, character } = ts.getLineAndCharacterOfPosition(diagnostic.file, diagnostic.start!);
            let message = ts.flattenDiagnosticMessageText(diagnostic.messageText, '\n');
            console.log(`${ diagnostic.file.fileName } (${ line + 1 },${ character + 1 }): ${ message }`);
        } else {
            console.log(ts.flattenDiagnosticMessageText(diagnostic.messageText, '\n'));
        }
    });
    CS.UnityEditor.EditorUtility.ClearProgressBar();
}



    /*[ `${ getTsProjectPath() }/src/QuickStart.ts` ]*/

    compile([ 
        //`${ getTsProjectPath() }/src/QuickStart.ts` 
        `${CS.UnityEngine.Application.dataPath}/Examples/src/QuickStart.ts`
    ], getCompilerOptions());

