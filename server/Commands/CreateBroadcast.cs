using Tailwind.Data;

namespace Tailwind.Mail.Commands;


public class CreateBroadcast
{
  public string Slug { get; set; }
  public int EmailId { get; set; }
  public int TagId { get; set; }

  public CreateBroadcast(string slug, int emailId, int sendToTagId=0)
  {
    Slug = slug;
    EmailId = emailId;
    TagId = sendToTagId;
  }
  public async Task<CommandResult> Execute(){

    using(var cmd = new Command()){

      //pull the email
      var email = cmd.First("mail.emails", new{
        id = EmailId
      });
      
      if(email == null){
        throw new InvalidOperationException("Email not found");
      }

      //create the broadcast
      var broadcastId = cmd.Insert("mail.broadcasts", new{
        slug = Slug,
        email_id = EmailId,
        name = email.subject
      });

      //create the messages
      var sql = @"
          insert into mail.messages (source, slug, send_to, send_from, subject, html, send_at)
          select 'broadcast', @slug, mail.contacts.email, @reply_to, @subject, @html, now() 
          from mail.contacts
          inner join mail.tagged on mail.tagged.contact_id = mail.contacts.id
          inner join mail.tags on mail.tags.id = mail.tagged.tag_id
          where subscribed = true
        ";
      
      int messagesCreated;

      if(TagId != 0){
        sql += "and tags.id = @tagId";
        messagesCreated = cmd.Exec(sql, new{
          broadcastId,
          TagId,
          slug = Slug,
          reply_to = Viper.Config().Get("DEFAULT_FROM"),
          subject = email.subject,
          html = email.html
        });
      }else{
        messagesCreated = cmd.Exec(sql, new{
          broadcastId,
          slug = Slug,
          reply_to = Viper.Config().Get("DEFAULT_FROM"),
          subject = email.subject,
          html = email.html
        });
      }
Console.WriteLine($"Created {messagesCreated} messages");
      cmd.Notify("broadcasts", Slug);

      return new CommandResult{
        Data = new{
          BroadcastId = broadcastId,
          Notified = true
        },
        Inserted = messagesCreated
      };

    }
  }
}