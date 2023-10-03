const { Model, DataTypes } = require('sequelize');
const ejs = require("ejs");

var md = require('markdown-it')();
const Message = require("./message");

class Email extends Model{

  render(data = {}){
    
    //The idea with EJS is that you could pass data in but... I think
    //we'll just have "contextual" data... punting on that for now
    
    //let renderOne;
    //renderOne =  ejs.render(this.markdown, data);
    //try{
    //mattered = gm(renderOne);
  
    //assert(mattered.attributes.subject, "Can't send an email without a subject");
    //const layoutHtml = fs.readFileSync(this.layout,"utf-8");
    
    return md.render(this.markdown);
  }
}

//An email is basically the "prototype" of a message. It has a body and a subject, but turns
//into a Message when sent to people. 1 Email can be turned into 1000 messages, for instance

exports.init = function(sequelize){
  Email.init({
    slug: {
      type: DataTypes.TEXT,
      allowNull: false,
      unique: true
    },
    subject: {
      type: DataTypes.TEXT,
      allowNull: false,
    },
    preview: {
      type: DataTypes.TEXT,
    },
    delay_hours : {
      type: DataTypes.INTEGER,
      allowNull: false,
      defaultValue: 0, //send right now
    },
    markdown: DataTypes.TEXT,
    text: DataTypes.TEXT,
    }, {
     sequelize,
     underscored: true,
     modelName: "email",
     hooks : {
       //https://github.com/sequelize/sequelize/blob/v6/src/hooks.js#L7
    }
  });
  return Email;
}