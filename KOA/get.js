const koa = require("koa");
var app = new koa();
app.use(async (ctx) => {

    let url = ctx.url;
    let req = ctx.request;
    let req_query = req.query;
    let req_query_str = req.querystring;
    let ctx_req_query = ctx.query;
    let ctx_req_query_str = ctx.querystring;
    let method = ctx.method;
    if (ctx.method == "GET" && ctx.url == "/") {
        ctx.body = "GET";
    }



    ctx.body = {
        url,
        req_query,
        req_query_str,
        ctx_req_query,
        ctx_req_query_str,
        method
    }

});


app.listen(3000, () => {

    console.log("koa req")
});