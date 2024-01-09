using Npgsql;

namespace Tailwind.Data;

public class Command {
  Dictionary<string,Dictionary<string,object>> _queue = new Dictionary<string,Dictionary<string,object>>();
  public NpgsqlConnection Connection { get; set; }
  public NpgsqlTransaction Transaction { get; set; }
  string _connectionString;

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
  public NpgsqlConnection Open(){
    Connection = new NpgsqlConnection(_connectionString);
    Connection.Open();
    return Connection;
  }
  public Command Begin(){
    Connection = new NpgsqlConnection(_connectionString);
    Connection.Open();
    Transaction = Connection.BeginTransaction();
    return this;
  }

  public Command Commit(){
    Transaction.Commit();
    Connection.Close();
    return this;
  }
  public Command Rollback(){
    Transaction.Rollback();
    Connection.Close();
    return this;
  }

}