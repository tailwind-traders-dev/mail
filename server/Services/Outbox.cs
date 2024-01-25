namespace Tailwind.Mail.Services;
using System.Net.Mail;
using System.Net;
using System.Text.Json;
using System.Text.Encodings;
using Tailwind.Mail.Models;
using Microsoft.EntityFrameworkCore;
using Humanizer;

public class Outbox 
{


  public static async Task<Message> Send(Message mssg)
  {
    if(!mssg.ReadyToSend()){
      throw new InvalidOperationException("Message is not ready to send. Be sure status is pending, Send At is now or in the past, and all fields are filled out.");
    }
    var config = Viper.Config();
    var env = config.Get("ASPNETCORE_ENVIRONMENT");
     
    var host = config.Get("SMTP_HOST");
    var user = config.Get("SMTP_USER");
    var pw = config.Get("SMTP_PASSWORD");
    var port = 465;
    if (env == "Integration"){
      host="smtp.ethereal.email";
      user = config.Get("ETHEREAL_USER");
      pw = config.Get("ETHEREAL_PASSWORD");
      port = 587;
    }
    
    var sendMessage = new MailMessage{
      IsBodyHtml = true,
      Subject = mssg.Subject,
      Body = mssg.Html,
      From=new MailAddress(mssg.SendFrom),
      To = {new MailAddress(mssg.SendTo)}
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

    mssg.Status = "sent";
    mssg.SentAt = DateTimeOffset.Now;
    return mssg;
  }

}