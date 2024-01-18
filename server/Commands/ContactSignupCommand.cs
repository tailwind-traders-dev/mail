using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Tailwind.Data;
using Tailwind.Mail.Models;

namespace Tailwind.Mail.Commands;

public class ContactSignupCommand{
  public Contact Contact { get; set; }
  public ContactSignupCommand(Contact contact)
  {
    Contact = contact;
  }
  public CommandResult Execute(){
    
    using(var cmd = new Command()){
      //make sure they're not there already
      var contacts = cmd.Count("mail.contacts", new{
        email = Contact.Email
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
        email = Contact.Email,
        name = Contact.Name,
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