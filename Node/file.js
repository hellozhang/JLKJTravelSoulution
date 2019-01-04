import fs from "fs";
fs.readFile('./a.js',"utf-8",function(err,data)
{

    console.log(err,data);
})
