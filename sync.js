import {DB} from "./lib/models";

const go = async function(){
  await DB.sync();
  await DB.close();
  return "done"
};
go().then(console.log);