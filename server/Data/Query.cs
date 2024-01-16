using Npgsql;

namespace Tailwind.Data;

public class Query {
  protected NpgsqlConnection _conn { get; set; }
  protected NpgsqlTransaction _tx { get; set; }
  
  public Query()
  {
    var _connectionString = Viper.Config().Get("DATABASE_URL");
    if(String.IsNullOrEmpty(_connectionString)){
      throw new InvalidOperationException("You must set a DATABASE_URL environment variable or call Viper.Config()");
    }
    _conn = new NpgsqlConnection(_connectionString);
    _conn.Open();
  }

  public dynamic Raw(string sql, object o)
  {
    var cmd = new NpgsqlCommand(sql).AddParams(o);
    return Run(cmd);
  }
  public dynamic Raw(string sql)
  {
    var cmd = new NpgsqlCommand(sql);
    return Run(cmd);
  }

  public long Count(string table)
  {
    var sql = $"select count(1) as count from {table}";
    var cmd = new NpgsqlCommand(sql);
    dynamic res =  Run(cmd);
    return res != null ? res.count : 0;
  }
  public long Count(string table, object where)
  {
    var sql = $"select count(1) as count from {table}";
    var cmd = new NpgsqlCommand(sql).Where(where);
    dynamic res =  Run(cmd);
    return res != null ? res.count : 0;
  }

  public IList<dynamic> Select(string table)
  {
    var sql = $"select * from {table} limit 1000";
    var cmd = new NpgsqlCommand(sql);
    return Run(cmd);
  }
  public IList<dynamic> Select(string table, object where)
  {
    var sql = $"select * from {table}";
    var cmd = new NpgsqlCommand(sql).Where(where).Limit(1000);
    return Run(cmd);
  }
  public dynamic First(string table, object where)
  {
    var sql = $"select * from {table}";
    var cmd = new NpgsqlCommand(sql).Where(where);
    return Run(cmd);
  }
  dynamic Run(NpgsqlCommand cmd)
  {

    cmd.Connection = _conn;
    if(_tx != null){
      cmd.Transaction = _tx;
    }
    //Console.WriteLine(cmd.CommandText);

    using(var rdr = cmd.ExecuteReader()){
      var results = rdr.ToExpandoList();
      if(results.Count() == 1){
        return results.First();
      }else if(results.Count() == 0){
        return null;
      }else{
        return results;
      }
    }

  }
}