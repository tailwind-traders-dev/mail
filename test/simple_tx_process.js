
const {DB, Email} = require("../lib/models");
const Mail = require("../lib/mail");

const assert = require("assert");

//The process here is the simplest one: we have an Email we want to send to a 
//customer. We need the email template

describe('A simple transactional email message', () => { 
  let email=null, message=null;
  before(async function(){
    email = await Email.create({
      slug: "test",
      subject: "Testing Blah",
      markdown: `#Testing!
Exciting to be testing stuff.
      `
    });
    message = await Mail.prepare("test", "test@test.com");
    console.log(message);
  });  
  it("it will work", async function(){
    assert(email.slug)
  });
});

