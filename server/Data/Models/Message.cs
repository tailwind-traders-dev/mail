using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace Tailwind.Mail.Data.Models;

public class Message
{
    [Key]
    [Column("id")]
    public int ID { get; set; }
    
    public Email? Email { get; set; }
    [Column("subject")]
    public string? Subject { get; set; }
    [Column("status")]
    public string? Status { get; set; } = "pending";

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
}