var redis = require("redis");
var redisport = 6379;
var redishost = "192.168.30.176"
const client = redis.createClient(redisport, redishost);

client.subscribe("TestSub");
client.on('message', (channel, msg) => {
    console.log("+++++++++++++++++++++++++++++");
    console.log("channel:", channel, "msg:", msg);
    console.log("+++++++++++++++++++++++++++++");
});