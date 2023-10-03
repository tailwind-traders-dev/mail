//just need a few methods in here...
const {Email} = require("../models");
const mail = require('../mail');

exports.queue = async function(slug, to){
  const {message, send_at} = await Email.prepareMessage(slug, to);
  
  //queue it up!
}

exports.process = async function(){
  //pull the messages in the queue based on the send_at timestamp (epoch)
  //and send them off...
  
  //await mail.send(message)
}