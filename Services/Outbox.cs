namespace Tailwind.Mail.Services;
using System.Net.Mail;
using System.Net;
using System.Text.Json;
using System.Text.Encodings;
using Tailwind.Mail.Data.Models;
using Tailwind.Mail.Data;
using Microsoft.EntityFrameworkCore;

public class Outbox 
{

  public static Broadcast Queue(Broadcast broadcast, IList<Message> messages){
    var _db = new Db();
    //update the status
    broadcast.Status = "queued";
    broadcast.MessageCount = messages.Count;
    _db.Add(broadcast);
    _db.AddRange(messages); //this isn't a relation as it's a historical table
    _db.SaveChanges();
    _db.Database.SqlQuery<int>($"NOTIFY broadcasts, '{broadcast.Slug}'").ToListAsync();
    return broadcast;
  }

  public static async Task<Message> SendNow(Email email, string to, string from)
  {
    var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
     
    var host = Environment.GetEnvironmentVariable("SMTP_HOST");
    var user = Environment.GetEnvironmentVariable("SMTP_USER");
    var pw = Environment.GetEnvironmentVariable("SMTP_PASSWORD");
    var port = 465;
    if (env == "Integration"){
      host="smtp.ethereal.email";
      user = Environment.GetEnvironmentVariable("ETHEREAL_USER");
      pw = Environment.GetEnvironmentVariable("ETHEREAL_PASSWORD");
      port = 587;
    }
    
    var sendMessage = new MailMessage{
      IsBodyHtml = true,
      Subject = email.Subject,
      Body = email.Html,
      From=new MailAddress(from),
      To = {new MailAddress(to)}
    };
    
    if (env == "Integration" || env == "Production"){
      Console.WriteLine("Sending email");
      var _client = new SmtpClient(host, port);
      _client.Credentials = new NetworkCredential(user, pw);
      _client.UseDefaultCredentials = false;
      _client.EnableSsl = true;
      await _client.SendMailAsync(sendMessage);
      //TODO: handle the receipt somehow!

    }
    var _db = new Db();
    var message = new Message{
      SendTo = to,
      SendFrom = from,
      SentAt = DateTimeOffset.UtcNow,
      Status = "sent",
      Subject = email.Subject,
      Html = email.Html,
    };
    
    _db.Add(message);

    await _db.SaveChangesAsync();
    return message;
  }

}