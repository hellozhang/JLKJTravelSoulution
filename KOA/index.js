const koa = require('koa')
const app = new koa();

app.use(async (ctx) => {

    ctx.body = "ZZZZZZZZZZZZZ"
});
app.listen(3000);
console.log('ggg');