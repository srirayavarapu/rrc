using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TownsApi.Data;
using TownsApi.Models;
using System.Linq;
using System.Linq;
using Dapper;

namespace TownsApi.Controllers
{
    //[Route("towns/api/[controller]")]
    //[ApiController]
    //public class UsersController : Controller
    //{
    //    private readonly TownDBContext _context;
    //    public UsersController(TownDBContext context)
    //    {
    //        _context = context;
    //    }
    //    [HttpGet("/rrc/api/[controller]/[action]")]
    //    [ProducesResponseType(typeof(Towns), StatusCodes.Status200OK)]
    //    [ProducesResponseType(StatusCodes.Status404NotFound)]
    //    public async Task<IActionResult> GetAllUsersData()
    //    {
    //        try
    //        {
    //            var users = await _context.UsersData.ToListAsync();

    //            if (users.Count() <= 0)
    //            {

    //                ResultObject patResult = new ResultObject
    //                {
    //                    Status = false,
    //                    StatusCode = StatusCodes.Status401Unauthorized,
    //                    token = null,
    //                    Message = "No Uer data exists",
    //                    data = null
    //                };
    //                return Ok(patResult);
    //            }


    //            ResultObject patResult1 = new ResultObject
    //            {
    //                Status = true,
    //                StatusCode = StatusCodes.Status200OK,
    //                token = null,
    //                Message = "Data Found",
    //                data = users
    //            };
    //            return Ok(patResult1);

    //        }
    //        catch (Exception ex)
    //        {
    //            ResultObject patResult = new ResultObject
    //            {
    //                Status = true,
    //                StatusCode = StatusCodes.Status422UnprocessableEntity,
    //                token = null,
    //                Message = ex.StackTrace,
    //                data = null
    //            };
    //            return Ok(patResult);
    //        }

    //    }
    //    [HttpGet("/rrc/api/[controller]/[action]")]
    //    public async Task<IActionResult> GetUserDetails(int id)
    //    {

    //        var newUser = await _context.UsersData.Where(u => u.UserId == id).ToListAsync();
    //        if (newUser.Count() == 0)
    //        {
    //            ResultObject patResult = new ResultObject
    //            {
    //                Status = false,
    //                StatusCode = StatusCodes.Status204NoContent,
    //                token = null,
    //                data = null,
    //                Message = "No User exists with this Id"
    //            };
    //            return Ok(patResult);
    //        }

    //        try
    //        {
    //            ResultObject successResult = new ResultObject
    //            {
    //                Status = true,
    //                StatusCode = StatusCodes.Status200OK,
    //                token = null,
    //                data = newUser,
    //                Message = "User Found"
    //            };
    //            return Ok(successResult);

    //        }
    //        catch (DbUpdateConcurrencyException ex)
    //        {
    //            ResultObject patResult = new ResultObject
    //            {
    //                Status = false,
    //                StatusCode = StatusCodes.Status204NoContent,
    //                token = null,
    //                data = null,
    //                Message = ex.Message
    //            };
    //            return Ok(patResult);
    //        }
    //        catch (Exception ex)
    //        {
    //            ResultObject patResult = new ResultObject
    //            {
    //                Status = false,
    //                StatusCode = StatusCodes.Status204NoContent,
    //                token = null,
    //                data = null,
    //                Message = ex.Message
    //            };
    //            return Ok(patResult);
    //        }

    //    }
    //    [HttpPost("/rrc/api/[controller]/[action]")]
    //    public async Task<IActionResult> AddUserData(UsersData con)
    //    {


    //        if (!string.IsNullOrEmpty(con.Username))
    //        {
    //            con.Username = con.Username;
    //        }
    //        else
    //        {
    //            con.Username = "";
    //        }
    //        if (!string.IsNullOrEmpty(con.Password))
    //        {
    //            con.Password = con.Password;
    //        }
    //        else
    //        {
    //            con.Password = "";
    //        }
    //        _context.Add(con);

    //        try
    //        {
    //            await _context.SaveChangesAsync();
    //            ResultObject successResult = new ResultObject
    //            {
    //                Status = true,
    //                StatusCode = StatusCodes.Status200OK,
    //                token = null,
    //                data = con,
    //                Message = "Updated successfully"
    //            };
    //            return Ok(successResult);

    //        }
    //        catch (DbUpdateConcurrencyException ex)
    //        {
    //            ResultObject patResult = new ResultObject
    //            {
    //                Status = false,
    //                StatusCode = StatusCodes.Status204NoContent,
    //                token = null,
    //                data = null,
    //                Message = ex.Message
    //            };
    //            return Ok(patResult);
    //        }
    //        catch (Exception ex)
    //        {
    //            ResultObject patResult = new ResultObject
    //            {
    //                Status = false,
    //                StatusCode = StatusCodes.Status204NoContent,
    //                token = null,
    //                data = null,
    //                Message = ex.Message
    //            };
    //            return Ok(patResult);
    //        }
    //    }
    //    [HttpDelete("/rrc/api/[controller]/[action]/{id}")]
    //    public async Task<IActionResult> DeleteUserData(int id)
    //    {
    //        var user = await _context.UsersData.FindAsync(id);
    //        if (user == null)
    //        {
    //            return NotFound();
    //        }

    //        _context.UsersData.Remove(user);
    //        await _context.SaveChangesAsync();

    //        return NoContent();
    //    }
    //    [HttpPut("/rrc/api/[controller]/[action]/{id}")]
    //    public async Task<IActionResult> PutUser(int id, UsersData user)
    //    {
    //        if (id != user.UserId)
    //        {
    //            return BadRequest();
    //        }

    //        _context.Entry(user).State = EntityState.Modified;

    //        try
    //        {
    //            await _context.SaveChangesAsync();
    //        }
    //        catch (DbUpdateConcurrencyException)
    //        {
    //        }

    //        return NoContent();
    //    }

    //    [HttpGet("/rrc/api/[controller]/[action]")]
    //    [ProducesResponseType(typeof(Towns), StatusCodes.Status200OK)]
    //    [ProducesResponseType(StatusCodes.Status404NotFound)]
    //    public async Task<IActionResult> GetAllSurveyData()
    //    {
    //        try
    //        {
    //            string ConnectionString = "Server=tcp:rrcupdated.database.windows.net,1433;Initial Catalog=RRC_Town1;Persist Security Info=False;User ID=rrcadmin;Password=Happy@1234A;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=100;;";

    //            var connection = new Microsoft.Data.SqlClient.SqlConnection(ConnectionString);
    //            var outputList = connection.Query<QuestionsData>("SELECT * FROM QuestionsData").ToList();
    //            var choices = connection.Query<ChoicesData>("SELECT * FROM ChoicesData").ToList();
    //            var users = await _context.SurveyHeadersData.ToListAsync();
               
    //            foreach(var data in users)
    //            {
    //               data.Questions=outputList.Where(x=>x.SurveyHeaderId==data.SurveyHeaderId).ToList();
    //            }
    //            foreach (var data in outputList)
    //            {
    //                data.Choices = choices.Where(x => x.QuestionId == data.QuestionId).ToList();
    //            }

    //            if (users.Count() <= 0)
    //            {

    //                ResultObject patResult = new ResultObject
    //                {
    //                    Status = false,
    //                    StatusCode = StatusCodes.Status401Unauthorized,
    //                    token = null,
    //                    Message = "No Survey data exists",
    //                    data = null
    //                };
    //                return Ok(patResult);
    //            }


    //            ResultObject patResult1 = new ResultObject
    //            {
    //                Status = true,
    //                StatusCode = StatusCodes.Status200OK,
    //                token = null,
    //                Message = "Surveys Found",
    //                data = users
    //            };
    //            return Ok(patResult1);

    //        }
    //        catch (Exception ex)
    //        {
    //            ResultObject patResult = new ResultObject
    //            {
    //                Status = true,
    //                StatusCode = StatusCodes.Status422UnprocessableEntity,
    //                token = null,
    //                Message = ex.StackTrace,
    //                data = null
    //            };
    //            return Ok(patResult);
    //        }

    //    }
    //    [HttpGet("/rrc/api/[controller]/[action]")]
    //    public async Task<IActionResult> GetSurveyDataDetails(int id)
    //    {

    //        var newUser = await _context.SurveyHeadersData.Where(u => u.SurveyHeaderId == id).ToListAsync();
    //        if (newUser.Count() == 0)
    //        {
    //            ResultObject patResult = new ResultObject
    //            {
    //                Status = false,
    //                StatusCode = StatusCodes.Status204NoContent,
    //                token = null,
    //                data = null,
    //                Message = "No Survey dats exists with this Id"
    //            };
    //            return Ok(patResult);
    //        }

    //        try
    //        {
    //            ResultObject successResult = new ResultObject
    //            {
    //                Status = true,
    //                StatusCode = StatusCodes.Status200OK,
    //                token = null,
    //                data = newUser,
    //                Message = "Survey Found"
    //            };
    //            return Ok(successResult);

    //        }
    //        catch (DbUpdateConcurrencyException ex)
    //        {
    //            ResultObject patResult = new ResultObject
    //            {
    //                Status = false,
    //                StatusCode = StatusCodes.Status204NoContent,
    //                token = null,
    //                data = null,
    //                Message = ex.Message
    //            };
    //            return Ok(patResult);
    //        }
    //        catch (Exception ex)
    //        {
    //            ResultObject patResult = new ResultObject
    //            {
    //                Status = false,
    //                StatusCode = StatusCodes.Status204NoContent,
    //                token = null,
    //                data = null,
    //                Message = ex.Message
    //            };
    //            return Ok(patResult);
    //        }

    //    }
    //    [HttpPost("/rrc/api/[controller]/[action]")]
    //    public async Task<IActionResult> AddSurveyData(SurveyHeadersData con)
    //    {

    //        if (con.UserId == 0)
    //        {
    //            ResultObject successResult = new ResultObject
    //            {
    //                Status = false,
    //                StatusCode = StatusCodes.Status200OK,
    //                token = null,
    //                data = con,
    //                Message = "Please enter user id"
    //            };
    //            return Ok(successResult);
    //        }
    //        if (!string.IsNullOrEmpty(con.Title))
    //        {
    //            con.Title = con.Title;
    //        }
    //        else
    //        {
    //            con.Title = "";
    //        }
    //        if (!string.IsNullOrEmpty(con.Description))
    //        {
    //            con.Description = con.Description;
    //        }
    //        else
    //        {
    //            con.Description = "";
    //        }
    //        _context.Add(con);

    //        try
    //        {
    //            await _context.SaveChangesAsync();
    //            ResultObject successResult = new ResultObject
    //            {
    //                Status = true,
    //                StatusCode = StatusCodes.Status200OK,
    //                token = null,
    //                data = con,
    //                Message = "Added successfully"
    //            };
    //            return Ok(successResult);

    //        }
    //        catch (DbUpdateConcurrencyException ex)
    //        {
    //            ResultObject patResult = new ResultObject
    //            {
    //                Status = false,
    //                StatusCode = StatusCodes.Status204NoContent,
    //                token = null,
    //                data = null,
    //                Message = ex.Message
    //            };
    //            return Ok(patResult);
    //        }
    //        catch (Exception ex)
    //        {
    //            ResultObject patResult = new ResultObject
    //            {
    //                Status = false,
    //                StatusCode = StatusCodes.Status204NoContent,
    //                token = null,
    //                data = null,
    //                Message = ex.Message
    //            };
    //            return Ok(patResult);
    //        }
    //    }
    //    [HttpDelete("/rrc/api/[controller]/[action]/{id}")]
    //    public async Task<IActionResult> DeleteSurveyData(int id)
    //    {
    //        var user = await _context.SurveyHeadersData.FindAsync(id);
    //        if (user == null)
    //        {
    //            return NotFound();
    //        }

    //        _context.SurveyHeadersData.Remove(user);
    //        await _context.SaveChangesAsync();

    //        return NoContent();
    //    }
    //    [HttpPut("/rrc/api/[controller]/[action]/{id}")]
    //    public async Task<IActionResult> PutSurveyData(int id, SurveyHeadersData user)
    //    {
    //        if (id != user.SurveyHeaderId)
    //        {
    //            return BadRequest();
    //        }

    //        _context.Entry(user).State = EntityState.Modified;

    //        try
    //        {
    //            await _context.SaveChangesAsync();
    //        }
    //        catch (DbUpdateConcurrencyException)
    //        {
    //        }

    //        return NoContent();
    //    }

    //    [HttpGet("/rrc/api/[controller]/[action]")]
    //    [ProducesResponseType(typeof(Towns), StatusCodes.Status200OK)]
    //    [ProducesResponseType(StatusCodes.Status404NotFound)]
    //    public async Task<IActionResult> GetAllQuestionsData()
    //    {
    //        try
    //        {
    //            var users = await _context.QuestionsData.ToListAsync();

    //            if (users.Count() <= 0)
    //            {

    //                ResultObject patResult = new ResultObject
    //                {
    //                    Status = false,
    //                    StatusCode = StatusCodes.Status401Unauthorized,
    //                    token = null,
    //                    Message = "No Question data exists",
    //                    data = null
    //                };
    //                return Ok(patResult);
    //            }


    //            ResultObject patResult1 = new ResultObject
    //            {
    //                Status = true,
    //                StatusCode = StatusCodes.Status200OK,
    //                token = null,
    //                Message = "Questions Found",
    //                data = users
    //            };
    //            return Ok(patResult1);

    //        }
    //        catch (Exception ex)
    //        {
    //            ResultObject patResult = new ResultObject
    //            {
    //                Status = true,
    //                StatusCode = StatusCodes.Status422UnprocessableEntity,
    //                token = null,
    //                Message = ex.StackTrace,
    //                data = null
    //            };
    //            return Ok(patResult);
    //        }

    //    }
    //    [HttpGet("/rrc/api/[controller]/[action]")]
    //    public async Task<IActionResult> GetQuestionDataDetails(int id)
    //    {

    //        var newUser = await _context.QuestionsData.Where(u => u.QuestionId == id).ToListAsync();
    //        if (newUser.Count() == 0)
    //        {
    //            ResultObject patResult = new ResultObject
    //            {
    //                Status = false,
    //                StatusCode = StatusCodes.Status204NoContent,
    //                token = null,
    //                data = null,
    //                Message = "No Question data exists with this Id"
    //            };
    //            return Ok(patResult);
    //        }

    //        try
    //        {
    //            ResultObject successResult = new ResultObject
    //            {
    //                Status = true,
    //                StatusCode = StatusCodes.Status200OK,
    //                token = null,
    //                data = newUser,
    //                Message = "Question Found"
    //            };
    //            return Ok(successResult);

    //        }
    //        catch (DbUpdateConcurrencyException ex)
    //        {
    //            ResultObject patResult = new ResultObject
    //            {
    //                Status = false,
    //                StatusCode = StatusCodes.Status204NoContent,
    //                token = null,
    //                data = null,
    //                Message = ex.Message
    //            };
    //            return Ok(patResult);
    //        }
    //        catch (Exception ex)
    //        {
    //            ResultObject patResult = new ResultObject
    //            {
    //                Status = false,
    //                StatusCode = StatusCodes.Status204NoContent,
    //                token = null,
    //                data = null,
    //                Message = ex.Message
    //            };
    //            return Ok(patResult);
    //        }

    //    }
    //    [HttpPost("/rrc/api/[controller]/[action]")]
    //    public async Task<IActionResult> AddQuestionData(QuestionsData con)
    //    {

    //        if (con.SurveyHeaderId == 0)
    //        {
    //            ResultObject successResult = new ResultObject
    //            {
    //                Status = false,
    //                StatusCode = StatusCodes.Status200OK,
    //                token = null,
    //                data = con,
    //                Message = "Please enter Survey Header id"
    //            };
    //            return Ok(successResult);
    //        }
    //        if (!string.IsNullOrEmpty(con.Text))
    //        {
    //            con.Text = con.Text;
    //        }
    //        else
    //        {
    //            con.Text = "";
    //        }

    //        _context.QuestionsData.Add(con);

    //        try
    //        {
    //            await _context.SaveChangesAsync();
    //            ResultObject successResult = new ResultObject
    //            {
    //                Status = true,
    //                StatusCode = StatusCodes.Status200OK,
    //                token = null,
    //                data = con,
    //                Message = "Added successfully"
    //            };
    //            return Ok(successResult);

    //        }
    //        catch (DbUpdateConcurrencyException ex)
    //        {
    //            ResultObject patResult = new ResultObject
    //            {
    //                Status = false,
    //                StatusCode = StatusCodes.Status204NoContent,
    //                token = null,
    //                data = null,
    //                Message = ex.Message
    //            };
    //            return Ok(patResult);
    //        }
    //        catch (Exception ex)
    //        {
    //            ResultObject patResult = new ResultObject
    //            {
    //                Status = false,
    //                StatusCode = StatusCodes.Status204NoContent,
    //                token = null,
    //                data = null,
    //                Message = ex.Message
    //            };
    //            return Ok(patResult);
    //        }
    //    }
    //    [HttpDelete("/rrc/api/[controller]/[action]/{id}")]
    //    public async Task<IActionResult> DeleteQuestionData(int id)
    //    {
    //        var user = await _context.QuestionsData.FindAsync(id);
    //        if (user == null)
    //        {
    //            return NotFound();
    //        }

    //        _context.QuestionsData.Remove(user);
    //        await _context.SaveChangesAsync();

    //        return NoContent();
    //    }
    //    [HttpPut("/rrc/api/[controller]/[action]/{id}")]
    //    public async Task<IActionResult> PutQuestionData(int id, QuestionsData user)
    //    {
    //        if (id != user.QuestionId)
    //        {
    //            return BadRequest();
    //        }

    //        _context.Entry(user).State = EntityState.Modified;

    //        try
    //        {
    //            await _context.SaveChangesAsync();
    //        }
    //        catch (DbUpdateConcurrencyException)
    //        {
    //        }

    //        return NoContent();
    //    }


    //    [HttpGet("/rrc/api/[controller]/[action]")]
    //    [ProducesResponseType(typeof(Towns), StatusCodes.Status200OK)]
    //    [ProducesResponseType(StatusCodes.Status404NotFound)]
    //    public async Task<IActionResult> GetAllChoicesData()
    //    {
    //        try
    //        {
    //            var users = await _context.ChoicesData.ToListAsync();

    //            if (users.Count() <= 0)
    //            {

    //                ResultObject patResult = new ResultObject
    //                {
    //                    Status = false,
    //                    StatusCode = StatusCodes.Status401Unauthorized,
    //                    token = null,
    //                    Message = "No Choices data exists",
    //                    data = null
    //                };
    //                return Ok(patResult);
    //            }


    //            ResultObject patResult1 = new ResultObject
    //            {
    //                Status = true,
    //                StatusCode = StatusCodes.Status200OK,
    //                token = null,
    //                Message = "Choices Found",
    //                data = users
    //            };
    //            return Ok(patResult1);

    //        }
    //        catch (Exception ex)
    //        {
    //            ResultObject patResult = new ResultObject
    //            {
    //                Status = true,
    //                StatusCode = StatusCodes.Status422UnprocessableEntity,
    //                token = null,
    //                Message = ex.StackTrace,
    //                data = null
    //            };
    //            return Ok(patResult);
    //        }

    //    }
    //    [HttpGet("/rrc/api/[controller]/[action]")]
    //    public async Task<IActionResult> GetChoicesDataDetails(int id)
    //    {

    //        var newUser = await _context.ChoicesData.Where(u => u.ChoiceId == id).ToListAsync();
    //        if (newUser.Count() == 0)
    //        {
    //            ResultObject patResult = new ResultObject
    //            {
    //                Status = false,
    //                StatusCode = StatusCodes.Status204NoContent,
    //                token = null,
    //                data = null,
    //                Message = "No Choices data exists with this Id"
    //            };
    //            return Ok(patResult);
    //        }

    //        try
    //        {
    //            ResultObject successResult = new ResultObject
    //            {
    //                Status = true,
    //                StatusCode = StatusCodes.Status200OK,
    //                token = null,
    //                data = newUser,
    //                Message = "Choices Found"
    //            };
    //            return Ok(successResult);

    //        }
    //        catch (DbUpdateConcurrencyException ex)
    //        {
    //            ResultObject patResult = new ResultObject
    //            {
    //                Status = false,
    //                StatusCode = StatusCodes.Status204NoContent,
    //                token = null,
    //                data = null,
    //                Message = ex.Message
    //            };
    //            return Ok(patResult);
    //        }
    //        catch (Exception ex)
    //        {
    //            ResultObject patResult = new ResultObject
    //            {
    //                Status = false,
    //                StatusCode = StatusCodes.Status204NoContent,
    //                token = null,
    //                data = null,
    //                Message = ex.Message
    //            };
    //            return Ok(patResult);
    //        }

    //    }
    //    [HttpPost("/rrc/api/[controller]/[action]")]
    //    public async Task<IActionResult> AddChoicesData(ChoicesData con)
    //    {

    //        if (con.QuestionId == 0)
    //        {
    //            ResultObject successResult = new ResultObject
    //            {
    //                Status = false,
    //                StatusCode = StatusCodes.Status200OK,
    //                token = null,
    //                data = con,
    //                Message = "Please enter Question id"
    //            };
    //            return Ok(successResult);
    //        }
    //        if (!string.IsNullOrEmpty(con.ChoiceText))
    //        {
    //            con.ChoiceText = con.ChoiceText;
    //        }
    //        else
    //        {
    //            con.ChoiceText = "";
    //        }

    //        _context.ChoicesData.Add(con);

    //        try
    //        {
    //            await _context.SaveChangesAsync();
    //            ResultObject successResult = new ResultObject
    //            {
    //                Status = true,
    //                StatusCode = StatusCodes.Status200OK,
    //                token = null,
    //                data = con,
    //                Message = "Added successfully"
    //            };
    //            return Ok(successResult);

    //        }
    //        catch (DbUpdateConcurrencyException ex)
    //        {
    //            ResultObject patResult = new ResultObject
    //            {
    //                Status = false,
    //                StatusCode = StatusCodes.Status204NoContent,
    //                token = null,
    //                data = null,
    //                Message = ex.Message
    //            };
    //            return Ok(patResult);
    //        }
    //        catch (Exception ex)
    //        {
    //            ResultObject patResult = new ResultObject
    //            {
    //                Status = false,
    //                StatusCode = StatusCodes.Status204NoContent,
    //                token = null,
    //                data = null,
    //                Message = ex.Message
    //            };
    //            return Ok(patResult);
    //        }
    //    }
    //    [HttpDelete("/rrc/api/[controller]/[action]/{id}")]
    //    public async Task<IActionResult> DeleteChoicesData(int id)
    //    {
    //        var user = await _context.ChoicesData.FindAsync(id);
    //        if (user == null)
    //        {
    //            return NotFound();
    //        }

    //        _context.ChoicesData.Remove(user);
    //        await _context.SaveChangesAsync();

    //        return NoContent();
    //    }
    //    [HttpPut("/rrc/api/[controller]/[action]/{id}")]
    //    public async Task<IActionResult> PutChoicesData(int id, ChoicesData user)
    //    {
    //        if (id != user.ChoiceId)
    //        {
    //            return BadRequest();
    //        }

    //        _context.Entry(user).State = EntityState.Modified;

    //        try
    //        {
    //            await _context.SaveChangesAsync();
    //        }
    //        catch (DbUpdateConcurrencyException)
    //        {
    //        }

    //        return NoContent();
    //    }

    //    [HttpGet("/rrc/api/[controller]/[action]")]
    //    [ProducesResponseType(typeof(Towns), StatusCodes.Status200OK)]
    //    [ProducesResponseType(StatusCodes.Status404NotFound)]
    //    public async Task<IActionResult> GetAllResponsesData()
    //    {
    //        try
    //        {
    //            var users = await _context.ResponsesData.ToListAsync();

    //            if (users.Count() <= 0)
    //            {

    //                ResultObject patResult = new ResultObject
    //                {
    //                    Status = false,
    //                    StatusCode = StatusCodes.Status401Unauthorized,
    //                    token = null,
    //                    Message = "No Responses Data exists",
    //                    data = null
    //                };
    //                return Ok(patResult);
    //            }


    //            ResultObject patResult1 = new ResultObject
    //            {
    //                Status = true,
    //                StatusCode = StatusCodes.Status200OK,
    //                token = null,
    //                Message = "Responses Found",
    //                data = users
    //            };
    //            return Ok(patResult1);

    //        }
    //        catch (Exception ex)
    //        {
    //            ResultObject patResult = new ResultObject
    //            {
    //                Status = true,
    //                StatusCode = StatusCodes.Status422UnprocessableEntity,
    //                token = null,
    //                Message = ex.StackTrace,
    //                data = null
    //            };
    //            return Ok(patResult);
    //        }

    //    }
    //    [HttpGet("/rrc/api/[controller]/[action]")]
    //    public async Task<IActionResult> GetResponseDataDetails(int id)
    //    {

    //        var newUser = await _context.ChoicesData.Where(u => u.ChoiceId == id).ToListAsync();
    //        if (newUser.Count() == 0)
    //        {
    //            ResultObject patResult = new ResultObject
    //            {
    //                Status = false,
    //                StatusCode = StatusCodes.Status204NoContent,
    //                token = null,
    //                data = null,
    //                Message = "No Response data exists with this Id"
    //            };
    //            return Ok(patResult);
    //        }

    //        try
    //        {
    //            ResultObject successResult = new ResultObject
    //            {
    //                Status = true,
    //                StatusCode = StatusCodes.Status200OK,
    //                token = null,
    //                data = newUser,
    //                Message = "Response Found"
    //            };
    //            return Ok(successResult);

    //        }
    //        catch (DbUpdateConcurrencyException ex)
    //        {
    //            ResultObject patResult = new ResultObject
    //            {
    //                Status = false,
    //                StatusCode = StatusCodes.Status204NoContent,
    //                token = null,
    //                data = null,
    //                Message = ex.Message
    //            };
    //            return Ok(patResult);
    //        }
    //        catch (Exception ex)
    //        {
    //            ResultObject patResult = new ResultObject
    //            {
    //                Status = false,
    //                StatusCode = StatusCodes.Status204NoContent,
    //                token = null,
    //                data = null,
    //                Message = ex.Message
    //            };
    //            return Ok(patResult);
    //        }

    //    }
    //    [HttpPost("/rrc/api/[controller]/[action]")]
    //    public async Task<IActionResult> AddResponsesData(ResponsesData con)
    //    {

    //        if (con.QuestionId == 0)
    //        {
    //            ResultObject successResult = new ResultObject
    //            {
    //                Status = false,
    //                StatusCode = StatusCodes.Status200OK,
    //                token = null,
    //                data = con,
    //                Message = "Please enter Response id"
    //            };
    //            return Ok(successResult);
    //        }
    //        if (!string.IsNullOrEmpty(con.Answer))
    //        {
    //            con.Answer = con.Answer;
    //        }
    //        else
    //        {
    //            con.Answer = "";
    //        }

    //        _context.ResponsesData.Add(con);

    //        try
    //        {
    //            await _context.SaveChangesAsync();
    //            ResultObject successResult = new ResultObject
    //            {
    //                Status = true,
    //                StatusCode = StatusCodes.Status200OK,
    //                token = null,
    //                data = con,
    //                Message = "Added successfully"
    //            };
    //            return Ok(successResult);

    //        }
    //        catch (DbUpdateConcurrencyException ex)
    //        {
    //            ResultObject patResult = new ResultObject
    //            {
    //                Status = false,
    //                StatusCode = StatusCodes.Status204NoContent,
    //                token = null,
    //                data = null,
    //                Message = ex.Message
    //            };
    //            return Ok(patResult);
    //        }
    //        catch (Exception ex)
    //        {
    //            ResultObject patResult = new ResultObject
    //            {
    //                Status = false,
    //                StatusCode = StatusCodes.Status204NoContent,
    //                token = null,
    //                data = null,
    //                Message = ex.Message
    //            };
    //            return Ok(patResult);
    //        }
    //    }
    //    [HttpDelete("/rrc/api/[controller]/[action]/{id}")]
    //    public async Task<IActionResult> DeleteResponseData(int id)
    //    {
    //        var user = await _context.ResponsesData.FindAsync(id);
    //        if (user == null)
    //        {
    //            return NotFound();
    //        }

    //        _context.ResponsesData.Remove(user);
    //        await _context.SaveChangesAsync();

    //        return NoContent();
    //    }
    //    [HttpPut("/rrc/api/[controller]/[action]/{id}")]
    //    public async Task<IActionResult> PutResponsesData(int id, ResponsesData user)
    //    {
    //        if (id != user.ResponseId)
    //        {
    //            return BadRequest();
    //        }

    //        _context.Entry(user).State = EntityState.Modified;

    //        try
    //        {
    //            await _context.SaveChangesAsync();
    //        }
    //        catch (DbUpdateConcurrencyException)
    //        {
    //        }

    //        return NoContent();
    //    }
    //}
}
