using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SurveyAPI.Models
{
    public class Question
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public required string Text { get; set; }

        public int SurveyId { get; set; }

        [JsonIgnore]
        public Survey? Survey { get; set; } 
        public required ICollection<Answer> Answers { get; set; }
    }
}
