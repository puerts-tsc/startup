"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var puerts_1 = require("puerts");
var csharp_1 = require("csharp");
//要手动声明一下，否则通过obj.func引用扩展方法会失败
(0, puerts_1.$extension)(csharp_1.PuertsTest.BaseClass, csharp_1.PuertsTest.BaseClassExtension);
//# sourceMappingURL=ExtensionDecl.js.map