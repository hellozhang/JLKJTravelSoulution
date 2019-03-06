var koa=require("koa");
var koabodyparse=require('koa-bodyparser');
var app=new koa();
app.listen(3000,()=>{ console.log('ZZ')});
app.use(koabodyparse);

app.use(async(ctx) => {
if(ctx.method=="POST")
{

    let postdata=ctx.request.body;
    ctx.body=postdata;
}

});

