const assert = require("assert");
const nodemailer = require("nodemailer");
const ejs = require("ejs");
var md = require('markdown-it')();

const {Email, Message} = require("./models");

const fakeTsp = nodemailer.createTransport({
  host: 'smtp.ethereal.email',
  port: 587,
  auth: {
      user: 'joel.heller93@ethereal.email',
      pass: 'ZAQU7RPMnmcRTeHP4P'
  }
});

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

exports.prepare = async function(slug, to, data={}){
  const email = await Email.findOne({where: {slug: slug}});
  assert(email, `An email with slug ${slug} doesn't exist`);
  let renderOne;
  renderOne =  ejs.render(email.markdown, data);
  //try{
  //mattered = gm(renderOne);

  //assert(mattered.attributes.subject, "Can't send an email without a subject");
  //const layoutHtml = fs.readFileSync(this.layout,"utf-8");
  
  const html = md.render(renderOne);
  return new Message({
    to,
    from: process.env.DEFAULT_FROM,
    subject: email.subject,
    html,
  })
}

exports.send = async function(message){
  
  if(process.env.NODE_ENV !== "test"){
    console.log("Sending",message.subject,"to",message.to);
    message.receipt = await tsp.sendMail(this);
  }else{
    //this.response = await fakeTsp.sendMail(this);
    message.receipt = {messageId: 1} //use this if you don't care, otherwise test against ethereal
  }
  message.sent_at = new Date().toISOString();
  await message.save();
  return this;
}
