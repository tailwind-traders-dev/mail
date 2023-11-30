public abstract class TestBase : IDisposable
{
  
    protected TestBase()
    {
      //load up the test config
      Viper.Test();
      // Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Test");
      // DotEnv.Load();
    }

    public void Dispose()
    {
        // Do "global" teardown here; Called after every test method.
    }
}