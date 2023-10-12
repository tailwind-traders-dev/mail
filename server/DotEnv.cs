using System;
using System.IO;

//https://dusted.codes/dotenv-in-dotnet
public static class DotEnv
{
    public static void Load()
    {
        var execDirectory = Directory.GetCurrentDirectory();
        string projectDirectory = Directory.GetParent(execDirectory).Parent.Parent.FullName;
        var filePath = Path.Combine(projectDirectory, ".env");
        if (!File.Exists(filePath))
            return;
        Console.WriteLine("Loading .env file");
        foreach (var line in File.ReadAllLines(filePath))
        {

          var parts = line.Split(
              '=',
              StringSplitOptions.RemoveEmptyEntries);
          
          if (parts.Length != 2)
              continue;
          Console.WriteLine("Setting {0} to {1}", parts[0], parts[1]);
          Environment.SetEnvironmentVariable(parts[0], parts[1].Replace("\"", ""));
        }
    }
}
