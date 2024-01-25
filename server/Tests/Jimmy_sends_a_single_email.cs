using Xunit;
using Tailwind.Mail.Models;
using Tailwind.Mail.Services;
namespace Tailwind.Mail.Tests;

public class Jimmy_Sends_Single_Email:TestBase
{
  [Fact]
  public async Task The_email_is_sent()
  {
    var mssg = new Message("test", "test@test.com",
      "Test Email", "<h1>Test Email</h1><p>This is a test email</p>");

    //this won't actually send an email, but it will update the status
    //if you want to test mail sending, set environment to integration in the test base
    //at some point we'll add proper integration testing
    var message = await Outbox.Send(mssg);
    //assert
    Assert.Equal("sent",message.Status);
  }
}
