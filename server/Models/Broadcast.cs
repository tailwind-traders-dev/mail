using Tailwind.Data;
using Tailwind.Mail.Queries;

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
  public string SendToTag { get; set; } = "*";
  public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;
  private Broadcast()
  {
    
  }
  public static Broadcast FromMarkdownEmail(MarkdownEmail doc){
    var broadcast = new Broadcast();
    broadcast.Email = new Email(doc);
    broadcast.Name = broadcast.Email.Subject;
    broadcast.SendToTag = doc.Data.SendToTag;
    return broadcast;
  }
  public static Broadcast FromMarkdown(string markdown){
    var broadcast = new Broadcast();
    var doc = MarkdownEmail.FromString(markdown);
    broadcast.Email = new Email(doc);
    broadcast.Name = broadcast.Email.Subject;
    broadcast.SendToTag = doc.Data.SendToTag;
    return broadcast;
  }
  public Broadcast(string markdownEmailPath)
  {
    var doc = MarkdownEmail.FromFile(markdownEmailPath);
    this.Email = new Email(doc);
    this.Name = this.Email.Subject;
    this.SendToTag = doc.Data.SendToTag;
  }
  public long ContactCount(){
    //do we have a tag?
    long contacts = 0;
    if(SendToTag == "*"){
      contacts = new Query().Count("mail.contacts");
    }else{
      var sql = @"
        select count(1) as count from mail.contacts 
        inner join mail.tagged on mail.tagged.contact_id = mail.contacts.id
        inner join mail.tags on mail.tags.id = mail.tagged.tag_id
        where subscribed = true
        and tags.slug = @tagId
        or tags.name = @tagId
      ";
      var res = new Query().Raw(sql, new{tagId = SendToTag});
      contacts = res.count;
    }
    return contacts;
  }
}