using Tailwind.Mail.Models;
using Tailwind.Data;

namespace Tailwind.Mail.Commands;

public class ContactOptinCommand{
  public Contact Contact { get; set; }
  public ContactOptinCommand(Contact contact)
  {
    Contact = contact;
  }

  public CommandResult Execute(){
    
    using(var cmd = new Command()){
    
      cmd.Update("mail.contacts", new{subscribed = true}, new{
        id = Contact.ID
      });
      cmd.Insert("mail.activity", new{
        contact_id = Contact.ID,
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