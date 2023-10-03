const { Model, DataTypes } = require('sequelize');

class Email extends Model{

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