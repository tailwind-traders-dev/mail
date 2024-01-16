using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Tailwind.Data;

namespace Tailwind.Mail.Commands;

public class ContactSignupCommand{
  public string? Email { get; set; }
  public string? Name { get; set; }
  public ContactSignupCommand(string email, string name)
  {
    Email = email;
    Name = name; 
  }
  public CommandResult Execute(){
    
    using(var cmd = new Command()){
      //make sure they're not there already
      var contacts = cmd.Count("mail.contacts", new{
        email = Email
      });

      if(contacts > 0){
        return new CommandResult{
          Data = new{
            Success=false,
            Message = "User exists"
          }
        };
      }  

      var contactId = cmd.Insert("mail.contacts", new{
        email = Email,
        name = Name,
        subscribed = false
      });

      cmd.Insert("mail.activity", new{
        contact_id = contactId,
        key = "signup",
        description="Test sign up",
      });

      //pull the contact back out so we can get the KEY, which
      //will serve as the optin link
      dynamic contact = cmd.First("mail.contacts", new{
        id = contactId
      });
      return new CommandResult{
        Inserted = 1,
        Data = new {
          Success=true,
          Key=contact.key,
          Subscribed = contact.subscribed
        }
      };
    }
  }
}