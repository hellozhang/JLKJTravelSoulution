function Test() {

    console.log('NEW' + new Date());
    return 10;
}

function promisefn() {
    return new Promise((res) => {

        setTimeout(() => {
            res("long_time_value")

        }, 30000);

    });

}

async function asyn() {

    var res = await Test();
    var res2 = await promisefn();
    console.log(res, res2);

}
var s = asyn();