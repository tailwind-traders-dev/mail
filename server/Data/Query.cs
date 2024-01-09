// a class that wraps the idea of a query
using System.Data;
using System.Dynamic;
using System.Reflection.Metadata.Ecma335;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Npgsql;
using NuGet.Packaging;

namespace Tailwind.Data;


//the goal here is to have a generalized class that creates a NpgsqlCommand
//using a fluent interface (aka method chaining).

public class Query
{

  string? sql { get; set; }
  public Dictionary<string, object> parameters = new Dictionary<string, object>();
  public List<NpgsqlCommand> Batch { get; set; } = new List<NpgsqlCommand>();

  public Query(string sql, Dictionary<string, object> parameters)
  {
    this.sql = sql;
    this.parameters = parameters;
  }
  public Query(string sql)
  {
    this.sql = sql;
  }
  public Query()
  {
    
  }

  public static Query Select(string table)
  {
    var qry = new Query { sql = $"select * from {table}" };
    return qry;
  }


  public static Query Delete(string table)
  {
    //thank you copilot
    var sql = $"delete from {table}";
    var qry = new Query { sql = sql };
    return qry;
  }

  public static Query Insert(string table, Dictionary<string, object> values)
  {
    //thank you copilot
    var sql = $"insert into {table} ({string.Join(", ", values.Keys)}) values ({string.Join(", ", values.Keys.Select(k => $"@{k}"))}) returning id;";

    var qry = new Query { sql = sql, parameters = values };
    return qry;
  }

  public static Query Update(string table, Dictionary<string, object> values)
  {
    //thank you copilot
    var sql = $"update {table} set {string.Join(",", values.Keys.Select(k => $"{k} = @{k}"))}";
    var qry = new Query { sql = sql, parameters = values };
    return qry;
  }

  public Query Where(string expression)
  {
    sql += $" where {expression}";
    return this;
  }

  public Query Where(Dictionary<string, object> parameters)
  {
    sql += $" where {string.Join(" and ", parameters.Keys.Select(k => $"{k}=@{k}"))}";
    this.parameters.AddRange(parameters);
    return this;
  }
  public Query Limit(int limit)
  {
    sql += $" limit {limit}";
    return this;
  }

  public Query Order(string column = "id", string direction = "asc")
  {
    sql += $" order by {column} {direction}";
    return this;
  }

  NpgsqlCommand Build()
  {
    var cmd = new NpgsqlCommand(sql);
    foreach (var item in parameters)
    {

      Console.WriteLine($"{item.Key} = {item.Value}");
      cmd.Parameters.AddWithValue(item.Key, item.Value);

    }
    return cmd;
  }

  public async Task<dynamic> First(){
    var cmd = Build();
    var _connectionString = Viper.Config().Get("DATABASE_URL");
    using var conn = new NpgsqlConnection(_connectionString);
    conn.Open();
    cmd.Connection = conn;
    using(var rdr = await cmd.ExecuteReaderAsync()){
      var results = rdr.ToExpandoList();
      if(results.Count > 0){
        return results[0];
      }else{
        return null;
      }
    }
  }
  public async Task<List<dynamic>> All(){
    var cmd = Build();
    var _connectionString = Viper.Config().Get("DATABASE_URL");
    using var conn = new NpgsqlConnection(_connectionString);
    conn.Open();
    cmd.Connection = conn;
    using(var rdr = await cmd.ExecuteReaderAsync()){
      var results = rdr.ToExpandoList();
      return results;
    }
  }
  public async Task<dynamic> Run(NpgsqlConnection conn, NpgsqlTransaction tx = null)
  {
    var cmd = Build();

    cmd.Connection = conn;
    if(tx !=null){
      cmd.Transaction = tx;
    }
    //Console.WriteLine(cmd.CommandText);
    if(cmd.CommandText.Contains("select") || cmd.CommandText.Contains("with")){
      using(var rdr = await cmd.ExecuteReaderAsync()){
        var results = rdr.ToExpandoList();
        if(results.Count == 1){
          return results[0];
        }else{
          return results;
        }
      }
    }
    if(cmd.CommandText.Contains("returning")){
      var result = await cmd.ExecuteScalarAsync();
      if(result is null){
        return 0; //TODO: this is a hack
      }else{
        return result;
      }
    }else{
      var recordsAffected = await cmd.ExecuteNonQueryAsync();
      return recordsAffected;
    }
  }
}