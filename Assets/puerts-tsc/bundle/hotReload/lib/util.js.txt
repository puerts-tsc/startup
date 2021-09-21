"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.getTsProjectPath = exports.getCompilerOptions = void 0;
const CS = puertsRequire('csharp');
const fs = require('fs');
const path = require('path');
function getCompilerOptions() {
    const TSProjPath = getTsProjectPath();
    const compilerOptions = JSON.parse(fs.readFileSync(`${TSProjPath}/tsconfig.json`)).compilerOptions;
    if (!compilerOptions) {
        return {};
    }
    compilerOptions.outDir = path.resolve(TSProjPath, compilerOptions.outDir);
    if (compilerOptions.typeRoots) {
        compilerOptions.typeRoots = compilerOptions.typeRoots.map((item) => {
            return path.resolve(TSProjPath, item);
        });
    }
    return compilerOptions;
}
exports.getCompilerOptions = getCompilerOptions;
function getTsProjectPath() {
    return path.resolve(CS.UnityEngine.Application.dataPath, '../TsProj/');
}
exports.getTsProjectPath = getTsProjectPath;
//# sourceMappingURL=util.js.map