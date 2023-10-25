using Xunit;
using Tailwind.Mail.Data;
using Tailwind.Mail.Data.Models;
using Microsoft.EntityFrameworkCore;
using Tailwind.Mail.Services;
namespace Tailwind.Mail.Tests;

public class Jon_Sends_a_Broadcast:TestBase
{

  [Fact]
  public async Task ten_thousand_messages_are_queued()
  {
    var sequence = new Sequence{
      Name = "Test Broadcast",
      Slug = "test-sequence"
    };
    var email = new Email{
      Sequence = sequence,
      Slug = "test-broadcast",
      Subject = "Test",
      Markdown = "# Test",
      DelayHours = 0
    };

    email.Render();


    var _contacts = new List<Contact>();
    for(var i = 0; i < 25000; i++){
      _contacts.Add(new Contact{
        Email = $"test{i}@test.com"
      });
    }
    await Outbox.Queue(email, _contacts, "test@tailwindtraders.dev");
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