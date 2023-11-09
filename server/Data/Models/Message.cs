using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace Tailwind.Mail.Data.Models;

public class Message
{
    [Key]
    [Column("id")]
    public int ID { get; set; }

    [Column("subject")]
    public string? Subject { get; set; }
    [Column("status")]
    public string? Status { get; set; } = "pending";
    [Column("source")]
    public string? Source { get; set; } = "broadcast";
    [Column("slug")]
    public string? Slug { get; set; }

    [Column("send_to")]
    public string? SendTo { get; set; } 
    [Column("send_from")]
    public string? SendFrom { get; set; } 
    [Column("html")]
    public string? Html { get; set; }
    
    [Column("send_at")]
    public DateTimeOffset? SendAt { get; set; }
    [Column("sent_at")]
    public DateTimeOffset? SentAt { get; set; }
    [Column("created_at")]
    public DateTimeOffset? CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public static IList<Message> CreateForBroadcast(Broadcast broadcast, ICollection<Contact> contacts){
      var _messages = new List<Message>();
      var email = broadcast.Email;
      foreach (var contact in contacts)
      {
        //probably need to do the rendering here so we can 
        //add template variables, like name, data, etc
        var message = new Message{
          Subject = email.Subject,
          Source = "broadcast",
          Slug = broadcast.Slug,
          SendTo = contact.Email,
          SendFrom = broadcast.ReplyTo,
          Status = "queued",
          SendAt = DateTimeOffset.UtcNow + TimeSpan.FromHours(email.DelayHours),
          Html = email.Html,
        };
        _messages.Add(message);
      }
      return _messages;
    }

}