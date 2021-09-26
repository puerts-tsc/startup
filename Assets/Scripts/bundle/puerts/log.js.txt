/*
 * Tencent is pleased to support the open source community by making Puerts available.
 * Copyright (C) 2020 THL A29 Limited, a Tencent company.  All rights reserved.
 * Puerts is licensed under the BSD 3-Clause License, except for the third-party components listed in the file 'LICENSE' which may be subject to their corresponding license terms. 
 * This file is subject to the terms and conditions defined in file 'LICENSE', which is part of this source code package.
 */

var global = global || (function () {
    return this;
}());
(function (global) {
    "use strict";
    
    let UnityEngine_Debug = puerts.loadType('UnityEngine.Debug');
    let Strings = puerts.loadType('GameEngine.Extensions.Strings');
    
    String.prototype['toBlue'] = function(...args) {
        return Strings.ToBlue(Array.prototype.map.call(args, x => {
            try {
                return x + '';
            } catch (err) {
                return err;
            }
        }).join(', ') + this);
    }
    
    String.prototype['toYellow'] = function(...args) {
        return Strings.ToYellow(Array.prototype.map.call(args, x => {
            try {
                return x + '';
            } catch (err) {
                return err;
            }
        }).join(', ') + this);
    }
    String.prototype['toGreen'] = function(...args) {
        return Strings.ToGreen(Array.prototype.map.call(args, x => {
            try {
                return x + '';
            } catch (err) {
                return err;
            }
        }).join(', ') + this);
    }
    String.prototype['toRed'] = function(...args) {
        return Strings.ToRed(Array.prototype.map.call(args, x => {
            try {
                return x + '';
            } catch (err) {
                return err;
            }
        }).join(', ') + this);
    }
    
    if (!UnityEngine_Debug) return;
    
    const console_org = global.console;
    var console = {}
    
    function toString(args, fn) {
        let error = new Error("");
        let path = puerts.loadType('UnityEngine.Application').dataPath.replace("/Assets", "")
        //UnityEngine_Debug.Log(path);
        error = error.stack.replace(error.message, '').replace(/\\/g, '/');
        error = error.replace(new RegExp(path+"/", "g"), '');
        error = error.replace(/((Packages|Assets)\/.+?):(\d+)(:\d+)/g, " <a href=\"$1\" line=\"$3\"><color=yellow>$1:$3</color></a> ");
        error = error.split("\n");
        
//        delete error[0]
//        delete error[1]
//        delete error[2]
        
        // error.forEach((s,i) => {
        //    
        //     let t = /^(.*)(F\:.+)(\:\d+)(\:\d+)(.*)$/g.exec(s);
        //     if(t !=null){
        //         // let ln = t[1].split(":")
        //         error[i]= `<color=red>${t[0]}<a href="${t[1]}" line="${t[2]}">${t[1]}${t[2]}${t[3]}</a>${t[4]}</color>`;
        //     }
        // })
        const reg = /\n(\n)*( )*(\n)*\n/g;
        fn = fn || (s => s);
        return fn(Array.prototype.map.call(args, x => {
            try {
                return x + '';
            } catch (err) {
                return err;
            }
        }).join(', ')) + error.join("\n").replace(reg, "\n");
    }
    
    console.log = function () {
        if (console_org) console_org.log.apply(null, Array.prototype.slice.call(arguments));
        UnityEngine_Debug.Log(toString(arguments));
    }
    
    console.log.blue = function () {
        if (console_org) console_org.log.apply(null, Array.prototype.slice.call(arguments));
        UnityEngine_Debug.Log(toString(arguments, Strings.ToBlue));
    }
    
    console.log.red = function () {
        if (console_org) console_org.log.apply(null, Array.prototype.slice.call(arguments));
        UnityEngine_Debug.Log(toString(arguments, Strings.ToRed));
    }
    
    console.log.yellow = function () {
        if (console_org) console_org.log.apply(null, Array.prototype.slice.call(arguments));
        UnityEngine_Debug.Log(toString(arguments, Strings.ToYellow));
    }
    
    console.log.green = function () {
        if (console_org) console_org.log.apply(null, Array.prototype.slice.call(arguments));
        UnityEngine_Debug.Log(toString(arguments, Strings.ToGreen));
    }
    
    console.info = function () {
        if (console_org) console_org.info.apply(null, Array.prototype.slice.call(arguments));
        UnityEngine_Debug.Log(toString(arguments));
    }
    
    console.warn = function () {
        if (console_org) console_org.warn.apply(null, Array.prototype.slice.call(arguments));
        UnityEngine_Debug.LogWarning(toString(arguments));
    }
    
    console.error = function () {
        if (console_org) console_org.error.apply(null, Array.prototype.slice.call(arguments));
        UnityEngine_Debug.LogError(toString(arguments));
    }
    
    global.console = console;
    puerts.console = console;
    
    //add
    global.log = console.log;
    // global.log.blue = console.log.blue;
    // global.log.red = console.log.red;
    // global.log.yellow = console.log.yellow;
    // global.log.green = console.log.green;
    puerts.log = console.log;
    
}(global));
