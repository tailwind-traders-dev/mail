using Xunit;
using Tailwind.Data;
using Markdig;

namespace Tailwind.Mail.Tests;

public class BroadCastTestCommand{
  
  public string Html { get; set; }
  public string Markdown { get; set; }
  public string SendToTag { get; set; }
  public string Slug { get; set; }
  public string Subject { get; set; }

  public BroadCastTestCommand(string slug, string markdown, string subject, string send_to_tag)
  {
    var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
    this.Markdown = markdown;
    this.Html = Markdig.Markdown.ToHtml(markdown, pipeline);
    this.SendToTag = send_to_tag;
    this.Slug = slug;
    this.Subject = subject;
  }
  public async Task<CommandResult> Execute(){

    
    using(var cmd = new Command()){
      
      cmd.Raw("delete from mail.tagged");
      cmd.Raw("delete from mail.tags");
      cmd.Raw("delete from mail.contacts");
      cmd.Raw("delete from mail.broadcasts");
      cmd.Raw("delete from mail.emails");

      var tagId = cmd.Insert("mail.tags", new{
        slug = SendToTag.ToLower(),
        name = SendToTag
      });

      //create 100 contacts
      for(var i = 0; i < 100; i++){
        var cid = cmd.Insert("mail.contacts", new{
          name = $"Test User {i}",
          email = $"test{i}@test.com"
        });
        //tag them
        cmd.Raw("insert into mail.tagged(contact_id, tag_id) values (@contact_id, @tag_id)", new{
          contact_id = cid,
          tag_id = tagId
        });
      }


      //create the email
      var emailId = cmd.Insert("mail.emails", new{
        slug = "test-email",
        subject = "Test Email",
        markdown = this.Markdown,
        html = this.Html
      });


      var broacastId = cmd.Insert("mail.broadcasts", new{
        email_id = emailId,
        slug = "test-broadcast",
        name = "Test Broadcast",
      });

      
      //create the messages
      var sql = @"
        insert into mail.messages (source, slug, send_to, send_from, subject, html, send_at)
        select 'broadcast', @slug, mail.contacts.email, @reply_to, @subject, @html, now() 
        from mail.contacts
        inner join mail.tagged on mail.tagged.contact_id = mail.contacts.id
        inner join mail.tags on mail.tags.id = mail.tagged.tag_id
        where subscribed = true
        and tags.id = @tagId;
      ";

      cmd.Raw(sql, new{
        broacastId,
        tagId,
        slug = Slug,
        reply_to = Viper.Config().Get("DEFAULT_FROM"),
        subject = Subject,
        html = Html
      });

      //ping the job
      cmd.Notify("broadcasts", Slug);
      
      return new CommandResult{
        Data = new{
          BroadcastId = broacastId,
          EmailId = emailId,
          ContactsCreated = 100
        }
      };
    };
  }
}

public class Broadcast_Test:TestBase
{
  [Fact]
  public async Task A_broadcast_is_created_and_updated()
  {

    var result = await new BroadCastTestCommand("test-broadcast", "## Test Email", "Test Broadcast", "Test").Execute();
    Assert.Equal(100, result.Data.ContactsCreated);

  }
}