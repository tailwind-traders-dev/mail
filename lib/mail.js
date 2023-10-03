const nodemailer = require("nodemailer");

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
  await message.save();
  return message;
}
