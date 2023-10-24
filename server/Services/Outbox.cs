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
  private readonly SmtpClient _client;

  public static async Task Queue(Email email, ICollection<Contact> contacts, string from){
    //drops the message into the DB... somewhere?
    var _db = new Db();
    var _messages = new List<Message>();
    foreach (var contact in contacts)
    {
      var message = new Message{
        Email = email,
        Subject=email.Subject,
        SendTo = contact.Email,
        SendFrom = from,
        Status = "queued",
        SendAt = DateTimeOffset.UtcNow + TimeSpan.FromHours(email.DelayHours),
        Html = email.Html,
      };
      _messages.Add(message);
    }
    _db.AddRange(_messages);
    await _db.SaveChangesAsync();
    await _db.Database.SqlQuery<int>($"NOTIFY messages, 'new sequence'").ToListAsync();

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

    var _client = new SmtpClient(host, port);
    _client.Credentials = new NetworkCredential(user, pw);
    _client.UseDefaultCredentials = false;
    _client.EnableSsl = true;
    
    var sendMessage = new MailMessage{
      IsBodyHtml = true,
      Subject = email.Subject,
      Body = email.Html,
      From=new MailAddress(from),
      To = {new MailAddress(to)}
    };
    
    if (env == "Integration" || env == "Production"){
      Console.WriteLine("Sending email");
      await _client.SendMailAsync(sendMessage);
      //TODO: handle the receipt somehow!

    }
    var _db = new Db();
    var message = new Message{
      Email = email,
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