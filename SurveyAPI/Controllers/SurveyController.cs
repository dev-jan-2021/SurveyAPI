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

            // Return the created survey
            return CreatedAtAction(nameof(GetSurvey), new { id = survey.Id }, survey);
        }

        // READ all Surveys
        [HttpGet]
        public async Task<IActionResult> GetSurveys()
        {
            var surveys = await _context.Surveys.Include(s => s.Questions).ToListAsync();
            return Ok(surveys);
        }

        // READ a Survey by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSurvey(int id)
        {
            var survey = await _context.Surveys.Include(s => s.Questions).FirstOrDefaultAsync(s => s.Id == id);
            if (survey == null)
            {
                return NotFound();
            }

            return Ok(survey);
        }

        // UPDATE Survey
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSurvey(int id, [FromBody] Survey survey)
        {
            Survey? objSurvey = await _context.Surveys.FirstOrDefaultAsync(s => s.Id == id);

            if (objSurvey == null) {
                return new NotFoundResult();
            }

            objSurvey.Questions = survey.Questions;
            objSurvey.Title = survey.Title;
            objSurvey.Description = survey.Description;

            _context.SaveChanges();

            return Ok();
        }

        // DELETE Survey
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSurvey(int id)
        {
            var survey = await _context.Surveys.FindAsync(id);
            if (survey == null)
            {
                return NotFound();
            }

            _context.Surveys.Remove(survey);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
