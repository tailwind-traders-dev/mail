const { Model, DataTypes } = require('sequelize');
class Member extends Model{}
exports.init = function(sequelize){
  Member.init({
    email: {
      type: DataTypes.TEXT,
      allowNull: false,
      unique: true
    },
    unsubbed: {
      type: DataTypes.BOOLEAN,
      allowNull: false,
      defaultValue: false
    },
    }, {
     sequelize,
     underscored: true,
     timestamps: false,
     hooks : {
       //https://github.com/sequelize/sequelize/blob/v6/src/hooks.js#L7
    }
  });
  return Member;
}