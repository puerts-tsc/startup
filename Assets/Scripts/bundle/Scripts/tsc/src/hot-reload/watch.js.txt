let CS = global.CS = require('csharp');
let HotReloadWatcher = global.HotReloadWatcher || {};
let $root = global.$root || {};
let $config = global.$config || {};
process.on('uncaughtException', function (e) { console.error('uncaughtException', e); });
try {
    const moduleRequire = require('module').createRequire($root);
    moduleRequire('ts-node').register($config);
    global.HotReloadWatcher = moduleRequire('./Scripts/tsc/src/watch.ts').default;
    const jsEnvs = CS.Puerts.JsEnv.jsEnvs;
    console.log('jsEnvs.Count:' + jsEnvs.Count);
    for (let i = 0; i < jsEnvs.Count; i++) {
        const item = jsEnvs.get_Item(i);
        if (item && item.debugPort != -1) {
            HotReloadWatcher.addDebugger(item.debugPort);
        }
    }
    CS.NodeTSCAndHotReload.addDebugger = HotReloadWatcher.addDebugger.bind(HotReloadWatcher);
    CS.NodeTSCAndHotReload.removeDebugger = HotReloadWatcher.removeDebugger.bind(HotReloadWatcher);
    true;
}
catch (e) {
    console.error(e.stack);
    console.error('Some error triggered. Maybe you should run `npm i` in project directory');
    false;
}
//# sourceMappingURL=watch.js.map