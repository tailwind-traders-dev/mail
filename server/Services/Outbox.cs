namespace Tailwind.Mail.Services;
using System.Net.Mail;
using System.Net;
using System.Text.Json;
using System.Text.Encodings;
using Tailwind.Mail.Models;
using Microsoft.EntityFrameworkCore;
using Humanizer;
using Microsoft.VisualBasic;

public interface IEmailSender{
  public Task<Message> Send(Message mssg);
  public Task<IEnumerable<Message>> SendBulk(IEnumerable<Message> mssgs);
}
public class InMemoryEmailSender: IEmailSender{
  public IEnumerable<Message> Sent { get; set; } = new List<Message>();
  public async Task<Message> Send(Message mssg)
  {
    mssg.Sent();
    Sent.Append(mssg);
    return mssg;
  }
  public async Task<IEnumerable<Message>> SendBulk(IEnumerable<Message> mssgs)
  {
    foreach(var mssg in mssgs)
    {
      mssg.Sent();
      Sent.Append(mssg);
    }
    return mssgs;
  }
}
public class MailHogSender: IEmailSender{
  private SmtpClient _client;
  public MailHogSender()
  {
    _client = new SmtpClient("localhost", 1025);
  }
  public async Task<Message> Send(Message mssg)
  {
    var sendMessage = new MailMessage{
      IsBodyHtml = true,
      Subject = mssg.Subject,
      Body = mssg.Html,
      From= new MailAddress(mssg.SendFrom),
    };
    sendMessage.To.Add(new MailAddress(mssg.SendTo));
    await _client.SendMailAsync(sendMessage);
    mssg.Sent();
    return mssg;
  }

  public async Task<IEnumerable<Message>> SendBulk(IEnumerable<Message> mssgs)
  {
    Parallel.ForEach(mssgs, async mssg => {
      //the SMTP client is not reusable and can only send
      //one email at a time, which is OK cause we're going it in parallel
      using var client = new SmtpClient("localhost", 1025);
      var sendMessage = new MailMessage{
        IsBodyHtml = true,
        Subject = mssg.Subject,
        Body = mssg.Html,
        From = new MailAddress(mssg.SendFrom),
      };
      sendMessage.To.Add(new MailAddress(mssg.SendTo));
      await client.SendMailAsync(sendMessage);
      mssg.Sent();
    });
    return mssgs;
  }
}
public class SmtpEmailSender: IEmailSender{
  private SmtpClient _client;
  public SmtpEmailSender()
  {
    var config = Viper.Config();
    var host = config.Get("SMTP_HOST");
    var user = config.Get("SMTP_USER");
    var pw = config.Get("SMTP_PASSWORD");
    var port = 465;
    _client = new SmtpClient(host, port);
    _client.Credentials = new NetworkCredential(user, pw);
    _client.UseDefaultCredentials = false;
    _client.EnableSsl = true;
  }

  public async Task<Message> Send(Message mssg)
  {
    var sendMessage = new MailMessage{
      IsBodyHtml = true,
      Subject = mssg.Subject,
      Body = mssg.Html,
      From=new MailAddress(mssg.SendFrom),
    };
    sendMessage.To.Add(new MailAddress(mssg.SendTo));
    await _client.SendMailAsync(sendMessage);
    mssg.Sent();
    return mssg;
  }
    public async Task<IEnumerable<Message>> SendBulk(IEnumerable<Message> mssgs)
  {
    Parallel.ForEach(mssgs, async mssg => {
      //the SMTP client is not reusable and can only send
      //one email at a time, which is OK cause we're going it in parallel
      using var client = new SmtpClient("localhost", 1025);
      var sendMessage = new MailMessage{
        IsBodyHtml = true,
        Subject = mssg.Subject,
        Body = mssg.Html,
        From = new MailAddress(mssg.SendFrom),
      };
      sendMessage.To.Add(new MailAddress(mssg.SendTo));
      await client.SendMailAsync(sendMessage);
      mssg.Sent();
    });
    return mssgs;
  }
}
