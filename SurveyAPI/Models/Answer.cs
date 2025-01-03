using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using SurveyAPI.Models;

public class Answer
{
    public int Id { get; set; } // Primary Key
    [Required]
    [StringLength(200)]
    public required string Text { get; set; } // Answer Text
    public int QuestionId { get; set; } // Foreign Key
    [JsonIgnore]
    public Question? Question { get; set; } // Navigation Property
}
