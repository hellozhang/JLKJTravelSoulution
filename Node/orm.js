var Sequelize = require("sequelize");
var connet = new Sequelize(
    'fyt_cms', 'sa', '123456', {
        host: '192.168.30.176',
        dialect: 'mysql',
        port: 3309,
        timezone: '+08:00',
        operatorsAliases: false
    });


connet.authenticate().then(() => {
    console.log('连接成功')
}).catch(err => {
    console.log('连接失败')
});




var user = connet.define("user", {
    Name: Sequelize.STRING,
    Age: Sequelize.STRING,
    BirthDay: Sequelize.DATE
}, {
    timestramp: false,
    updatedAt: "utime",
    timestamps: true,
    freezeTableName:true,
    tableName:'users'
});

var user=user.build({
    Name:'hellozhang',
    Age:20,
    BirthDay:'2018-11-23'
});
user=yield user.save();