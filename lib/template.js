const assert = require("assert");
const ejs = require("ejs");
var md = require('markdown-it')();


const {Email,Message} = require("./models");

exports.prepare = async function(slug, data={}, {from, to}){
  const email = await Email.findOne({where: {slug: slug}});
  assert(email, `An email with slug ${slug} doesn't exist`);
  let renderOne;
  renderOne =  ejs.render(markdown, data);
  //try{
  //mattered = gm(renderOne);

  //assert(mattered.attributes.subject, "Can't send an email without a subject");
  //const layoutHtml = fs.readFileSync(this.layout,"utf-8");
  
  const html = md.render(markdown);
  return new Message({
    to,
    from: from || process.env.DEFAULT_FROM,
    subject,
    html,
  })
}

// class Template{
//   constructor(args){

//     assert(args.name, "Need a template name");
//     assert(args.email, "Need an email please");
//     assert(args.subject, "Need a subject please");

//     this.template = `${args.template}.md`;
//     this.data = args.data || null;
//     this.email = args.email;
//     this.layout = args.layout || path.resolve(__dirname, "../templates/default.ejs");
//   }
//   async render(){
//     const templateFile = path.resolve(__dirname, "templates",this.template);
//     const template = fs.readFileSync(templateFile,"utf-8");
//     //const template = await db.findOne("mail_templates", "name", this.template);
//     let renderOne, mattered;
//     renderOne =  ejs.render(template, this.data);
//     //try{
//     mattered = gm(renderOne);

//     assert(mattered.attributes.subject, "Can't send an email without a subject");
//     const layoutHtml = fs.readFileSync(this.layout,"utf-8");
    
//     const body = md.render(mattered.body);
//     const renderThree = ejs.render(layoutHtml,{
//       data: {
//         body: body, 
//         //preheader: mattered.data.subject
//       }
//     })
//     this.body = renderThree;  
//     this.subject = mattered.attributes.subject;
//   }
// }