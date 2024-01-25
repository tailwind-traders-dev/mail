namespace Tailwind.Mail.Models;

public class Broadcast {
  public int? ID { get; set; }
  public Email? Email { get; set; }
  public string Status { get; set; } = "pending";
  public string? Name { get; set; }
  public string? ReplyTo { get; set; }
  public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;
  public Broadcast()
  {
    
  }
}