const { Model, DataTypes } = require('sequelize');
class Message extends Model{}
exports.init = function(sequelize){
  Message.init({
    to: {
      type: DataTypes.TEXT,
      allowNull: false,
    },
    from: {
      type: DataTypes.TEXT,
      allowNull: false,
    },
    subject: {
      type: DataTypes.TEXT,
      allowNull: false,
    },
    html: {
      type: DataTypes.TEXT,
      allowNull: false,
    },
    sent_at: {
      type: DataTypes.DATE(6),
      allowNull: false,
      defaultValue: new Date().toISOString()
    },
    receipt: {
      type: DataTypes.JSON,
    },
    }, {
     sequelize,
     underscored: true,
     timestamps: false,
     hooks : {
       //https://github.com/sequelize/sequelize/blob/v6/src/hooks.js#L7
    }
  });
  return Message;
}