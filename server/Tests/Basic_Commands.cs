using Xunit;
using Tailwind.Data;

namespace Tailwind.Mail.Tests;

class CreateContactCommand: ICommand {
  public string Name { get; set; }
  public string Email { get; set; }
  public CreateContactCommand(string name, string email)
  {
    Name = name;
    Email = email;
  }
  public async Task<dynamic> Execute()
  {

    //Any issue with the database will roll the transaction back
    //However, if there is programmatic error, you MUST call Rollback() manually
    using(var db = new Transaction()){
      //Commit is implicit if no exceptions are thrown

      //delete existing contact
      var deleted = db.Delete("mail.contacts", new {email="test@test.com"});
      //create a new one
      var newContactId = db.Insert("mail.contacts", new{name="Test User", email="test@test.com"});
      //update existing
      var updated = db.Update("mail.contacts", new {name="Big Time"}, new {id=newContactId});
      
      //throw new Exception("test");
      //pull the new contact back out
      var newContact = db.First("mail.contacts", new {id=newContactId});
      
      return new{
        newContact,
        deleted,
        newContactId,
        updated
      };
  
    };
    
  }
}

public class Basic_Commands:TestBase
{
  [Fact]
  public async Task A_contact_is_deleted_created_and_updated()
  {

    var results = await new CreateContactCommand("Test User", "test@test.com").Execute();
    Assert.Equal("Big Time", results.newContact.name);
    Assert.Equal("test@test.com", results.newContact.email);
    Assert.Equal(1, results.updated);
  }
}
