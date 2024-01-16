using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Tailwind.Data;

namespace Tailwind.Mail.Commands;

public class ContactOptinCommand{
  public string? Key { get; set; }
  public ContactOptinCommand(string key)
  {
    Key = key; 
  }
  public CommandResult Execute(){
    
    using(var cmd = new Command()){
      
      var contact = new Query().First("mail.contacts", new{
        key = Key
      });

      if(contact == null){
        return new CommandResult{
          Data = new{
            Success = false,
            Message = "Contact not found"
          }
        };
      }

      cmd.Update("mail.contacts", new{subscribed = true}, new{
        id = contact.id
      });
      cmd.Insert("mail.activity", new{
        contact_id = contact.id,
        key = "optin",
        description="Opted in using key",
      });
      return new CommandResult{
        Updated = 1,
        Data = new{
          Success = true,
          Message = "Contact subscribed"
        }
      };
    }
  }
}