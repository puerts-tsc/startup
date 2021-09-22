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
exports.State = exports.Debugger = void 0;
//@ts-ignore
const fs = __importStar(require("fs"));
const path = __importStar(require("path"));
const events_1 = require("events");
const os = __importStar(require("os"));
const CDPFunc = require("chrome-remote-interface");
const MAX_SCRIPTS_CACHE_SIZE = 10000000;
let example;
class Debugger extends events_1.EventEmitter {
    get state() { return this._state; }
    get isOpend() { return this._state === State.Open; }
    _trace;
    _ignorePathCase;
    _checkOnStartup;
    _state = State.None;
    _client;
    _debugger;
    //cache script infos
    _scriptParsed;
    _scriptFailedToParse;
    _locks;
    constructor(params) {
        super();
        let { trace, ignorePathCase, checkOnStartup } = params ?? {};
        this._trace = trace ?? true;
        this._ignorePathCase = ignorePathCase ?? process.platform === "win32";
        this._checkOnStartup = checkOnStartup ?? true;
    }
    async open(host, port, local) {
        console.log('open1');
        if (this._state === State.Connecting || this._state === State.Open)
            throw new Error("socket is opening");
        this.close();
        if (typeof local === "undefined")
            local = true;
        console.log('open2');
        try {
            this._state = State.Connecting;
            this._client = await CDPFunc({ host, port, local });
            console.log('open3');
            const { Runtime, Debugger } = this._client;
            this._debugger = Debugger;
            Debugger.on("scriptParsed", this._scriptParsedHanlder);
            Debugger.on("scriptFailedToParse", this._scriptFailedToParseHandler);
            await Runtime.enable();
            console.log('open4');
            await Debugger.enable({ "maxScriptsCacheSize": MAX_SCRIPTS_CACHE_SIZE });
            console.log('open5');
            this._client.on("disconnect", this._disconnectHandler);
            this._state = State.Open;
        }
        catch (err) {
            console.error(err);
            this.close();
            return "CONNECT_FAIL: " + err;
        }
        return undefined;
    }
    async close() {
        this._state = State.Close;
        this._scriptParsed = undefined;
        this._scriptFailedToParse = undefined;
        this._debugger = undefined;
        this._locks = undefined;
        if (this._client) {
            let client = this._client;
            this._client = undefined;
            await client.close();
        }
    }
    update(filepath) {
        if (this._state !== State.Open || !this._debugger)
            throw new Error("socket is close");
        if (this._trace) {
            console.log(`change: ${filepath}`);
        }
        if (!this._scriptParsed)
            return;
        filepath = path.normalize(filepath).replace(/\\/g, "/");
        ;
        if (this._ignorePathCase)
            filepath = filepath.toLowerCase();
        this._pushUpdate(filepath);
    }
    async _pushUpdate(filepath) {
        console.log('update debugger:', filepath);
        let scriptId = this._scriptParsed[filepath] || this._scriptParsed[(os.platform() == 'win32' ? "file:/" : "file:") + filepath];
        if (scriptId && fs.existsSync(filepath) && fs.lstatSync(filepath).isFile()) {
            let scriptSource = fs.readFileSync(filepath).toString("utf-8");
            scriptSource = ("(function (exports, require, module, __filename, __dirname) { " + scriptSource + "\n});");
            let lock = await this._lock(scriptId);
            if (!this._debugger)
                return;
            if (this._trace)
                console.log(`check: \t${scriptId}:${filepath}`);
            let exist = await this._debugger.getScriptSource({ scriptId });
            if (!exist || exist.scriptSource === scriptSource || !this._debugger)
                return;
            if (this._trace)
                console.log(`send: \t${scriptId}:${filepath}`);
            let response = await this._debugger.setScriptSource({ scriptId, scriptSource });
            if (this._trace) {
                console.log(`completed: \t${scriptId}:${filepath}` /** + `| \t${JSON.stringify(response)}` */);
            }
            lock.release();
        }
    }
    _scriptParsedHanlder = (params) => {
        if (!params || !params.url || !params.scriptId)
            return;
        let scriptId = params.scriptId;
        let filepath = path.normalize(params.url).replace(/\\/g, "/");
        ;
        if (this._ignorePathCase)
            filepath = filepath.toLowerCase();
        if (this._trace) {
            console.log(`scriptParsed: ${scriptId}:${filepath}`);
        }
        if (!this._scriptParsed)
            this._scriptParsed = {};
        this._scriptParsed[filepath] = scriptId;
        this._scriptParsed[scriptId] = filepath;
        if (this._checkOnStartup)
            this._pushUpdate(filepath);
    };
    _scriptFailedToParseHandler = (params) => {
        if (!params || !params.url || !params.scriptId)
            return;
        let scriptId = params.scriptId;
        let url = path.normalize(params.url).replace(/\\/g, "/");
        ;
        if (this._ignorePathCase)
            url = url.toLowerCase();
        if (this._trace) {
            console.log(`scriptFailedToParse: ${scriptId}:${url}`);
        }
        if (!this._scriptFailedToParse)
            this._scriptFailedToParse = {};
        this._scriptFailedToParse[url] = scriptId;
        this._scriptFailedToParse[scriptId] = url;
    };
    _disconnectHandler = () => {
        this.close();
        this.emit("disconnect");
    };
    async _lock(key) {
        if (!this._locks)
            this._locks = new Map();
        let lock = this._locks.get(key);
        if (!lock) {
            lock = new Lock();
            this._locks.set(key, lock);
        }
        return lock.acquire(0);
    }
}
exports.Debugger = Debugger;
var State;
(function (State) {
    State[State["None"] = 0] = "None";
    State[State["Connecting"] = 1] = "Connecting";
    State[State["Open"] = 2] = "Open";
    State[State["Close"] = 3] = "Close";
})(State = exports.State || (exports.State = {}));
class Lock {
    get isLocked() { return this._isLocked; }
    _isLocked; //是否正在锁定中
    _handlers; //异步等待回调 
    _handler;
    async acquire(priority = 0) {
        if (priority && priority > 0) {
            await new Promise(function (resolve) {
                setTimeout(resolve, priority);
            });
        }
        await this.acquireLock(priority ?? 0);
        this._handler = {
            release: () => {
                this._isLocked = false;
                this._handler = undefined;
                this.moveNext();
            }
        };
        return this._handler;
    }
    reset() {
        this._isLocked = false;
        this._handlers = undefined;
        if (this._handler) {
            this._handler.release = function () { };
            this._handler = undefined;
        }
    }
    async acquireLock(priority) {
        while (this._isLocked) {
            await new Promise((resolve) => {
                if (!this._handlers)
                    this._handlers = [];
                this._handlers.push({ priority, resolve });
            });
        }
        this._isLocked = true;
    }
    moveNext() {
        if (!this._handlers || this._handlers.length === 0)
            return;
        this._handlers.sort((o1, o2) => o1.priority < o2.priority ? -1 : o1.priority > o2.priority ? 1 : 0);
        this._handlers.shift().resolve();
    }
}
//# sourceMappingURL=debugger.js.map