using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SurveyAPI.Models
{
    public class Survey
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public required string Title { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        public DateTime CreatedDate { get; set; }

        public required ICollection<Question> Questions { get; set; }
    }
}
