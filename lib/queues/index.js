const Bull = require("bull");

//so far I'm thinking 3 queues:
//the first will be "send it right now dammit" for transactional stuff. Anything that's put in it, will get sent when the job is processed by the handler.
const txQueue = new Bull('transactional');

txQueue.process(async (job) => {
  //hand it off to nodemailer immediately
  return null;
  //return doSomething(job.data);
});

txQueue.on('completed', (job, result) => {
  //log it so we know what happened. Let's use an external service for this
  console.log(`Job completed with result ${result}`);
});
txQueue.on('failed', (job, result) => {
  //log it so we know what happened. Let's use an external service for this
  console.log(`Job completed with result ${result}`);
});