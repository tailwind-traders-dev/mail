// a class that wraps the idea of a query
using System.Diagnostics.Contracts;
using System.Dynamic;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Npgsql;


namespace Tailwind.Data;

public class CommandResult{
  public dynamic Data { get; set; }
  public int Inserted { get; set; } = 0;
  public int Updated { get; set; } = 0;
  public int Deleted { get; set; } = 0;
}

public interface ICommand{
  Task<CommandResult> Execute();
}

public class Command : Query, IDisposable
{

  public Command()
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
    var values = o.ToValueList();
    var cols = o.ToColumnList();
    var sql = $"insert into {table} ({cols}) values ({values}) returning id;";
    //Console.WriteLine(sql);
    var cmd = new NpgsqlCommand(sql).AddParams(o);
    return Run(cmd);
  }

  public int Exec(string sql, object o)
  {
    var cmd = new NpgsqlCommand(sql).AddParams(o);
    return Run(cmd);
  }

  public int Update(string table, object settings, object where)
  {
  
    var sets = settings.ToSettingList();
    var sql = $"update {table} set {sets}";
    var cmd = new NpgsqlCommand(sql).AddParams(settings).Where(where);
    return Run(cmd);
  }

  public int Notify(string channel, string payload)
  {
    var sql = $"notify {channel}, '{payload}'";
    var cmd = new NpgsqlCommand(sql);
    return Run(cmd);
  }

  public int Delete(string table, object where)
  {
    if(where == null){
      throw new InvalidOperationException("You must provide a where clause otherwise you'll delete everything. If that's what you want, run it Raw.");
    }
    var sql = $"delete from {table}";
    var cmd = new NpgsqlCommand(sql);
    cmd.Where(where);
    return Run(cmd);
  }

  dynamic Run(NpgsqlCommand cmd)
  {
    cmd.Connection = _conn;
    cmd.Transaction = _tx;
    //Console.WriteLine(cmd.CommandText);

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


  }
  public void Dispose()
  {
    //thanks Oren! https://ayende.com/blog/2577/did-you-know-find-out-if-an-exception-was-thrown-from-a-finally-block
    if (Marshal.GetExceptionPointers() == IntPtr.Zero){
      _tx.Commit();
    }else{
      Console.WriteLine("Rolling back transaction");
      _tx.Rollback();
    }
    _conn.Close();
  }
}