namespace Tailwind.Mail.Services;
using System.Net.Mail;
using System.Text.Json;
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
    //pull the SMTP stuff from settings
    _client = new SmtpClient("localhost", 587);
  }

  public async Task<Message> SendNow(Message message)
  {
    var sendMessage = new MailMessage();
    
    sendMessage.From = new MailAddress(message.SendFrom);
    sendMessage.To.Add(message.SendTo);
    sendMessage.Subject = message.Subject;
    sendMessage.Body = message.Html;

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