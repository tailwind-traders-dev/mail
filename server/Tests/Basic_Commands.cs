using Xunit;
using Tailwind.Data;

namespace Tailwind.Mail.Tests;

class CreateContactCommand: Command{
  public string Name { get; set; }
  public string Email { get; set; }
  public CreateContactCommand(string name, string email)
  {
    Name = name;
    Email = email;
  }
  public async Task<dynamic> Execute()
  {
    //creating a transaction for the command bits
    Begin();
    await Query.Delete("mail.contacts").Where(new Dictionary<string,object>{
      {"email", Email}
    }).Run(this.Connection, this.Transaction);

    var vals = new Dictionary<string,object>{
      {"name", "Test User"},
      {"email", "test@test.com"}
    };
    var newContactId = await Query.Insert("mail.contacts", vals).Run(this.Connection, this.Transaction);
    var changedContact = await Query.Update("mail.contacts", new Dictionary<string,object>{
      {"name", "Big Time"},
    }).Where(new Dictionary<string,object>{
      {"id", newContactId}
    }).Run(this.Connection, this.Transaction);
    Commit();

    //can't do this in the transaction because it's a different connection
    var newContact = await Query.Select("mail.contacts").Where(new Dictionary<string,object>{
      {"id", newContactId}
    }).First();

    return newContact;
    
  }
}

public class Basic_Commands:TestBase
{
  [Fact]
  public async Task A_contact_is_deleted_created_and_updated()
  {

    var contact = await new CreateContactCommand("Test User", "test@test.com").Execute();
    Assert.Equal("Big Time", contact.name);
    Assert.Equal("test@test.com", contact.email);
  }
}

// using Xunit;
// using Tailwind.Mail.Commands;
// namespace Tailwind.Mail.Tests;

// public class Basic_Commands:TestBase
// {
//   [Fact]
//   public async Task A_contact_is_deleted_created_and_updated()
//   {
//     var cmd = new Command();
//     var vals =     new Dictionary<string,object>{
//       {"name", "Test User"},
//       {"email", "test@test.com "}
//     };
//     cmd.Delete("mail.contacts", vals);
//     cmd.Insert("mail.contacts", vals);
//     cmd.Update("mail.contacts", 1, new Dictionary<string,object>{
//       {"name", "Test User 2"},
//       {"email", "test2@test.com "}
//     });
//     var results = await cmd.Execute();
//     Assert.Equal(3, results);
//   }
// }