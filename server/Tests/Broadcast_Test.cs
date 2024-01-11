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

  public BroadCastTestCommand(string markdown, string subject)
  {
    var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
    this.Markdown = markdown;
    this.Html = Markdig.Markdown.ToHtml(markdown, pipeline);
    this.Subject = subject;
  }
  public async Task<CommandResult> Execute(){

    
    using(var cmd = new Command()){
      
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
  public async Task A_broadcast_is_created_and_sent_to_a_tag()
  {

    var testData = await new BroadCastTestCommand("## Test Derper", "Test Broadcast").Execute();
    //this tag id will exist if you run the seed.sql file which is what you should be doing
    //the Makefile does it for you just run make
    //That's right. I said Make.
    var result = await new CreateBroadcast("test-broadcast", testData.Data.EmailId, 1).Execute();
    
    Assert.Equal(10000, result.Inserted);
    Assert.Equal(true, result.Data.Notified);

  }
  [Fact]
  public async Task A_broadcast_is_created_and_sent_to_every_sub()
  {
    var q = new Query();

    var emailId = q.First("mail.emails", new{
      slug = "test-email"
    }).id;

    var result = await new CreateBroadcast("test-broadcast-2", emailId).Execute();
    
    Assert.Equal(10000, result.Inserted);

  }
  [Fact]
  public async Task A_broadcast_wont_send_to_unsubbed()
  {
    var emailId = new Query().First("mail.emails", new{
      slug = "test-email"
    }).id;

    using(var cmd = new Command()){
      cmd.Insert("mail.contacts", new {
        name = "Unsubbed User",
        email = "leavemealone@test.com",
        subscribed = false
      });
    }
    //var testData = await new BroadCastTestCommand("## Test Every Sub", "Test Broadcast 2", "Test").Execute();
    var result = await new CreateBroadcast("test-broadcast-3", emailId).Execute();
    
    Assert.Equal(10000, result.Inserted);

  }
}