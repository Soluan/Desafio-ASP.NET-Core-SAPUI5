using System.ComponentModel.DataAnnotations;

namespace MyApp.Models
{
    public class User
    {
        [Key]
        public int ID { get; set; }
        public int UserID { get; set; }
        public string Title { get; set; } = string.Empty;
        public bool Completed { get; set; }
    }
}