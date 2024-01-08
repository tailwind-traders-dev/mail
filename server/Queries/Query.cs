using System.Data;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using System.Dynamic;
using SQLitePCL;
namespace Tailwind.Mail.Queries;



//this is a base class for the commands 
public class Query{
  string _connectionString;
  string? _sql;
  Dictionary<string,object> parameters = new Dictionary<string,object>();

  public Query()
  {
    //the ENV stuff should be loaded by now, if not, we throw
    var config = Viper.Config();
    _connectionString = config.Get("DATABASE_URL");
    if(string.IsNullOrEmpty(_connectionString))
    {
      throw new InvalidProgramException("DATABASE_URL is not set");
    }
  }
  
  public Task<IList<dynamic>> Select(string tableName, Dictionary<string,object> where)
  {
    //thank you copilot
    _sql = $"select * from {tableName} where {string.Join(" and ", where.Keys.Select(k=>$"{k} = @{k}"))}";
    parameters = where;
    return Run();
  }


  public Task<IList<dynamic>> Select(string tableName, int limit = 1000)
  {
    //thank you copilot
    _sql = $"select * from {tableName} limit {limit}";
    return Run();
  }

  public async Task<dynamic> First(string tableName, Dictionary<string,object> where)
  {
    //thank you copilot
    _sql = $"select * from {tableName} where {string.Join(" and ", where.Keys.Select(k=>$"{k} = @{k}"))}";
    parameters = where;
    var res = await Run();
    return res.IsNullOrEmpty() ? null : res.First();
  }

  public async Task<IList<dynamic>> Run()
  {
    using var connection = new NpgsqlConnection(_connectionString);
    await connection.OpenAsync();
    using var command = new NpgsqlCommand(_sql, connection);
    foreach(var parameter in parameters)
    {
      command.Parameters.AddWithValue(parameter.Key, parameter.Value);
    }
    var rdr = await command.ExecuteReaderAsync();
    var result = new List<dynamic>();
    while(rdr.Read())
    {
      result.Add(RecordToExpando(rdr));
    }
    return result;
  }
  public async Task<IList<dynamic>> Run(string sql, Dictionary<string,object> parameters)
  {
    _sql = sql;
    this.parameters = parameters;
    return await Run();
  }
  public async Task<IList<dynamic>> Run(string sql)
  {
    _sql = sql;
    return await Run();
  }


  dynamic RecordToExpando(IDataReader reader)
  {
    dynamic e = new ExpandoObject();
    var d = (IDictionary<string, object>)e;
    object[] values = new object[reader.FieldCount];
    reader.GetValues(values);
    for(int i = 0; i < values.Length; i++)
    {
      var v = values[i];
      d.Add(reader.GetName(i), DBNull.Value.Equals(v) ? null : v);
    }
    return e;
  }

}