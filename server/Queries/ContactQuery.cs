using Tailwind.Mail.Models;
using Tailwind.Data;

namespace Tailwind.Mail.Queries;

public class ContactQuery {
  public string? Email { get; set; }
  public int? ID { get; set; }
  public string? Key { get; set; }
  public bool Subscribed { get; set; } = true;
  public string? Tag { get; set; }

  public Contact First(){
    var res = new Query().First("mail.contacts", new{
      id = ID,
      email = Email,
      key = Key,
      tag = Tag
    });
     
    return new Contact{
      ID = res.id,
      Name = res.name,
      Email = res.email,
      Subscribed = res.subscribed,
      Key = res.key,
      CreatedAt = res.created_at
    };
  }
}