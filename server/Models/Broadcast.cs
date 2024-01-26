namespace Tailwind.Mail.Models;

//The process of creating a broadcast is:
//The initial data is created first (name, slug)
//The the email and finally the segment to send to, which is done by tag (or not)
//If the initial use case is using a markdown document, then it should contain all 
//that we need

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