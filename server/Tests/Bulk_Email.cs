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
  IList<Message> _queue;
  public BulkEmailTest()
  {
    _outbox = new MailHogSender();
  }
  [Fact]
  public async void Bulk_email_sending(){
    List<Message> mssgs = new List<Message>();
    for(var i = 0; i < 25; i++)
    {
      var m = new Message{
        SendTo = $"test{i}-bulk@test.com",
        SendFrom = "thing@dev.null",
        Subject = "Bulk Send Test",
        Html = "<h1>Test</h1>",
        Slug = "bulk-send-test",
        SendAt = DateTimeOffset.UtcNow
      };
      m.ID = await Conn.InsertAsync(m);
      mssgs.Add(m);
    }
    Console.WriteLine(mssgs.Count);
    var sent = await _outbox.SendBulk(mssgs);
    Console.WriteLine(sent);
    Assert.Equal(25, sent);
  }
}