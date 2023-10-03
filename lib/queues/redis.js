const Bull = require("bull");
const {Email} = require("../models");
const mail = require('../mail');
const txQueue = new Bull('transactional');

exports.enqueue = async function(slug, to){
  const {message, send_at} = await Email.prepareMessage(slug, to);
  
  //need to figure out how to tell Bull to process something
  //at a given time
  txQueue.process(async (job) => {
    //hand it off to nodemailer immediately
    return null;
    //return doSomething(job.data);
  });

}


txQueue.on('completed', (job, result) => {
  //log it so we know what happened. Let's use an external service for this
  console.log(`Job completed with result ${result}`);
});

txQueue.on('failed', (job, result) => {
  //log it so we know what happened. Let's use an external service for this
  console.log(`Job completed with result ${result}`);
});
