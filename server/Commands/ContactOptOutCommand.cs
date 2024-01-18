using Tailwind.Data;
using Tailwind.Mail.Models;

namespace Tailwind.Mail.Commands;

public class ContactOptOutCommand{
  public Contact Contact { get; set; }
  public ContactOptOutCommand(Contact contact)
  {
    Contact = contact;
  }
  public CommandResult Execute(){
    
    using(var cmd = new Command()){
      
      cmd.Update("mail.contacts", new{subscribed = false}, new{
        id = Contact.ID
      });
      cmd.Insert("mail.activity", new{
        contact_id = Contact.ID,
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