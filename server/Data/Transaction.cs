// a class that wraps the idea of a query
using System.Dynamic;
using Microsoft.IdentityModel.Tokens;
using Npgsql;


namespace Tailwind.Data;

public class TransactionResult{
  public dynamic Data { get; set; }
  public int Inserted { get; set; } = 0;
  public int Updated { get; set; } = 0;
  public int Deleted { get; set; } = 0;
}

public interface ICommand{
  Task<TransactionResult> Execute();
}

public class Transaction : Query, IDisposable
{

  bool _shouldCommit { get; set; } = true;
  public Transaction()
  {
    _tx = _conn.BeginTransaction();
  }

  public void Commit(){
    _tx.Commit();
  }
  public void Rollback(){
    _tx.Rollback();
  }
  
  public int Insert(string table, object o)
  {
    var expando = o.ToExpando();
    var values = (IDictionary<string, object>)expando;
    var sql = $"insert into {table} ({string.Join(", ", values.Keys)}) values ({string.Join(", ", values.Keys.Select(k => $"@{k}"))}) returning id;";
    var cmd = new NpgsqlCommand(sql).AddParams(o);
    return Run(cmd);
  }

  public int Update(string table, object settings, object where)
  {
     
    var settingsExpando = settings.ToExpando();
    var dSettings = (IDictionary<string, object>)settingsExpando;
    var sql = $"update {table} set {string.Join(",", dSettings.Keys.Select(k => $"{k}=@{k}"))}";
    var cmd = new NpgsqlCommand(sql).AddParams(settings);
    cmd.Where(where);
    return Run(cmd);
  }

  public int Delete(string table, object where)
  {
    var expando = where.ToExpando();
    var dict = (IDictionary<string, object>)expando;
    var sql = $"delete from {table}";
    var cmd = new NpgsqlCommand(sql);
    if(dict.IsNullOrEmpty()){
      throw new InvalidOperationException("You must provide a where clause otherwise you'll delete everything. If that's what you want, run it Raw.");
    }
    cmd.Where(dict);
    return Run(cmd);
  }

  dynamic Run(NpgsqlCommand cmd)
  {
    cmd.Connection = _conn;
    cmd.Transaction = _tx;
    Console.WriteLine(cmd.CommandText);
    try{
      if(cmd.CommandText.Contains("returning")){
        var result = cmd.ExecuteScalar();
        if(result is null){
          return 0; //TODO: this is a hack
        }else{
          return result;
        }
      }else{
        var recordsAffected = cmd.ExecuteNonQuery();
        return recordsAffected;
      }
    }catch(Exception ex){
      _tx.Rollback();
      _shouldCommit = false;
      throw ex;
    }

  }
  public void Dispose()
  {
    if(_shouldCommit){
      _tx.Commit();
    }
    _conn.Close();
  }
}