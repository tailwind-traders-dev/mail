using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Tailwind.Data;
using Tailwind.Mail.Models;
using Dapper;
using System.Data;
namespace Tailwind.Mail.Services;

public class BackgroundSend : BackgroundService
{
  IDbConnection _conn { get; set; }
  IEmailSender _outbox { get; set; }
  public BackgroundSend(IEmailSender outbox, IDb db)
  {
    _outbox = outbox;
    _conn = db.Connect();;
  }
  protected override async Task ExecuteAsync(CancellationToken stoppingToken)
  {
    while (!stoppingToken.IsCancellationRequested) {
      // Your background task here.
      // You can use the stoppingToken to stop the task if needed.
      //get 10 pending mails at a time to send
      Console.WriteLine("Checking for email to send...");

        var sql = "select * from mail.messages where status = 'pending' and send_at <= now() limit 10";
        dynamic messages = await _conn.QueryAsync(sql);
        var sendCount = 0;
        foreach(var row in messages){
          var message = new Message{
            ID = row.id,
            Subject = row.subject,
            Status = row.status,
            Slug = row.slug,
            Html = row.html,
            SendAt = row.send_at,
            SendTo = row.send_to,
            SendFrom = row.send_from
          };
          await _outbox.Send(message);
          sql = "update mail.messages set status = 'sent', sent_at = now() where id = @id";
          await _conn.ExecuteAsync(sql, new {id = message.ID});
          sendCount++;
        }
        if(sendCount > 0){
          Console.WriteLine($"Sent {sendCount} messages");
        }
      
      await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
    }
  }
}