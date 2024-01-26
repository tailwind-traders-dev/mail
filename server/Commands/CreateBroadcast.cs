using Tailwind.Data;
using Tailwind.Mail.Models;

namespace Tailwind.Mail.Commands;


public class CreateBroadcast
{
  private Broadcast _broadcast { get; set; }
  public CreateBroadcast(Broadcast broadcast)
  {
    _broadcast = broadcast;
  } 

  public CommandResult Execute(){

    using(var cmd = new Command()){

      //save the email
      var emailId = cmd.Insert("mail.emails", new{
        slug = _broadcast.Email.Slug,
        subject = _broadcast.Email.Subject,
        html = _broadcast.Email.Html
      });

      //create the broadcast
      var broadcastId = cmd.Insert("mail.broadcasts", new{
        slug = _broadcast.Email.Slug,
        email_id = emailId,
        name = _broadcast.Email.Subject
      });

      //create the messages - glorious sql, isn't it?
      //makes me want to write a haiku using copilot, all about SQL:
      //select from where and join
      //insert update delete
      //sql is the best
      var sql = @"
          insert into mail.messages (source, slug, send_to, send_from, subject, html, send_at)
          select 'broadcast', @slug, mail.contacts.email, @reply_to, @subject, @html, now() 
          from mail.contacts
        ";
      
      int messagesCreated;
      var from = Viper.Config().Get("DEFAULT_FROM");
      if(String.IsNullOrEmpty(from)){
        from="noreply@tailwind.dev";
      }

      if(_broadcast.SendToTag != "*"){
        sql += @"
        inner join mail.tagged on mail.tagged.contact_id = mail.contacts.id
        inner join mail.tags on mail.tags.id = mail.tagged.tag_id
        where subscribed = true
        and tags.slug = @tagId
        or tags.name = @tagId
        ";
        Console.WriteLine(sql);
        messagesCreated = cmd.Exec(sql, new{
          broadcastId,
          tagId = _broadcast.SendToTag,
          slug = _broadcast.Email.Slug,
          reply_to = from,
          subject = _broadcast.Email.Subject,
          html = _broadcast.Email.Html
        });
      }else{
        sql+="where subscribed = true";
        messagesCreated = cmd.Exec(sql, new{
          broadcastId,
          slug = _broadcast.Email.Slug,
          reply_to = from,
          subject = _broadcast.Email.Subject,
          html = _broadcast.Email.Html
        });
      }
      
      cmd.Notify("broadcasts", _broadcast.Email.Slug);

      return new CommandResult{
        Data = new{
          BroadcastId = broadcastId,
          EmailId = emailId,
          Notified = true
        },
        Inserted = messagesCreated
      };

    }
  }
}