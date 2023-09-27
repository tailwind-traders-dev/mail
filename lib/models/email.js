const assert = require("assert");
const nodemailer = require("nodemailer");

const tsp = nodemailer.createTransport({
  host: process.env.SMTP_HOST,
  port: 465,
  secure: true,
  pool: true,
  auth: {
    user: process.env.SMTP_USER,
    pass: process.env.SMTP_PASSWORD
  }
});

class Email{

  constructor(args){

    assert(args.to, "Who are we sending to? ");
    assert(args.subject, "Need a subject here");
    assert(args.html, "Need the HTML set to something... it's empty");

    Object.assign(this,args);
    this.from ||= process.env.DEFAULT_FROM;
  }

  async send(){

    if(process.env.NODE_ENV !== "test"){
      console.log("Sending",this.template,"to",this.email);
      this.response = await tsp.sendMail(this);
    }else{
      this.response = false;
    }

    this.sent_at = new Date().toISOString();
    return this;
  }
}

module.exports = Email;