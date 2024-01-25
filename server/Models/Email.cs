namespace Tailwind.Mail.Models;

public class Email{
  public int? ID { get; set; }
  public string Slug { get; set; }
  public string Subject { get; set; }
  public string? Preview { get; set; }
  public int DelayHours { get; set; }=0;
  public string? Markdown { get; set; }
  public string? Html { get; set; }
  public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;
  public Email(string slug, string subject)
  {
    Slug = slug;
    Subject = subject;
  }
}