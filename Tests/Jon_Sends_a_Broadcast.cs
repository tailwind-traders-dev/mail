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

    //add 10,000 contacts with "Test" tag
    var tag = new Tag{Name="Test", Slug="test"};

    //create a list of contacts with the tag
    var _contacts = new List<Contact>();
    for(var i = 0; i < 10000; i++){
      var c = new Contact{
        Email = $"test{i}@test.com",
        Subscribed = true
      };
      c.Tags.Add(tag);
      _contacts.Add(c);
    }

    var broadcast = Broadcast.Create("test","This is a test","# Hello");
    var messages = Message.CreateForBroadcast(broadcast, _contacts);

    broadcast = Outbox.Queue(broadcast, messages);
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