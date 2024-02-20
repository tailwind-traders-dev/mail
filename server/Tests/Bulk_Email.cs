using Xunit;
using Tailwind.Data;
using Tailwind.Mail.Commands;
using Tailwind.Mail.Models;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Tailwind.Mail.Api.Admin;
using Tailwind.Mail.Services;

namespace Tailwind.Mail.Tests;

//probably end up mocking this at some point
//for now, just use the test db


[Collection("Bulk email")]
public class BulkEmailTest:TestBase
{
  MailHogSender _outbox;
  public BulkEmailTest()
  {
    _outbox = new MailHogSender();
  }
  [Fact]
  public async void Bulk_email_sending(){
    await _outbox.Send(new Message{
      SendTo = "someone@test.com",
      SendFrom = "test@test.com",
      Subject = "Test",
      Html = "<h1>Test</h1>"
    });
  }
}