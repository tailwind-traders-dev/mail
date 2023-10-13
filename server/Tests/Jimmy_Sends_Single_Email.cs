using Xunit;
using Tailwind.Mail.Data;
using Tailwind.Mail.Data.Models;
using Microsoft.EntityFrameworkCore;
using Tailwind.Mail.Services;
namespace Tailwind.Mail.Tests;

public class Jimmy_Sends_Single_Email:TestBase
{
  [Fact]
  public async Task The_email_is_sent()
  {
    //arrange

    var outbox = new Outbox();
    var message = new Message
    {
      SendFrom = "test@test.com",
      SendTo = "test@test.com",
      Subject = "Test",
      Html = "<h1>Test</h1>"
    };
    //act
    await outbox.SendNow(message);
    //assert
    Assert.NotNull(message.SentAt);
  }
}

// public class ContactBasicTests {
//   Db _db;
//   Contact _contact;
//   public ContactBasicTests()
//   {

//     _db = new Db();
//     _contact = new Contact{Email="test@test.com", Name="Test"};
//     _db.Add(_contact);
//     _db.SaveChanges();
//   }  
//   [Fact]
//   public void The_contact_is_saved_with_an_id()
//   {
//     //there should be one doc in there
//     Assert.Equal(_contact.ID, 1);
//   }
// }