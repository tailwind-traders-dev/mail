import { expect, test, describe, beforeAll } from "bun:test";

const Email = require('../lib/models/email');
const assert = require("assert");

describe('The email model', () => { 
  let email=null;
  beforeAll(async function(){
    email = new Email({
      to: "test@test.com",
      subject: "Test",
      html: "HI"
    });
    await email.send();
  });  
  test("won't send because we're testing", async function(){
    expect(email.response).toBeFalsy();
  });
})