
const { Email, Outbox} = require("../lib/models");
const {DB} = require("../lib/queues");

const assert = require("assert");

//The process here is the simplest one: we have an Email we want to send to a 
//customer. We need the email template

describe('The DB queue', () => { 
  let email=null, message=null;
  before(async function(){
    email = await Email.create({
      slug: "test",
      subject: "Testing Blah",
      markdown: `#Testing!
Exciting to be testing stuff.
      `
    });
    message = await DB.queue("test", "test@test.com");
  });  
  it("gets queued in the outbox", async function(){
    const out = await Outbox.findOne({where: {messageId: message.id}});
    assert(out)
  });
  it("is in the ready state", async function(){
    const ready = await DB.process();
    assert.strictEqual(1, ready.length);
  });
  it("the outbox is now empty", async function(){
    const count = await Outbox.count();
    assert.strictEqual(0, count)
  });
});

