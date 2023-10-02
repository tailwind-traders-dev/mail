const { Model, DataTypes } = require('sequelize');
class Tag extends Model{}
exports.init = function(sequelize){
  Tag.init({
    name: {
      type: DataTypes.TEXT,
      allowNull: false,
      unique: true
    },
    }, {
     sequelize,
     underscored: true,
     timestamps: false,
     hooks : {
       //https://github.com/sequelize/sequelize/blob/v6/src/hooks.js#L7
    }
  });
  return Tag;
}