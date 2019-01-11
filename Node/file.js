var fs = require("http");
fs.readFile('./a.js',"utf-8",function(err,data)
{

    console.log(err,data);
})
