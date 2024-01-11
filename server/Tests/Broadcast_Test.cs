using Xunit;
using Tailwind.Data;
using Tailwind.Mail.Commands;
using Markdig;

namespace Tailwind.Mail.Tests;

public class BroadCastTestCommand{
  
  public string Html { get; set; }
  public string Markdown { get; set; }
  public string SendToTag { get; set; }
  public string Slug { get; set; }
  public string Subject { get; set; }

  public BroadCastTestCommand(string markdown, string subject, string send_to_tag)
  {
    var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
    this.Markdown = markdown;
    this.Html = Markdig.Markdown.ToHtml(markdown, pipeline);
    this.SendToTag = send_to_tag;
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

      
      return new CommandResult{
        Data = new{
          EmailId = emailId,
          TagId = tagId,
          ContactsCreated = 100,
          TablesEmpty = true
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

    var testData = await new BroadCastTestCommand("## Test Derper", "Test Broadcast", "Test").Execute();
    var result = await new CreateBroadcast("test-broadcast", testData.Data.EmailId, testData.Data.TagId).Execute();
    
    Console.WriteLine(result.Data);
    Assert.Equal(100, result.Inserted);
    Assert.Equal(true, result.Data.Notified);

  }
}