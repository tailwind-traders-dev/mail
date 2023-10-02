require("dotenv").config();
const {DB } = require("../../lib/models");
before(async () => {

  await DB.sync();
});

after(async () => {
  //await PG.close();
  DB.close();
})
