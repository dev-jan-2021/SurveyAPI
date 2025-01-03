using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SurveyAPI.Data;
using SurveyAPI.Models;

namespace SurveyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SurveyController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SurveyController(ApplicationDbContext context)
        {
            _context = context;
        }

        // CREATE Survey
        [HttpPost]
        public async Task<IActionResult> CreateSurvey([FromBody] Survey survey)
        {
            if (survey == null || survey.Questions == null || !survey.Questions.Any())
            {
                return BadRequest("Survey data or questions are missing.");
            }

            // Add the survey to the database
            _context.Surveys.Add(survey);
            await _context.SaveChangesAsync();
            if (survey.Questions != null && survey.Questions.Any())
            {
                foreach (var question in survey.Questions)
                {
                    question.Id = 0;
                    question.SurveyId = survey.Id;

                    if (question.Answers != null && question.Answers.Any())
                    {
                        foreach (var answer in question.Answers)
                        {
                            answer.Id = 0;
                            answer.QuestionId = question.Id;
                            _context.Answers.Add(answer);
                        }
                    }

                    _context.Questions.Add(question);
                }

                await _context.SaveChangesAsync();
            }
            // Return the created survey
            return CreatedAtAction(nameof(GetSurvey), new { id = survey.Id }, survey);
        }

        // READ all Surveys
        [HttpGet]
        public async Task<IActionResult> GetSurveys()
        {
            var surveys = await _context.Surveys.Include(s => s.Questions).ThenInclude(q => q.Answers).ToListAsync();
            return Ok(surveys);
        }

        // READ a Survey by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSurvey(int id)
        {
            var survey = await _context.Surveys.Include(s => s.Questions).ThenInclude(q => q.Answers).FirstOrDefaultAsync(s => s.Id == id);
            if (survey == null)
            {
                return NotFound();
            }

            return Ok(survey);
        }

        // UPDATE Survey
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSurvey(int id, [FromBody] Survey updatedSurvey)
        {
            if (id != updatedSurvey.Id)
            {
                return BadRequest("Survey ID mismatch.");
            }

            var existingSurvey = await _context.Surveys
                .Include(s => s.Questions)
                .ThenInclude(q => q.Answers)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (existingSurvey == null)
            {
                return NotFound("Survey not found.");
            }

            // Update survey fields
            existingSurvey.Title = updatedSurvey.Title;
            existingSurvey.Description = updatedSurvey.Description;

            // Update questions
            foreach (var updatedQuestion in updatedSurvey.Questions)
            {
                var existingQuestion = existingSurvey.Questions
                    .FirstOrDefault(q => q.Id == updatedQuestion.Id);
                
                if (existingQuestion != null)
                {
                    existingQuestion.Text = updatedQuestion.Text;

                    // Update answers
                    foreach (var updatedAnswer in updatedQuestion.Answers)
                    {
                        var existingAnswer = existingQuestion.Answers
                            .FirstOrDefault(a => a.Id == updatedAnswer.Id);
                        if (existingAnswer != null)
                        {
                            existingAnswer.Text = updatedAnswer.Text;
                        }
                        else
                        {
                            existingQuestion.Answers.Add(updatedAnswer);
                        }
                    }
                }
                else
                {
                    updatedQuestion.Id = 0;
                    updatedQuestion.SurveyId = id;
                    existingSurvey.Questions.Add(updatedQuestion);
                }
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE Survey
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSurvey(int id)
        {
            var survey = await _context.Surveys
                .Include(s => s.Questions)
                .ThenInclude(q => q.Answers)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (survey == null)
            {
                return NotFound("Survey not found.");
            }

            // Remove survey and related data
            _context.Surveys.Remove(survey);
            await _context.SaveChangesAsync();
            return NoContent();
        }

    }
}
