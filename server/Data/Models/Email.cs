using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Markdig;
namespace Tailwind.Mail.Data.Models;

public class Email
{
    [Key]
    [Column("id")]
    public int ID { get; set; }
    [Column("slug")]
    public string? Slug { get; set; }
    [Column("subject")]
    public string? Subject { get; set; }
    [Column("markdown")]
    public string? Markdown { get; set; }
    [Column("delay_hours")]  
    public int DelayHours { get; set; }
    [NotMapped]
    public string? Html { get; set; } //this isn't in the DB and is rendered when/where needed
    public Sequence? Sequence { get; set; }
    public ICollection<Message> Messages { get; }

    public void Render(){
      var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
      this.Html = Markdig.Markdown.ToHtml(this.Markdown, pipeline);
    }
}