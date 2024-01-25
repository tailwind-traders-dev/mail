using Xunit;
using Tailwind.Data;
using Tailwind.Mail.Commands;
using Markdig;

namespace Tailwind.Mail.Tests;

//probably end up mocking this at some point
//for now, just use the test db

public class BroadCastTestCommand{
  
  public string Html { get; set; }
  public string Markdown { get; set; }
  public string Subject { get; set; }

  public BroadCastTestCommand(string markdown, string subject)
  {
    var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
    this.Markdown = markdown;
    this.Html = Markdig.Markdown.ToHtml(markdown, pipeline);
    this.Subject = subject;
  }
  public CommandResult Execute(){

    var slug = this.Subject.ToLower().Replace(" ", "-");
    using(var cmd = new Command()){
      
      //create the email
      var emailId = cmd.Insert("mail.emails", new{
        slug,
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

[Collection("Jill Queues a Broadcast")]
public class Jill_Queues_a_Broadcasts:TestBase
{
  [Fact]
  public void Given_there_are_10000_or_more_subbed_contacts()
  {
    var contacts = new Query().Count("mail.contacts", new{
      subscribed = true
    });
    Assert.True(contacts >= 10000);

  }
  [Fact]
  public void A_When_a_broadcast_is_created_for_everyone_there_should_be_10000_messages()
  {
    var testData = new BroadCastTestCommand("## Test Derper", "Test Broadcast").Execute();
    //this tag id will exist if you run the seed.sql file which is what you should be doing
    //the Makefile does it for you just run make
    //That's right. I said Make.
    var result = new CreateBroadcast("test-broadcast", testData.Data.EmailId).Execute();
    Assert.True(result.Inserted >= 10000);
    Assert.Equal(true, result.Data.Notified);
  }
  [Fact]
  public void A_When_a_broadcast_is_created_for_a_tag_there_should_be_10000_messages()
  {
    var testData = new BroadCastTestCommand("## Test Derper Two", "Test Broadcast 2").Execute();
    var result = new CreateBroadcast("test-broadcast-2", testData.Data.EmailId, 1).Execute();
    Assert.True(result.Inserted >= 10000);
    Assert.Equal(true, result.Data.Notified);
  }

} 
[Collection("Jill won't send to unsubbed")]
public class Jill_wont_send_to_unsubbed:TestBase{
  [Fact]
  public void Given_there_is_1_or_more_usub()
  {

    using(var cmd = new Command()){
      cmd.Insert("mail.contacts", new {
        name = "Unsubbed User",
        email = "leavemealone@test.com",
        subscribed = false
      });
    }

    var contacts = new Query().Count("mail.contacts", new{
      subscribed = false
    });
    Assert.True(contacts >= 1);

  }
  [Fact]
  public void A_broadcast_wont_send_to_unsubbed()
  {
    var testData = new BroadCastTestCommand("## Test Derper Three", "Test Broadcast 3").Execute();
    new CreateBroadcast("test-broadcast-3", testData.Data.EmailId).Execute();
    
    var result = new Query().Count("mail.messages", new{
      send_to="leavemealone@test.com"
    });
    Assert.Equal(0, result);
  }
}