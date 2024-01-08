// using Xunit;
// using Tailwind.Mail.Data;
// using Tailwind.Mail.Data.Models;
// using Microsoft.EntityFrameworkCore;
// using Tailwind.Mail.Services;
// namespace Tailwind.Mail.Tests;

// public class Jimmy_Sends_Single_Email:TestBase
// {
//   [Fact]
//   public async Task The_email_is_sent()
//   {
//     var email = new Email{
//       Slug = "test",
//       Sequence=null,
//       Subject = "Test",
//       Markdown = "# Test",
//       DelayHours = 0
//     };
//     email.Render();
//     //act

//     var message = await Outbox.SendNow(email,"test@test.com","test@tailwindtraders.dev");
//     //assert
//     Assert.NotNull(message.SentAt);
//     Assert.Equal("sent",message.Status);
//   }
// }

// // public class ContactBasicTests {
// //   Db _db;
// //   Contact _contact;
// //   public ContactBasicTests()
// //   {

// //     _db = new Db();
// //     _contact = new Contact{Email="test@test.com", Name="Test"};
// //     _db.Add(_contact);
// //     _db.SaveChanges();
// //   }  
// //   [Fact]
// //   public void The_contact_is_saved_with_an_id()
// //   {
// //     //there should be one doc in there
// //     Assert.Equal(_contact.ID, 1);
// //   }
// // }