const { Model, DataTypes, Sequelize } = require('sequelize');
class Outbox extends Model{

}
exports.init = function(sequelize){
  Outbox.init({
      send_at: {
        type: DataTypes.BIGINT, //unix timestamp
        allowNull: false,
        defaultValue: 0
      },
      status: {
        type: DataTypes.TEXT,
        allowNull: false,
        defaultValue: "queued"
      },
    }, {
     sequelize,
     underscored: true,
     tableName: "outbox",
     hooks : {
       //https://github.com/sequelize/sequelize/blob/v6/src/hooks.js#L7
    }
  });
  return Outbox;
}