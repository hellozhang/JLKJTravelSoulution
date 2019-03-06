var redis = require("redis");
var redisport = 6379;
var redishost = "192.168.30.176"
const client = redis.createClient(redisport, redishost);
client.set("NodeJs", new Date());
client.set("NodeJson", "{'name';'hellozhang'}")
client.del("NodeList");
client.rpush('NodeList', 'A');
client.lpush('NodeList', '==');


client.get("NodeJs", (er, v) => {

    console.log("NodeJsVAL", er, v);
});
client.get("NodeJson", (er, v) => {
    console.warn(v)
});















Object.prototype.toString = function () {
    return JSON.stringify(this);
};