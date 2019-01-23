"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var test_1 = require("./test");
var data = new test_1.Demo(1, 2);
console.log(data);
var anyData;
anyData = 1;
anyData = false;
anyData = [];
anyData = {};
var t1 = "";
var t2 = "{t1}-------New";
console.log(t2);
//缺省参数
function func(x, y) {
    if (y) {
        return x + y;
    }
    return x;
}
//默认值
function func2(x, y) {
    if (y === void 0) { y = 1; }
    if (y) {
        return x + y;
    }
    return x;
}
// 不确认参数  ...
function func3(x) {
    var inputParam = [];
    for (var _i = 1; _i < arguments.length; _i++) {
        inputParam[_i - 1] = arguments[_i];
    }
    return 1000 + inputParam.join('_');
}
var t3 = func3(10, 'AA', 'BB', 'CC');
//传入对象
function func4(x) {
    return x.name;
}
console.log(func4({ name: 'HWC', age: 10 }));
