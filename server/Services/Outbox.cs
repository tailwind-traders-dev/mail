namespace Tailwind.Mail.Services;
using System.Net.Mail;
using System.Net;
using System.Text.Json;
using System.Text.Encodings;
using Tailwind.Mail.Data.Models;

public interface IOutbox
{
  Task<Message> SendNow(Message message);
}

public class Outbox : IOutbox
{
  private readonly SmtpClient _client;

  public Outbox()
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
    Console.WriteLine($"host: {host}, user: {user}, pw: {pw}");
    _client = new SmtpClient(host, port);
    _client.Credentials = new NetworkCredential(user, pw);
    _client.UseDefaultCredentials = false;
    _client.EnableSsl = true;
  }

  public async Task<Message> SendNow(Message message)
  {
    var sendMessage = new MailMessage{
      IsBodyHtml = true,
      Subject = message.Subject,
      Body = message.Html,
      From=new MailAddress(message.SendFrom)

    };
    
    sendMessage.To.Add(message.SendTo);

    var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
     //ONLY SEND if we are in production or integration mode
    if (env == "Integration"){
      Console.WriteLine("Sending email");
      await _client.SendMailAsync(sendMessage);
      //create a JSON document
      var json = JsonSerializer.Serialize(new {thing="stuff"}); //TODO: make this a real receipt
      message.Receipt = json;
    }
    message.SentAt = DateTime.Now;
    return message;
  }

}