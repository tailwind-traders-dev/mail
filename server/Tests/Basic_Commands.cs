using Xunit;
using Tailwind.Mail.Commands;
namespace Tailwind.Mail.Tests;

public class Basic_Commands:TestBase
{
  [Fact]
  public async Task A_contact_is_deleted_created_and_updated()
  {
    var cmd = new Command();
    var vals =     new Dictionary<string,object>{
      {"name", "Test User"},
      {"email", "test@test.com "}
    };
    cmd.Delete("mail.contacts", vals);
    cmd.Insert("mail.contacts", vals);
    cmd.Update("mail.contacts", 1, new Dictionary<string,object>{
      {"name", "Test User 2"},
      {"email", "test2@test.com "}
    });
    var results = await cmd.Execute();
    Assert.Equal(3, results);
  }
}