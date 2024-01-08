using System.Runtime.InteropServices;
using Npgsql;

namespace Tailwind.Mail.Commands;

//this is a base class for the commands 
public class Command{
  string _connectionString;
  Dictionary<string,Dictionary<string,object>> _queue = new Dictionary<string,Dictionary<string,object>>();
  public Command()
  {
    //the ENV stuff should be loaded by now, if not, we throw
    var config = Viper.Config();
    _connectionString = config.Get("DATABASE_URL");
    if(string.IsNullOrEmpty(_connectionString))
    {
      throw new InvalidProgramException("DATABASE_URL is not set");
    }
  }
  public string Delete(string tableName, Dictionary<string,object> where)
  {
    //thank you copilot
    var sql = $"delete from {tableName} where {string.Join(" and ", where.Keys.Select(k=>$"{k} = @{k}"))}";
    Queue(sql, where);
    return sql;
  }
  public string Insert(string tableName, Dictionary<string,object> values)
  {
    //thank you copilot
    var sql = $"insert into {tableName} ({string.Join(",",values.Keys)}) values ({string.Join(",",values.Keys.Select(k=>$"@{k}"))})";
    Queue(sql, values);
    return sql;

  }
  public string Update(string tableName, int id, Dictionary<string,object> values)
  {
    //thank you copilot
    var sql = $"update {tableName} set {string.Join(",",values.Keys.Select(k=>$"{k} = @{k}"))} where id = {id}";
    values.Add("id", id);
    Queue(sql, values);
    return sql;
  }
  public void Queue(string sql, Dictionary<string,object> parameters)
  {
    using var command = new NpgsqlCommand(sql);
    foreach(var parameter in parameters)
    {
      command.Parameters.AddWithValue(parameter.Key, parameter.Value);
    }
    _queue.Add(sql, parameters);
  }
  public async Task<int> Execute()
  {
    using var connection = new NpgsqlConnection(_connectionString);
    await connection.OpenAsync();
    var results = 0;
    using var transaction = connection.BeginTransaction();
    try{
      foreach(var query in _queue)
      {
        using var command = new NpgsqlCommand(query.Key, connection, transaction);
        foreach(var parameter in query.Value)
        {
          Console.WriteLine($"{parameter.Key} = {parameter.Value}");
          command.Parameters.AddWithValue(parameter.Key, parameter.Value);
        }
         await command.ExecuteNonQueryAsync();
        results+=1; //just record the number of commands run
      }
      transaction.Commit();
    }catch(NpgsqlException ex){
      transaction.Rollback();
      throw ex; //rethrow here
    }
    return results;
  }

}