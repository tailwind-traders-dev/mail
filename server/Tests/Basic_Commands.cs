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
  public async Task<TransactionResult> Execute()
  {

    //Any issue with the database will roll the transaction back
    //However, if there is programmatic error, you MUST call Rollback() manually
    //Commit is implicit if no db exceptions are thrown
    var result = new TransactionResult();
    using(var db = new Transaction()){
      
      //delete existing contact
      result.Deleted = db.Delete("mail.contacts", new {email="test@test.com"});
      
      //create a new one
      var newContactId = db.Insert("mail.contacts", new{name="Test User", email="test@test.com"});
      
      //update existing
      result.Updated = db.Update("mail.contacts", new {name="Big Time"}, new {id=newContactId});
      
      //pull the new contact back out
      result.Data = db.First("mail.contacts", new {id=newContactId});
      
      return result;
  
    };
    
  }
}

public class Basic_Commands:TestBase
{
  [Fact]
  public async Task A_contact_is_deleted_created_and_updated()
  {

    var results = await new CreateContactCommand("Test User", "test@test.com").Execute();
    Assert.Equal("Big Time", results.Data.name);
    Assert.Equal("test@test.com", results.Data.email);
    Assert.Equal(1, results.Updated);
  }
}
