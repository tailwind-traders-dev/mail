const { Model, DataTypes } = require('sequelize');
class Sequence extends Model{}
exports.init = function(sequelize){
  Sequence.init({
    slug: { //this should correspond to the files on disk...
      type: DataTypes.TEXT,
      allowNull: false,
      unique: true
    },
    name: {
      type: DataTypes.TEXT,
      allowNull: false,
    },
    }, {
     sequelize,
     underscored: true,
     timestamps: false,
     hooks : {
       //https://github.com/sequelize/sequelize/blob/v6/src/hooks.js#L7
    }
  });
  return Sequence;
}