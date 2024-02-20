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
    List<Message> mssgs = new List<Message>();
    for(var i = 0; i < 25; i++)
    {
      mssgs.Add(new Message{
        SendTo = $"test{i}@test.com",
        SendFrom = "thing@dev.null",
        Subject = "Test",
        Html = "<h1>Test</h1>",
      });
    }
    var sent = await _outbox.SendBulk(mssgs);
    
  }
}