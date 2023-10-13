//xunit test suite
using Xunit;
using Tailwind.Mail.Data;
using Tailwind.Mail.Data.Models;
using Microsoft.EntityFrameworkCore;
using Tailwind.Mail.Services;
namespace Tailwind.Mail.Tests;

public class We_can_really_send_email:TestBase
{
  public We_can_really_send_email()
  {
    Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Integration");
  }
  [Fact(Skip ="This is an integration test that will actually send an email uncomment if you want to run it")]
  public async Task it_will_actually_send()
  {
    var outbox = new Outbox();
    var message = new Message
    {
      SendFrom = "test@test.com",
      SendTo = "test@test.com",
      Subject = "This is a Test from the Integration bits",
      Html = "<h1>Test</h1><p>How ya doing?</p><p>Here's a <a href='https://www.google.com'>link</a></p>"
    };
    //act
    await outbox.SendNow(message);
    //assert
    Assert.NotNull(message.SentAt);
  }
}