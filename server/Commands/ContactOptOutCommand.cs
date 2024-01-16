using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Tailwind.Data;

namespace Tailwind.Mail.Commands;

public class ContactOptOutCommand{
  public string? Email { get; set; }
  public ContactOptOutCommand(string email)
  {
    Email = email;
  }
  public CommandResult Execute(){
    
    using(var cmd = new Command()){
      
      var contact = new Query().First("mail.contacts", new{
        email = Email
      });

      if(contact == null){
        return new CommandResult{
          Data = new{
            Success = false,
            Message = "Contact not found"
          }
        };
      }

      cmd.Update("mail.contacts", new{subscribed = false}, new{
        id = contact.id
      });
      cmd.Insert("mail.activity", new{
        contact_id = contact.id,
        key = "optout",
        description="Unsubbed",
      });
      return new CommandResult{
        Updated = 1,
        Data = new{
          Success = true,
          Message = "Contact unsubscribed"
        }
      };
    }
  }
}