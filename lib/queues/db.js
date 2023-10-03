const {Email, Outbox, Message} = require("../models");
const {Op} = require("sequelize");
const mail = require("../mail");

//this method creates a message and queues it in the outbox
//to be processed by a job later on
exports.queue = async function(slug, to){

  const {message, send_at} = await Email.prepareMessage(slug, to);

  await Outbox.create({
    messageId: message.id,
    send_at
  });

  return message;
}

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
    const sent = await mail.send(message);
    //delete the Outbox if it's sent
    await Outbox.destroy({where: {messageId: message.id}})
    out.push(sent)
  }
  return out;
}