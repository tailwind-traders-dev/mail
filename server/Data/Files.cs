//create a class that generates a COPY command based on a past in list of objects
using System.Text;
namespace Tailwind.Data;

public class FileOps{
  public static string Copy<T>(string table, IEnumerable<T> list){
    var props = typeof(T).GetProperties();
    var columns = props.Select(p => p.Name).ToArray();
    var sb = new StringBuilder();
    sb.AppendLine($"COPY {table} ({string.Join(",", columns)}) FROM STDIN (FORMAT BINARY)");
    foreach (var item in list)
    {
      foreach (var prop in props)
      {
        var value = prop.GetValue(item, null);
        if(value is null){
          sb.AppendLine("\\N");
        }else{
          sb.AppendLine(value.ToString());
        }
      }
    }
    sb.AppendLine("\\.");
    return sb.ToString();
  }
}