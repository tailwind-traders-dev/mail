const { Model, DataTypes } = require('sequelize');
class Broadcast extends Model{}
exports.init = function(sequelize){
  Broadcast.init({
    slug: {
      type: DataTypes.TEXT,
      allowNull: false,
      unique: true
    },
    name: DataTypes.TEXT,
    description: DataTypes.TEXT,
    }, {
     sequelize,
     underscored: true,
     modelName: "broadcast",
     hooks : {
       //https://github.com/sequelize/sequelize/blob/v6/src/hooks.js#L7
    }
  });
  return Broadcast;
}