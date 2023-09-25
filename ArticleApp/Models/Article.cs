using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace ArticleApp.Models
{
    public class Article
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(32)]
        public string Author { get; set; }

        [Required]
        [MaxLength(96)]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        [DataType(DataType.Date)]
        [Column(TypeName = "Date")]
        public DateTime PublishedOn { get; set; }

        [Required]
        [MaxLength(64)]
        public string? ArticleTagsAsString { get; set; }

        [Required]
        public Category Category { get; set; }

        public int Views { get; set; } = 0;

    }

     public enum Category {
        Food,
        Politics,
        Sports,
        Technology,
        Health,
        Entertainment,
        Travel,
        Business,
    }
}
