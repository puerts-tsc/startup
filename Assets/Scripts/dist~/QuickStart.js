"use strict";
//部署:npm run build
var __importDefault = (this && this.__importDefault) || function (mod) {
    return (mod && mod.__esModule) ? mod : { "default": mod };
};
var csharp_1 = require("csharp");
var extensions_1 = __importDefault(require("gen/extensions"));
var puerts_1 = require("puerts");
require("./ExtensionDecl");
let react = require('react');
let metadata = require('reflect-metadata');
let reconciler = require('react-reconciler');
let testRenderer = require('react-test-renderer');
(0, extensions_1.default)();
module.exports = {
    $log: global.$log = function (msg) {
        console.log('[test 0x25611]', msg);
    },
    default: function () {
        global.$log('hello, puerts');
        //静态函数
        csharp_1.UnityEngine.Debug.Log('hello world');
        //对象构造
        let obj = new csharp_1.PuertsTest.DerivedClass();
        //实例成员访问
        obj.BMFunc(); //父类方法
        obj.DMFunc(csharp_1.PuertsTest.MyEnum.E1); //子类方法
        console.log(obj.BMF, obj.DMF);
        obj.BMF = 10; //父类属性
        obj.DMF = 30; //子类属性
        console.log(obj.BMF, obj.DMF);
        //静态成员
        console.log(csharp_1.PuertsTest.BaseClass.BSF, csharp_1.PuertsTest.DerivedClass.DSF, csharp_1.PuertsTest.DerivedClass.BSF);
        //委托，事件
        //如果你后续不需要-=，可以像这样直接传函数当delegate
        obj.MyCallback = msg => console.log('do not need remove, msg=' + msg);
        //通过new构建的delegate，后续可以拿这个引用去-=
        let delegate = new csharp_1.PuertsTest.MyCallback(msg => console.log('can be removed, msg=' + msg));
        //由于ts不支持操作符重载，Delegate.Combine相当于C#里头的obj.myCallback += delegate;
        obj.MyCallback = csharp_1.System.Delegate.Combine(obj.MyCallback, delegate);
        obj.Trigger();
        //Delegate.Remove相当于C#里头的obj.myCallback -= delegate;
        obj.MyCallback = csharp_1.System.Delegate.Remove(obj.MyCallback, delegate);
        obj.Trigger();
        //事件
        obj.add_MyEvent(delegate);
        obj.Trigger();
        obj.remove_MyEvent(delegate);
        obj.Trigger();
        //静态事件
        csharp_1.PuertsTest.DerivedClass.add_MyStaticEvent(delegate);
        obj.Trigger();
        csharp_1.PuertsTest.DerivedClass.remove_MyStaticEvent(delegate);
        obj.Trigger();
        //可变参数
        obj.ParamsFunc(1024, 'haha', 'hehe', 'heihei');
        //in out 参数
        let p1 = (0, puerts_1.$ref)(1);
        let p2 = (0, puerts_1.$ref)(10);
        let ret = obj.InOutArgFunc(100, p1, p2);
        console.log('ret=' + ret + ', out=' + (0, puerts_1.$unref)(p1) + ', ref=' + (0, puerts_1.$unref)(p2));
        //泛型
        //先通过$generic实例化泛型参数
        let List = (0, puerts_1.$generic)(csharp_1.System.Collections.Generic.List$1, csharp_1.System.Int32); //$generic调用性能不会太好，同样泛型参数建议整个工程，至少一个文件内只做一次
        let Dictionary = (0, puerts_1.$generic)(csharp_1.System.Collections.Generic.Dictionary$2, csharp_1.System.String, List);
        let lst = new List();
        lst.Add(1);
        lst.Add(0);
        lst.Add(2);
        lst.Add(4);
        obj.PrintList(lst);
        let dic = new Dictionary();
        dic.Add('aaa', lst);
        obj.PrintList(dic.get_Item('aaa'));
        //arraybuffer
        let ab = obj.GetAb(5);
        let u8a0 = new Uint8Array(ab);
        console.log(obj.SumOfAb(u8a0));
        let u8a1 = new Uint8Array(2);
        u8a1[0] = 123;
        u8a1[1] = 101;
        console.log(obj.SumOfAb(u8a1));
        //引擎api
        const testGameObject = false;
        if (testGameObject) {
            let go = new csharp_1.UnityEngine.GameObject('testObject');
            go.AddComponent((0, puerts_1.$typeof)(csharp_1.UnityEngine.ParticleSystem));
            go.transform.position = new csharp_1.UnityEngine.Vector3(7, 8, 9);
            obj.Extension2(go);
        }
        //extension methods
        obj.PlainExtension();
        obj.Extension1();
        let obj1 = new csharp_1.PuertsTest.BaseClass1();
        obj.Extension2(obj1);
        // //typescript和c#的async，await联动，为了不在低版本的Unity下报错，先注释，c#7.3以上版本可以打开这些注释
        // async function asyncCall() {
        //     let task = obj.GetFileLength('Assets/Examples/05_Typescript/TsQuickStart.cs');
        //     let result = await $promise(task);
        //     console.log('file length is ' + result);
        //     let task2 = obj.GetFileLength('notexistedfile');//这个会抛文件找不到异常，被catch
        //     let result2 = await $promise(task2);
        //     console.log('file length is ,' + result2);
        // }
        //
        // asyncCall().catch(e => console.log('catch: ' + e));
    }
};
//# sourceMappingURL=QuickStart.js.map