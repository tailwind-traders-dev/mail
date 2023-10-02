require("dotenv").config();
const {DB} = require("./lib/models");

const go = async function(){
  //this is lame... it should create the schema...
  await DB.run("create schema if not exists mail");
  await DB.sync();
};
go().then(console.log);