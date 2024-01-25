namespace Tailwind.Mail.Models;

public class Message{
  public int? ID { get; set; }
  public string Source { get; set; } = "broadcast";
  public string? Slug { get; set; }
  public string Status { get; set; } = "pending";
  public string? SendTo { get; set; }
  public string? SendFrom { get; set; }
  public string Subject { get; set; }
  public string Html { get; set; }
  public DateTimeOffset SendAt { get; set; }
  public DateTimeOffset SentAt { get; set; }
  public DateTimeOffset CreatedAt { get; set; }
  public Message()
  {
    
  }
}