require("dotenv").config();
const { Sequelize } = require('sequelize');

//default to SQLite, but you can use Postgres if you like
const sequelize = new Sequelize(process.env.DATABASE_URL, {
  dialect: 'postgres',
  logging: false,
  schema: "mail"
});

//utility for testing, etc
exports.DB = {
  close(){
    console.log("Closing down...");
    sequelize.close();
  },
  async sync(){
    console.log("Syncing...");
    await this.run("create schema if not exists mail");
    await sequelize.sync({
      force: true
    });
    return "DONE"
  },
  async run(sql, opts={}){
    return sequelize.query(sql,opts);
  },
  async query(sql){
    return sequelize.query(sql,{type: sequelize.QueryTypes.SELECT});
  }
}
const Broadcast = require("./broadcast").init(sequelize);
const Email = require("./email").init(sequelize);
const Message = require("./message").init(sequelize);
const Member = require("./member").init(sequelize);
const Sequence = require("./sequence").init(sequelize);
const Tag = require("./tag").init(sequelize);
const Outbox = require("./outbox").init(sequelize);

//associations
Broadcast.hasOne(Email, {onDelete: "CASCADE"});


Message.hasOne(Outbox);
Outbox.belongsTo(Message);

Member.belongsToMany(Tag, {through: "member_tags"});
Tag.belongsToMany(Member, {through: "member_tags"});

Sequence.hasMany(Email, {onDelete: "CASCADE"});

exports.Broadcast = Broadcast;
exports.Email = Email;
exports.Member = Member;
exports.Message = Message;
exports.Sequence = Sequence;
exports.Tag = Tag;
exports.Outbox = Outbox;
