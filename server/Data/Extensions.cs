using Npgsql;
using System.Dynamic;
using System.Data;
using System.Collections.Specialized;
namespace Tailwind.Data;

public static class CommandExtensions
{
  /// <summary>
  /// Extension method for adding in a bunch of parameters
  /// </summary>
  public static void AddParams(this NpgsqlCommand cmd, params object[] args)
  {
    foreach (var item in args)
    {
      AddParam(cmd, item);
    }
  }
  /// <summary>
  /// Extension for adding single parameter
  /// </summary>
  public static void AddParam(this NpgsqlCommand cmd, object item)
  {
    var p = cmd.CreateParameter();
    p.ParameterName = string.Format("@{0}", cmd.Parameters.Count);
    if (item == null)
    {
      p.Value = DBNull.Value;
    }
    else
    {
      if (item.GetType() == typeof(Guid))
      {
        p.Value = item.ToString();
        p.DbType = DbType.String;
        p.Size = 4000;
      }
      else if (item.GetType() == typeof(ExpandoObject))
      {
        var d = (IDictionary<string, object>)item;
        p.Value = d.Values.FirstOrDefault();
      }
      else
      {
        p.Value = item;
      }
      if (item.GetType() == typeof(string))
        p.Size = ((string)item).Length > 4000 ? -1 : 4000;
    }
    cmd.Parameters.Add(p);
  }
  /// <summary>
  /// Turns an IDataReader to a Dynamic list of things
  /// </summary>
  public static List<dynamic> ToExpandoList(this IDataReader rdr)
  {
    var result = new List<dynamic>();
    while (rdr.Read())
    {
      result.Add(rdr.RecordToExpando());
    }
    return result;
  }
  public static dynamic RecordToExpando(this IDataReader rdr)
  {
    dynamic e = new ExpandoObject();
    var d = e as IDictionary<string, object>;
    object[] values = new object[rdr.FieldCount];
    rdr.GetValues(values);
    for (int i = 0; i < values.Length; i++)
    {
      var v = values[i];
      d.Add(rdr.GetName(i), DBNull.Value.Equals(v) ? null : v);
    }
    return e;
  }
  /// <summary>
  /// Turns the object into an ExpandoObject
  /// </summary>
  public static dynamic ToExpando(this object o)
  {
    if (o.GetType() == typeof(ExpandoObject)) return o; //shouldn't have to... but just in case
    var result = new ExpandoObject();
    var d = result as IDictionary<string, object>; //work with the Expando as a Dictionary
    if (o.GetType() == typeof(NameValueCollection) || o.GetType().IsSubclassOf(typeof(NameValueCollection)))
    {
      var nv = (NameValueCollection)o;
      nv.Cast<string>().Select(key => new KeyValuePair<string, object>(key, nv[key])).ToList().ForEach(i => d.Add(i));
    }
    else
    {
      var props = o.GetType().GetProperties();
      foreach (var item in props)
      {
        d.Add(item.Name, item.GetValue(o, null));
      }
    }
    return result;
  }

  /// <summary>
  /// Turns the object into a Dictionary
  /// </summary>
  public static IDictionary<string, object> ToDictionary(this object thingy)
  {
    return (IDictionary<string, object>)thingy.ToExpando();
  }
}