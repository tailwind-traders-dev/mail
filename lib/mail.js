
const assert = require("assert");
const nodemailer = require("nodemailer");
const ejs = require("ejs");
var md = require('markdown-it')();
const {Op} = require("sequelize");

const {Email, Message, Outbox} = require("./models");

const fakeTsp = nodemailer.createTransport({
  host: 'smtp.ethereal.email',
  port: 587,
  auth: {
      user: process.env.ETHEREAL_USER,
      pass: process.env.ETHEREAL_PW
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

//gets all the mail in the outbox with a send_at <= now
exports.process = async function(){
  const now= new Date().getTime();
  const ready = await Message.findAll({}, {
    include: {
      model: Outbox,
      where: {
        send_at : {
          [Op.lte]: now
        } 
      }
    }
  });

  const out = [];
  for(let message of ready){
    const sent = await this.send(message);
    //delete the Outbox if it's sent
    await Outbox.destroy({where: {messageId: message.id}})
    out.push(sent)
  }
  return out;
}

//this method creates a message and queues it in the outbox
//to be processed by a job later on
exports.queue = async function(slug, to, data={}){
  const email = await Email.findOne({where: {slug: slug}});
  assert(email, `An email with slug ${slug} doesn't exist`);
  let renderOne;
  renderOne =  ejs.render(email.markdown, data);
  //try{
  //mattered = gm(renderOne);

  //assert(mattered.attributes.subject, "Can't send an email without a subject");
  //const layoutHtml = fs.readFileSync(this.layout,"utf-8");
  
  const html = md.render(renderOne);
  const message = await Message.create({
    to,
    from: process.env.DEFAULT_FROM,
    subject: email.subject,
    html,
  });

  //if there's a delay, calculate the send
  const send_at = new Date().getTime() //epoch timestamp
  if(email.delay_hours > 0){
    send_at += (email.delay_hours + (1000 * 60 * 60))
  }
  console.log(send_at);
  const out = await Outbox.create({
    messageId: message.id,
    send_at
  });

  
  return message;
}

exports.send = async function(message){
  
  if(process.env.NODE_ENV !== "test"){
    console.log("Sending",message.subject,"to",message.to);
    message.receipt = await tsp.sendMail(message);
  }else if(process.env.NODE_ENV === "integration"){
    message.receipt = await fakeTsp.sendMail(message);
  }else{
    message.receipt = {messageId: 1} //use this if you don't care, otherwise test against ethereal
  }
  message.sent_at = new Date().toISOString();
  console.log(message);
  await message.save();
  return message;
}
