import { Demo } from './test'

const data = new Demo(1, 2);
console.log(data);




let anyData: any;
anyData = 1;
anyData = false;
anyData = [];
anyData = {};

let t1: string = "";
let t2: string = `{t1}-------New`

console.log(t2);

//缺省参数
function func(x: number, y?: number): number {
    if (y) {
        return x + y;
    }
    return x;
}
//默认值
function func2(x: number, y = 1): number {
    if (y) {
        return x + y;
    }
    return x;
}
// 不确认参数  ...


function func3(x: number, ...inputParam: string[]): string {
    return 1000 + inputParam.join('_')
}

let t3 = func3(10, 'AA', 'BB', 'CC');

//传入对象
function func4(x: { name: any, age: number }): string {
    return x.name;
}

console.log(func4({ name: '我是对象参数', age: 10 }));