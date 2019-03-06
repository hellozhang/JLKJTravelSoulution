var redis = require("redis");
var redisport = 6379;
var redishost = "192.168.30.176"
const client = redis.createClient(redisport, redishost);

client.publish('TestSub', "我来自外太空");