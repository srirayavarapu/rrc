﻿using Azure;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using System.Text.Json;
using TownsApi.Data;
using TownsApi.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TownsApi.Controllers
{
    [Route("towns/api/[controller]")]
    [ApiController]
    public class TownsController : Controller
    {
        private TownDBContext _context;
        private readonly IConfiguration _configuration;
        private readonly IConnectionStringProvider _connectionStringProvider;

        public TownsController(TownDBContext context, IConnectionStringProvider connectionStringProvider)
        {
            _context = context;
            _connectionStringProvider = connectionStringProvider;
        }
        [HttpGet("/rrc/api/[controller]/[action]")]
        public async Task<IActionResult> GetSurveysByUser(int empid)
        {
            try
            {
                var _connectionString = _connectionStringProvider.GetConnectionString("RRC_Test");

                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                // Fetch all surveys created by the specified user
                const string surveysQuery = @"
        SELECT Id, Title, Description, CreatedBy
        FROM Surveys
        WHERE CreatedBy = @CreatedBy;
        ";

                var surveys = (await connection.QueryAsync<Survey>(surveysQuery, new { CreatedBy = empid })).ToList();

                if (!surveys.Any())
                {
                    return NotFound($"No surveys found for user with ID {empid}.");
                }

                // Fetch all questions for the retrieved surveys
                var surveyIds = surveys.Select(s => s.Id).ToArray();

                const string questionsQuery = @"
        SELECT Id, Text, Type, SurveyId
        FROM Questions
        WHERE SurveyId IN @SurveyIds;
        ";

                var questions = (await connection.QueryAsync<Question>(questionsQuery, new { SurveyIds = surveyIds })).ToList();

                // Fetch all answers for the retrieved questions
                var questionIds = questions.Select(q => q.Id).ToArray();

                const string answersQuery = @"
        SELECT Id, Text, QuestionId
        FROM Answers
        WHERE QuestionId IN @QuestionIds;
        ";

                var answers = (await connection.QueryAsync<Answer>(answersQuery, new { QuestionIds = questionIds })).ToList();

                // Map questions and answers to their respective surveys
                foreach (var survey in surveys)
                {
                    var surveyQuestions = questions.Where(q => q.SurveyId == survey.Id).ToList();

                    foreach (var question in surveyQuestions)
                    {
                        question.Answers = answers.Where(a => a.QuestionId == question.Id).ToList();
                    }

                    survey.Questions = surveyQuestions;
                }

                return Ok(surveys);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }



        [HttpGet("/rrc/api/[controller]/[action]")]
        public async Task<IActionResult> GetSurveysByUserInfo(int empid, int SurveyId)
        {
            try
            {
                var _connectionString = _connectionStringProvider.GetConnectionString("RRC_Test");

                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                // Fetch all surveys created by the specified user
                const string surveysQuery = @"
        SELECT Id, Title, Description, CreatedBy
        FROM Surveys
        WHERE CreatedBy = @CreatedBy;
        ";

                var surveys = (await connection.QueryAsync<Survey>(surveysQuery, new { CreatedBy = empid })).ToList();

                if (!surveys.Any())
                {
                    return NotFound($"No surveys found for user with ID {empid}.");
                }

                // Fetch all questions for the retrieved surveys
                var surveyIds = surveys.Select(s => s.Id).ToArray();

                const string questionsQuery = @"
        SELECT Id, Text, Type, SurveyId
        FROM Questions
        WHERE SurveyId IN @SurveyIds;
        ";

                var questions = (await connection.QueryAsync<Question>(questionsQuery, new { SurveyIds = surveyIds })).ToList();

                // Fetch all answers for the retrieved questions
                var questionIds = questions.Select(q => q.Id).ToArray();

                const string answersQuery = @"
        SELECT Id, Text, QuestionId
        FROM Answers
        WHERE QuestionId IN @QuestionIds;
        ";

                var answers = (await connection.QueryAsync<Answer>(answersQuery, new { QuestionIds = questionIds })).ToList();

                // Map questions and answers to their respective surveys
                foreach (var survey in surveys)
                {
                    var surveyQuestions = questions.Where(q => q.SurveyId == survey.Id).ToList();

                    foreach (var question in surveyQuestions)
                    {
                        question.Answers = answers.Where(a => a.QuestionId == question.Id).ToList();
                    }

                    survey.Questions = surveyQuestions;
                }

                return Ok(surveys);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }




        [HttpGet("/rrc/api/[controller]/[action]")]
        [ProducesResponseType(typeof(Towns), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<List<Survey>> GetAllSurveyDetailsAsync()
        {

            var connectionString = _connectionStringProvider.GetConnectionString("RRC_Test");
            var connection = new Microsoft.Data.SqlClient.SqlConnection(connectionString);
            _context = DbContextFactory.Create(connectionString);
            var data = await connection.QueryAsync<SurveryDetails>("SELECT s.Id AS SurveyId,s.CreatedBy AS CreatedBy, s.Title AS SurveyTitle, s.Description AS SurveyDescription, q.Id AS QuestionId, q.Text AS QuestionText,q.Type AS QuestionType, a.Id AS AnswerId, a.Text AS AnswerText,r.Id AS ResponseId, r.TextResponse AS ResponseText, u.Id AS UserId,u.Username AS Username, u.Email AS UserEmail FROM Surveys s LEFT JOIN Questions q ON s.Id = q.SurveyId LEFT JOIN Answers a ON q.Id = a.QuestionId LEFT JOIN Responses r ON(a.Id = r.AnswerId OR q.Id = r.Id) LEFT JOIN Users u ON r.UserId = u.Id ORDER BY s.Id, q.Id, a.Id, r.Id; ");
            var surveys = new List<Survey>();
            foreach (var row in data)
            {
                // Find or create Survey
                var survey = surveys.FirstOrDefault(s => s.Id == row.SurveyId);
                if (survey == null)
                {
                    survey = new Survey
                    {
                        Id = row.SurveyId,
                        Title = row.SurveyTitle,
                        Description = row.SurveyDescription,
                        CreatedBy = row.CreatedBy
                    };
                    surveys.Add(survey);
                }

                // Find or create Question
                if (row.QuestionId.HasValue)
                {
                    if (survey.Questions == null)
                    {
                        survey.Questions = new List<Question>();
                    }
                    var question = survey.Questions?.FirstOrDefault(q => q.Id == row.QuestionId);
                    if (question == null)
                    {
                        question = new Question
                        {
                            Id = row.QuestionId.Value,
                            Text = row.QuestionText,
                            Type = row.QuestionType.HasValue ? (QuestionType)row.QuestionType.Value : default
                        };
                        survey.Questions.Add(question);
                    }

                    // Find or create Answer
                    if (row.AnswerId.HasValue)
                    {
                        if (question.Answers == null)
                        {
                            question.Answers = new List<Answer>();
                        }
                        if (!question.Answers.Any(a => a.Id == row.AnswerId))
                        {
                            question.Answers.Add(new Answer
                            {
                                Id = row.AnswerId.Value,
                                Text = row.AnswerText
                            });
                        }
                    }

                    // Find or create Response
                    if (row.ResponseId.HasValue)
                    {
                        if (question.Responses == null)
                        {
                            question.Responses = new List<Models.Response>();
                        }
                        if (!question.Responses.Any(r => r.Id == row.ResponseId))
                        {
                            question.Responses.Add(new Models.Response
                            {
                                Id = row.ResponseId.Value,
                                TextResponse = row.ResponseText,
                                User = row.UserId.HasValue ? new User
                                {
                                    Id = row.UserId.Value,
                                    Username = row.Username,
                                    Email = row.UserEmail
                                } : null
                            });
                        }
                    }
                }
            }
            List<SurveryDetails> surveryDetails = new List<SurveryDetails>();

            return surveys;
        }

        //    [HttpPost("/rrc/api/[controller]/[action]")]
        //    [ProducesResponseType(typeof(Towns), StatusCodes.Status200OK)]
        //    [ProducesResponseType(StatusCodes.Status404NotFound)]
        //    public async Task<IActionResult> SubmitSurveyAnswersAsync(int userId, List<SurveryDetails> responses)
        //    {
        //        const string query = @"
        //    INSERT INTO Responses (SurveyId, QuestionId, UserId, AnswerId, TextResponse)
        //    VALUES (@SurveyId, @QuestionId, @UserId, @AnswerId, @TextResponse);
        //";
        //        var connectionString = _connectionStringProvider.GetConnectionString("RRC_Test");

        //        using (var connection = new SqlConnection(connectionString))
        //        {
        //            foreach (var response in responses)
        //            {
        //                // Validate QuestionType and determine how to store the response
        //                if (response.QuestionType == QuestionType.MultipleChoice || response.QuestionType == QuestionType.Dropdown)
        //                {
        //                    if (response.AnswerId == null)
        //                    {
        //                        throw new ArgumentException($"AnswerId is required for question {response.QuestionId}.");
        //                    }
        //                }
        //                else if (response.QuestionType == QuestionType.Text)
        //                {
        //                    if (string.IsNullOrWhiteSpace(response.ResponseText))
        //                    {
        //                        throw new ArgumentException($"ResponseText is required for question {response.QuestionId}.");
        //                    }
        //                }

        //                // Insert the response into the database
        //                await connection.ExecuteAsync(query, new
        //                {
        //                    SurveyId = response.SurveyId,
        //                    QuestionId = response.QuestionId,
        //                    UserId = userId,
        //                    AnswerId = response.QuestionType == QuestionType.Text ? null : response.AnswerId,
        //                    TextResponse = response.QuestionType == QuestionType.Text ? response.ResponseText : null
        //                });
        //            }
        //        }

        //        ResultObject patResult1 = new ResultObject
        //        {
        //            Status = true,
        //            StatusCode = StatusCodes.Status200OK,
        //            token = null,
        //            Message = "Data Found",
        //            data = null
        //        };
        //        return Ok(patResult1);
        //    }

        [HttpPost("/rrc/api/[controller]/[action]")]
        [ProducesResponseType(typeof(Towns), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CreateSurvey([FromBody] SurveyRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Title) || string.IsNullOrWhiteSpace(request.Description))
            {
                return BadRequest("Title and description are required.");
            }

            try
            {
                var _connectionString = _connectionStringProvider.GetConnectionString("RRC_Test");

                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                // Insert the survey
                const string insertSurveyQuery = @"
                INSERT INTO Surveys (Title, Description, CreatedBy)
                OUTPUT INSERTED.Id
                VALUES (@Title, @Description, @CreatedBy);
            ";
                var surveyId = await connection.ExecuteScalarAsync<int>(insertSurveyQuery, new
                {
                    request.Title,
                    request.Description,
                    request.CreatedBy
                });

                if (request.Questions != null && request.Questions.Any())
                {
                    foreach (var question in request.Questions)
                    {
                        // Insert the question
                        const string insertQuestionQuery = @"
                        INSERT INTO Questions (Text, Type, SurveyId)
                        OUTPUT INSERTED.Id
                        VALUES (@Text, @Type, @SurveyId);
                    ";
                        var questionId = await connection.ExecuteScalarAsync<int>(insertQuestionQuery, new
                        {
                            question.Text,
                            question.Type,
                            SurveyId = surveyId
                        });

                        // Insert the answers (if applicable)
                        if (question.Answers != null && question.Answers.Any())
                        {
                            const string insertAnswerQuery = @"
                            INSERT INTO Answers (Text, QuestionId)
                            VALUES (@Text, @QuestionId);
                        ";

                            foreach (var answer in question.Answers)
                            {
                                await connection.ExecuteAsync(insertAnswerQuery, new
                                {
                                    answer.Text,
                                    QuestionId = questionId
                                });
                            }
                        }
                    }
                }

                return Ok(new { SurveyId = surveyId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        //public async Task<IActionResult> CreateSurvey([FromBody] SurveyRequest request)
        //{
        //    if (string.IsNullOrEmpty(request.Title) || string.IsNullOrEmpty(request.Description))
        //    {
        //        return BadRequest("Title and Description are required.");
        //    }

        //    try
        //    {
        //        var _connectionString = _connectionStringProvider.GetConnectionString("RRC_Test");

        //        using (var connection = new SqlConnection(_connectionString))
        //        {
        //            await connection.OpenAsync();

        //            // Insert Survey
        //            const string insertSurveyQuery = @"
        //            INSERT INTO Surveys (Title, Description, CreatedBy)
        //            OUTPUT INSERTED.Id
        //            VALUES (@Title, @Description, @CreatedBy)";

        //            var surveyId = await connection.ExecuteScalarAsync<int>(insertSurveyQuery, new
        //            {
        //                Title = request.Title,
        //                Description = request.Description,
        //                CreatedBy = request.CreatedBy
        //            });

        //            // Insert Questions if provided
        //            if (request.Questions?.Count > 0)
        //            {
        //                const string insertQuestionQuery = @"
        //                INSERT INTO Questions (Text, Type, SurveyId)
        //                VALUES (@Text, @Type, @SurveyId)";

        //                foreach (var question in request.Questions)
        //                {
        //                    await connection.ExecuteAsync(insertQuestionQuery, new
        //                    {
        //                        Text = question.Text,
        //                        Type = question.Type,
        //                        SurveyId = surveyId
        //                    });
        //                }
        //            }

        //            return Ok(new { SurveyId = surveyId });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"Internal server error: {ex.Message}");
        //    }
        //}

        [HttpPost("/rrc/api/[controller]/[action]")]
        [ProducesResponseType(typeof(Towns), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetOrCreateUser([FromBody] string email)
        {
            if (string.IsNullOrEmpty(email))
                return BadRequest("Email is required.");

            try
            {
                var _connectionString = _connectionStringProvider.GetConnectionString("RRC_Test");

                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    // Check if the user exists
                    const string getUserQuery = "SELECT Id, Username, Email FROM Users WHERE Email = @Email";
                    var existingUser = await connection.QueryFirstOrDefaultAsync<User>(getUserQuery, new { Email = email });

                    if (existingUser != null)
                    {
                        return Ok(existingUser); // Return the existing user
                    }

                    // Create a new user
                    const string insertUserQuery = @"
                    INSERT INTO Users (Username, Email)
                    OUTPUT INSERTED.*
                    VALUES (@Username, @Email)";

                    var newUser = await connection.QuerySingleAsync<User>(insertUserQuery, new
                    {
                        Username = email.Split('@')[0], // Derive username from email
                        Email = email
                    });
                    ResultObject patResult1 = new ResultObject
                    {
                        Status = true,
                        StatusCode = StatusCodes.Status200OK,
                        token = null,
                        Message = "Responses submitted successfully.",
                        data = newUser
                    };
                    return Ok(patResult1);
                }
            }
            catch (Exception ex)
            {
                // Handle errors
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("/rrc/api/[controller]/[action]")]
        public async Task<IActionResult> CreateCompany([FromBody] Company company)
        {
            var _connectionString = _connectionStringProvider.GetConnectionString("RRC_Test");

            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync("INSERT INTO Companies (Name) VALUES (@Name)", company);
            return Ok("Company created successfully.");
        }
        [HttpPost("/rrc/api/[controller]/[action]")]
        public async Task<IActionResult> AssignAdminToCompany(int adminId, int companyId)
        {
            var _connectionString = _connectionStringProvider.GetConnectionString("RRC_Test");

            using var connection = new SqlConnection(_connectionString);

            // Check if Admin exists
            var adminExists = await connection.ExecuteScalarAsync<int>(
                "SELECT COUNT(1) FROM Admins WHERE Id = @AdminId",
                new { AdminId = adminId });
            if (adminExists == 0) return NotFound("Admin does not exist.");

            // Check if Company exists
            var companyExists = await connection.ExecuteScalarAsync<int>(
                "SELECT COUNT(1) FROM Companies WHERE Id = @CompanyId",
                new { CompanyId = companyId });
            if (companyExists == 0) return NotFound("Company does not exist.");

            // Assign Admin to Company
            await connection.ExecuteAsync(@"
        UPDATE Admins
        SET CompanyId = @CompanyId
        WHERE Id = @AdminId", new { AdminId = adminId, CompanyId = companyId });

            return Ok("Admin assigned to company successfully.");
        }
        [HttpGet("/rrc/api/[controller]/[action]")]
        public async Task<IActionResult> GetCompanies()
        {
            var _connectionString = _connectionStringProvider.GetConnectionString("RRC_Test");

            using var connection = new SqlConnection(_connectionString);
            var companies = await connection.QueryAsync(@"
        SELECT c.Id AS CompanyId, c.Name AS CompanyName, 
               a.Id AS AdminId, a.Name AS AdminName, a.Email AS AdminEmail
        FROM Companies c
        LEFT JOIN Admins a ON c.Id = a.CompanyId");

            return Ok(companies);
        }

        [HttpGet("/rrc/api/[controller]/[action]")]
        public async Task<IActionResult> GetSurveys(int empid)
        {
            var _connectionString = _connectionStringProvider.GetConnectionString("RRC_Test");

            using var connection = new SqlConnection(_connectionString);
            var companies = await connection.QueryAsync(@"
         select *  from Surveys where CreatedBy='" + empid + "'");

            return Ok(companies);

        }


        [HttpGet("/rrc/api/[controller]/[action]")]
        public async Task<IActionResult> GetUnassignedAdmins()
        {
            var _connectionString = _connectionStringProvider.GetConnectionString("RRC_Test");

            using var connection = new SqlConnection(_connectionString);
            var unassignedAdmins = await connection.QueryAsync<Admin>(
                "SELECT * FROM Admins WHERE CompanyId IS NULL");
            return Ok(unassignedAdmins);
        }

        [HttpPost("/rrc/api/[controller]/[action]")]
        public async Task<IActionResult> CreateAdmin([FromBody] Admin admin)
        {
            var _connectionString = _connectionStringProvider.GetConnectionString("RRC_Test");

            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync(@"
        INSERT INTO Admins (Name, Email, Password) 
        VALUES (@Name, @Email, @Password)", admin);
            return Ok("Admin created successfully.");
        }
        [HttpGet("/rrc/api/[controller]/[action]")]
        public async Task<IActionResult> GetEmployees()
        {
            try
            {
                var _connectionString = _connectionStringProvider.GetConnectionString("RRC_Test");

                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                // Query the Employees table
                var employees = await connection.QueryAsync<Employee>("SELECT * FROM Employees");

                // Return the list of employees
                return Ok(employees);
            }
            catch (Exception ex)
            {
                // Return a 500 Internal Server Error if something goes wrong
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("/rrc/api/[controller]/[action]")]
        public async Task<IActionResult> AssignSurvey([FromBody] SurveyAssignment assignment)
        {
            var _connectionString = _connectionStringProvider.GetConnectionString("RRC_Test");

            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync(@"
        INSERT INTO SurveyAssignments (SurveyId, EmployeeId) 
        VALUES (@SurveyId, @EmployeeId)", assignment);
            return Ok("Survey assigned successfully.");
        }


        [HttpGet("/rrc/api/[controller]/[action]")]
        public async Task<IActionResult> CreateEmployee([FromBody] Employee employee)
        {
            var _connectionString = _connectionStringProvider.GetConnectionString("RRC_Test");

            using var connection = new SqlConnection(_connectionString);

            await connection.ExecuteAsync(@"
        INSERT INTO Employees (Name, Email, Password, CompanyId) 
        VALUES (@Name, @Email, @Password, @CompanyId)", employee);

            return Ok("Employee created successfully.");
        }

        [HttpPost("/rrc/api/[controller]/[action]")]
        public async Task<IActionResult> SuperAdminLogin([FromBody] SuperAdmin login)
        {
            var _connectionString = _connectionStringProvider.GetConnectionString("RRC_Test");

            using var connection = new SqlConnection(_connectionString);
            var superAdmin = await connection.QueryFirstOrDefaultAsync<SuperAdmin>(
                "SELECT * FROM SuperAdmins WHERE Email = @Email AND Password = @Password", login);

            if (superAdmin == null)
            {
                return Unauthorized("Invalid credentials.");
            }

            return Ok(new { superAdmin.Id, superAdmin.Name });
        }

        [HttpPost("/rrc/api/[controller]/[action]")]
        public async Task<IActionResult> AdminLogin([FromBody] Admin login)
        {
            var _connectionString = _connectionStringProvider.GetConnectionString("RRC_Test");

            using var connection = new SqlConnection(_connectionString);
            var superAdmin = await connection.QueryFirstOrDefaultAsync<Admin>(
                "SELECT * FROM Admins WHERE Email = @Email AND Password = @Password", login);

            if (superAdmin == null)
            {
                return Unauthorized("Invalid credentials.");
            }

            return Ok(new { superAdmin.Id, superAdmin.Name, superAdmin.CompanyId });
        }

        [HttpPost("/rrc/api/[controller]/[action]")]
        [ProducesResponseType(typeof(Towns), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UserLogin([FromBody] string email)
        {
            if (string.IsNullOrEmpty(email))
                return BadRequest("Email is required.");

            try
            {
                var _connectionString = _connectionStringProvider.GetConnectionString("RRC_Test");

                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    // Check if the user exists
                    const string getUserQuery = "SELECT Id, Username, Email FROM Users WHERE Email = @Email";
                    var existingUser = await connection.QueryFirstOrDefaultAsync<User>(getUserQuery, new { Email = email });

                    if (existingUser != null)
                    {
                        return Ok(existingUser); // Return the existing user
                    }

                    // Create a new user
                    const string insertUserQuery = @"
                    INSERT INTO Users (Username, Email)
                    OUTPUT INSERTED.*
                    VALUES (@Username, @Email)";

                    var newUser = await connection.QuerySingleAsync<User>(insertUserQuery, new
                    {
                        Username = email.Split('@')[0], // Derive username from email
                        Email = email
                    });
                    ResultObject patResult1 = new ResultObject
                    {
                        Status = true,
                        StatusCode = StatusCodes.Status200OK,
                        token = null,
                        Message = "Responses submitted successfully.",
                        data = newUser
                    };
                    return Ok(patResult1);
                }
            }
            catch (Exception ex)
            {
                // Handle errors
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("/rrc/api/[controller]/[action]")]
        [ProducesResponseType(typeof(Towns), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SurveyUserLogin([FromBody] SurveyUser email)
        {
            if (string.IsNullOrEmpty(email?.UserEmail))
                return BadRequest("Email is required.");

            try
            {
                var _connectionString = _connectionStringProvider.GetConnectionString("RRC_Test");
                _context = DbContextFactory.Create(_connectionString);
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    // Check if the user exists
                    const string getUserQuery = "SELECT Id, Username, Email FROM Users WHERE Email = @Email";
                    var existingUser = await connection.QueryFirstOrDefaultAsync<User>(getUserQuery, new { Email = email?.UserEmail });
                  

                    if (existingUser != null)
                    {
                        try
                        {
                            var answeredQuestions = _context.SurveyResponsesInfo
                               .Where(sr => sr.SurveyId == email.SurveyId && sr.UserId == existingUser.Id)
                               .Select(sr => sr.QuestionId)
                               .Distinct()
                               .Count();

                            bool isCompleted = false;
                            if (answeredQuestions > 0)
                            {
                                isCompleted = true;
                                ResultObject patResult11 = new ResultObject
                                {
                                    Status = true,
                                    StatusCode = StatusCodes.Status200OK,
                                    token = null,
                                    Message = "Survery Completed Already",
                                    data = existingUser
                                };
                                return Ok(patResult11);
                            }
                            else
                            {
                                var data = await connection.QueryAsync<SurveryDetails>("SELECT s.Id AS SurveyId,s.CreatedBy AS CreatedBy, s.Title AS SurveyTitle, s.Description AS SurveyDescription, q.Id AS QuestionId, q.Text AS QuestionText,q.Type AS QuestionType, a.Id AS AnswerId, a.Text AS AnswerText,r.Id AS ResponseId, r.TextResponse AS ResponseText, u.Id AS UserId,u.Username AS Username, u.Email AS UserEmail FROM Surveys s LEFT JOIN Questions q ON s.Id = q.SurveyId LEFT JOIN Answers a ON q.Id = a.QuestionId LEFT JOIN Responses r ON(a.Id = r.AnswerId OR q.Id = r.Id) LEFT JOIN Users u ON r.UserId = u.Id ORDER BY s.Id, q.Id, a.Id, r.Id; ");
                                var surveys = new List<Survey>();

                                foreach (var row in data)
                                {
                                    // Find or create Survey
                                    var survey = surveys.FirstOrDefault(s => s.Id == row.SurveyId);
                                    if (survey == null)
                                    {
                                        survey = new Survey
                                        {
                                            Id = row.SurveyId,
                                            Title = row.SurveyTitle,
                                            Description = row.SurveyDescription,
                                            CreatedBy = row.CreatedBy
                                        };
                                        surveys.Add(survey);
                                    }

                                    // Find or create Question
                                    if (row.QuestionId.HasValue)
                                    {
                                        if (survey.Questions == null)
                                        {
                                            survey.Questions = new List<Question>();
                                        }
                                        var question = survey.Questions?.FirstOrDefault(q => q.Id == row.QuestionId);
                                        if (question == null)
                                        {
                                            question = new Question
                                            {
                                                Id = row.QuestionId.Value,
                                                Text = row.QuestionText,
                                                Type = row.QuestionType.HasValue ? (QuestionType)row.QuestionType.Value : default
                                            };
                                            survey.Questions.Add(question);
                                        }

                                        // Find or create Answer
                                        if (row.AnswerId.HasValue)
                                        {
                                            if (question.Answers == null)
                                            {
                                                question.Answers = new List<Answer>();
                                            }
                                            if (!question.Answers.Any(a => a.Id == row.AnswerId))
                                            {
                                                question.Answers.Add(new Answer
                                                {
                                                    Id = row.AnswerId.Value,
                                                    Text = row.AnswerText
                                                });
                                            }
                                        }

                                        // Find or create Response
                                        if (row.ResponseId.HasValue)
                                        {
                                            if (question.Responses == null)
                                            {
                                                question.Responses = new List<Models.Response>();
                                            }
                                            if (!question.Responses.Any(r => r.Id == row.ResponseId))
                                            {
                                                question.Responses.Add(new Models.Response
                                                {
                                                    Id = row.ResponseId.Value,
                                                    TextResponse = row.ResponseText,
                                                    User = row.UserId.HasValue ? new User
                                                    {
                                                        Id = row.UserId.Value,
                                                        Username = row.Username,
                                                        Email = row.UserEmail
                                                    } : null
                                                });
                                            }
                                        }
                                    }
                                }

                                if (surveys.Count > 0)
                                {
                                    {
                                        surveys = surveys.Where(x => x.Id == email.SurveyId).ToList();
                                    }
                                    existingUser.Surveys = surveys;
                                    ResultObject patResult13 = new ResultObject
                                    {
                                        Status = true,
                                        StatusCode = StatusCodes.Status200OK,
                                        token = null,
                                        Message = "success",
                                        data = existingUser
                                    };
                                }
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                        return Ok(existingUser); // Return the existing user
                    }

                    // Create a new user
                    const string insertUserQuery = @"
                    INSERT INTO Users (Username, Email)
                    OUTPUT INSERTED.*
                    VALUES (@Username, @Email)";

                    var newUser = await connection.QuerySingleAsync<User>(insertUserQuery, new
                    {
                        Username = email?.UserEmail.Split('@')[0], // Derive username from email
                        Email = email?.UserEmail
                    });
                    if (newUser != null)

                    {
                        try
                        {

                            var answeredQuestions = _context.SurveyResponsesInfo
                               .Where(sr => sr.SurveyId == email.SurveyId && sr.UserId == newUser.Id)
                               .Select(sr => sr.QuestionId)
                               .Distinct()
                               .Count();

                            bool isCompleted = false;
                            if (answeredQuestions > 0)
                            {
                                isCompleted = true;
                                ResultObject patResult11 = new ResultObject
                                {
                                    Status = true,
                                    StatusCode = StatusCodes.Status200OK,
                                    token = null,
                                    Message = "Survery Completed Already",
                                    data = newUser
                                };
                            }
                            else
                            {
                                var data = await connection.QueryAsync<SurveryDetails>("SELECT s.Id AS SurveyId,s.CreatedBy AS CreatedBy, s.Title AS SurveyTitle, s.Description AS SurveyDescription, q.Id AS QuestionId, q.Text AS QuestionText,q.Type AS QuestionType, a.Id AS AnswerId, a.Text AS AnswerText,r.Id AS ResponseId, r.TextResponse AS ResponseText, u.Id AS UserId,u.Username AS Username, u.Email AS UserEmail FROM Surveys s LEFT JOIN Questions q ON s.Id = q.SurveyId LEFT JOIN Answers a ON q.Id = a.QuestionId LEFT JOIN Responses r ON(a.Id = r.AnswerId OR q.Id = r.Id) LEFT JOIN Users u ON r.UserId = u.Id ORDER BY s.Id, q.Id, a.Id, r.Id; ");
                                var surveys = new List<Survey>();

                                foreach (var row in data)
                                {
                                    // Find or create Survey
                                    var survey = surveys.FirstOrDefault(s => s.Id == row.SurveyId);
                                    if (survey == null)
                                    {
                                        survey = new Survey
                                        {
                                            Id = row.SurveyId,
                                            Title = row.SurveyTitle,
                                            Description = row.SurveyDescription,
                                            CreatedBy = row.CreatedBy
                                        };
                                        surveys.Add(survey);
                                    }

                                    // Find or create Question
                                    if (row.QuestionId.HasValue)
                                    {
                                        if (survey.Questions == null)
                                        {
                                            survey.Questions = new List<Question>();
                                        }
                                        var question = survey.Questions?.FirstOrDefault(q => q.Id == row.QuestionId);
                                        if (question == null)
                                        {
                                            question = new Question
                                            {
                                                Id = row.QuestionId.Value,
                                                Text = row.QuestionText,
                                                Type = row.QuestionType.HasValue ? (QuestionType)row.QuestionType.Value : default
                                            };
                                            survey.Questions.Add(question);
                                        }

                                        // Find or create Answer
                                        if (row.AnswerId.HasValue)
                                        {
                                            if (question.Answers == null)
                                            {
                                                question.Answers = new List<Answer>();
                                            }
                                            if (!question.Answers.Any(a => a.Id == row.AnswerId))
                                            {
                                                question.Answers.Add(new Answer
                                                {
                                                    Id = row.AnswerId.Value,
                                                    Text = row.AnswerText
                                                });
                                            }
                                        }

                                        // Find or create Response
                                        if (row.ResponseId.HasValue)
                                        {
                                            if (question.Responses == null)
                                            {
                                                question.Responses = new List<Models.Response>();
                                            }
                                            if (!question.Responses.Any(r => r.Id == row.ResponseId))
                                            {
                                                question.Responses.Add(new Models.Response
                                                {
                                                    Id = row.ResponseId.Value,
                                                    TextResponse = row.ResponseText,
                                                    User = row.UserId.HasValue ? new User
                                                    {
                                                        Id = row.UserId.Value,
                                                        Username = row.Username,
                                                        Email = row.UserEmail
                                                    } : null
                                                });
                                            }
                                        }
                                    }
                                }

                                if (surveys.Count > 0)
                                {
                                    {
                                        surveys = surveys.Where(x => x.Id == email.SurveyId).ToList();
                                    }
                                    ResultObject patResult13 = new ResultObject
                                    {
                                        Status = true,
                                        StatusCode = StatusCodes.Status200OK,
                                        token = null,
                                        Message = "success",
                                        data = surveys
                                    };
                                }
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                    ResultObject patResult1 = new ResultObject
                    {
                        Status = true,
                        StatusCode = StatusCodes.Status200OK,
                        token = null,
                        Message = "success",
                        data = newUser
                    };
                    return Ok(patResult1);
                }
            }
            catch (Exception ex)
            {
                // Handle errors
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpPost("/rrc/api/[controller]/[action]")]
        [ProducesResponseType(typeof(Towns), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SubmitSurveyResponses([FromBody] List<SurveyResponse> responses)
        {
            if (responses == null || !responses.Any())
            {
                return BadRequest("No responses provided.");
            }

            // Validate SurveyId consistency
            var surveyId = responses.First().SurveyId;
            if (responses.Any(r => r.SurveyId != surveyId))
            {
                return BadRequest("All responses must belong to the same survey.");
            }

            const string query = @"
        INSERT INTO UserSurveyResponse (SurveyId, QuestionId, AnswerId, TextResponse, UserId)
        VALUES (@SurveyId, @QuestionId, @AnswerId, @TextResponse, @UserId);
    ";
            var connectionString = _connectionStringProvider.GetConnectionString("RRC_Test");

            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        foreach (var response in responses)
                        {
                            // Validate QuestionType and corresponding data
                            if (response.QuestionType == 1 || response.QuestionType == 3) // MultipleChoice or Dropdown
                            {
                                if (response.AnswerId == null)
                                {
                                    return BadRequest($"AnswerId is required for question {response.QuestionId}.");
                                }
                            }
                            else if (response.QuestionType == 2) // Text
                            {
                                if (string.IsNullOrWhiteSpace(response.TextResponse))
                                {
                                    return BadRequest($"TextResponse is required for question {response.QuestionId}.");
                                }
                            }

                            // Insert the response into the database
                            await connection.ExecuteAsync(query, new
                            {
                                SurveyId = response.SurveyId,
                                QuestionId = response.QuestionId,
                                AnswerId = response.AnswerId,
                                TextResponse = response.TextResponse,
                                UserId = response.UserId
                            }, transaction);
                        }

                        transaction.Commit();
                        ResultObject patResult1 = new ResultObject
                        {
                            Status = true,
                            StatusCode = StatusCodes.Status200OK,
                            token = null,
                            Message = "Responses submitted successfully.",
                            data = "Responses submitted successfully."
                        };
                        return Ok(patResult1);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return StatusCode(500, $"Internal server error: {ex.Message}");
                    }
                }
            }
        }

        [HttpGet("/rrc/api/[controller]/[action]")]
        [ProducesResponseType(typeof(Towns), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult ValidateSurveyFullCompletion(int surveyId, int userId)
        {
            var connectionString = _connectionStringProvider.GetConnectionString("RRC_Test");
            var connection = new Microsoft.Data.SqlClient.SqlConnection(connectionString);
            _context = DbContextFactory.Create(connectionString);


            var answeredQuestions = _context.SurveyResponsesInfo
                .Where(sr => sr.SurveyId == surveyId && sr.UserId == userId)
                .Select(sr => sr.QuestionId)
                .Distinct()
                .Count();

            bool isCompleted = false;
            if (answeredQuestions > 0)
            {
                isCompleted = true;
            }
            if (isCompleted)
            {
                ResultObject patResult1 = new ResultObject
                {
                    Status = true,
                    StatusCode = StatusCodes.Status200OK,
                    token = null,
                    Message = "Data Found",
                    data = isCompleted
                };

                return Ok(patResult1);
            }
            else
            {
                ResultObject patResult1 = new ResultObject
                {
                    Status = true,
                    StatusCode = StatusCodes.Status200OK,
                    token = null,
                    Message = "Data Found",
                    data = isCompleted
                };

                return Ok(patResult1);
            }
        }
        [HttpGet("/rrc/api/[controller]/[action]")]
        [ProducesResponseType(typeof(Towns), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<List<Survey>> GetAllSurveyDetailsBySurveyId(int SurveyId)
        {

            var connectionString = _connectionStringProvider.GetConnectionString("RRC_Test");
            var connection = new Microsoft.Data.SqlClient.SqlConnection(connectionString);
            _context = DbContextFactory.Create(connectionString);
            var data = await connection.QueryAsync<SurveryDetails>("SELECT s.Id AS SurveyId,s.CreatedBy AS CreatedBy, s.Title AS SurveyTitle, s.Description AS SurveyDescription, q.Id AS QuestionId, q.Text AS QuestionText,q.Type AS QuestionType, a.Id AS AnswerId, a.Text AS AnswerText,r.Id AS ResponseId, r.TextResponse AS ResponseText, u.Id AS UserId,u.Username AS Username, u.Email AS UserEmail FROM Surveys s LEFT JOIN Questions q ON s.Id = q.SurveyId LEFT JOIN Answers a ON q.Id = a.QuestionId LEFT JOIN Responses r ON(a.Id = r.AnswerId OR q.Id = r.Id) LEFT JOIN Users u ON r.UserId = u.Id ORDER BY s.Id, q.Id, a.Id, r.Id; ");
            var surveys = new List<Survey>();
            foreach (var row in data)
            {
                // Find or create Survey
                var survey = surveys.FirstOrDefault(s => s.Id == row.SurveyId);
                if (survey == null)
                {
                    survey = new Survey
                    {
                        Id = row.SurveyId,
                        Title = row.SurveyTitle,
                        Description = row.SurveyDescription,
                        CreatedBy = row.CreatedBy
                    };
                    surveys.Add(survey);
                }

                // Find or create Question
                if (row.QuestionId.HasValue)
                {
                    if (survey.Questions == null)
                    {
                        survey.Questions = new List<Question>();
                    }
                    var question = survey.Questions?.FirstOrDefault(q => q.Id == row.QuestionId);
                    if (question == null)
                    {
                        question = new Question
                        {
                            Id = row.QuestionId.Value,
                            Text = row.QuestionText,
                            Type = row.QuestionType.HasValue ? (QuestionType)row.QuestionType.Value : default
                        };
                        survey.Questions.Add(question);
                    }

                    // Find or create Answer
                    if (row.AnswerId.HasValue)
                    {
                        if (question.Answers == null)
                        {
                            question.Answers = new List<Answer>();
                        }
                        if (!question.Answers.Any(a => a.Id == row.AnswerId))
                        {
                            question.Answers.Add(new Answer
                            {
                                Id = row.AnswerId.Value,
                                Text = row.AnswerText
                            });
                        }
                    }

                    // Find or create Response
                    if (row.ResponseId.HasValue)
                    {
                        if (question.Responses == null)
                        {
                            question.Responses = new List<Models.Response>();
                        }
                        if (!question.Responses.Any(r => r.Id == row.ResponseId))
                        {
                            question.Responses.Add(new Models.Response
                            {
                                Id = row.ResponseId.Value,
                                TextResponse = row.ResponseText,
                                User = row.UserId.HasValue ? new User
                                {
                                    Id = row.UserId.Value,
                                    Username = row.Username,
                                    Email = row.UserEmail
                                } : null
                            });
                        }
                    }
                }
            }
            List<SurveryDetails> surveryDetails = new List<SurveryDetails>();

            if (surveys.Count > 0)
            {
                surveys = surveys.Where(x => x.Id == SurveyId).ToList();
            }
            return surveys;
        }
        public class LoginRequest
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }

        public class LoginResponse
        {
            public int UserId { get; set; }
            public string Username { get; set; }
            public List<Survey> Surveys { get; set; }
        }


        [HttpPost("/rrc/api/[controller]/[action]")]
        public async Task<IActionResult> SurveyCompanyEmployeeLogin([FromBody] LoginRequest loginRequest)
        {
            if (string.IsNullOrEmpty(loginRequest.Username) || string.IsNullOrEmpty(loginRequest.Password))
            {
                return BadRequest("Username and password are required.");
            }
            var connectionString = _connectionStringProvider.GetConnectionString("RRC_Test");
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                // Validate user credentials
                var query = @"SELECT Id, Username, Password FROM Op_Employee_Survey WHERE Username = @Username AND Active = 1";
                var user = await connection.QuerySingleOrDefaultAsync<dynamic>(query, new { loginRequest.Username });

                if (user == null)
                {
                    return Unauthorized("Invalid username or password.");
                }

                // Verify password (consider hashing passwords in production)
                if (user.Password != loginRequest.Password)
                {
                    return Unauthorized("Invalid username or password.");
                }

                // Get surveys linked to the user
                var surveysQuery = @"
                SELECT s.Id, s.Title, s.Description
                FROM Surveys s
                WHERE s.CreatedBy = @UserId"; // Assuming `Surveys` table has a CreatedBy column for user linkage
                var surveys = (await connection.QueryAsync<Survey>(surveysQuery, new { UserId = user.Id })).ToList();

                // Prepare the response
                var response = new LoginResponse
                {
                    UserId = user.Id,
                    Username = user.Username,
                    Surveys = surveys
                };

                ResultObject patResult1 = new ResultObject
                {
                    Status = true,
                    StatusCode = StatusCodes.Status200OK,
                    token = null,
                    Message = "Data Found",
                    data = response
                };
                return Ok(patResult1);
            }
        }
        [HttpGet("/rrc/api/[controller]/[action]")]
        public async Task<IActionResult> GetSurveyResponses()
        {
            try
            {
                var connectionString = _connectionStringProvider.GetConnectionString("RRC_Test");

                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                var query = @"
           SELECT 
     sr.UserId,
     u.Name AS UserName,
     s.Id AS SurveyId,
     s.Title AS SurveyTitle,
     q.Text AS QuestionText,
     r.TextResponse AS ResponseText,
     a.Text AS AnswerText
 FROM SurveyResponsesInfo sr
 INNER JOIN Employees u ON sr.UserId = u.Id
 INNER JOIN Surveys s ON sr.SurveyId = s.Id
 INNER JOIN Questions q ON sr.QuestionId = q.Id
 LEFT JOIN Answers a ON sr.AnswerId = a.Id
 LEFT JOIN Responses r ON sr.Id = r.Id
 ORDER BY sr.UserId;
        ";

                var responses = await connection.QueryAsync<SurveyResponseDto>(query);

                // Group responses by User and Survey
                var groupedResponses = responses
                    .GroupBy(r => new { r.UserId, r.UserName, r.SurveyId, r.SurveyTitle })
                    .Select(group => new
                    {
                        group.Key.UserId,
                        group.Key.UserName,
                        group.Key.SurveyId,
                        group.Key.SurveyTitle,
                        Answers = group.Select(r => new
                        {
                            Question = r.QuestionText,
                            Response = r.AnswerText ?? r.ResponseText
                        })
                    });

                return Ok(groupedResponses);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        //[HttpPost("/rrc/api/[controller]/[action]")]
        //[ProducesResponseType(typeof(Towns), StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //public async Task<IActionResult> SubmitAnswer([FromBody] AnswerSubmission submission)
        //{
        //    if (submission == null || submission.SurveyId == 0 || submission.QuestionId == 0 || string.IsNullOrEmpty(submission.UserEmail))
        //    {
        //        return BadRequest("Invalid submission data.");
        //    }

        //    // Check if the submission is correct based on question type
        //    switch (submission.QuestionType)
        //    {
        //        case QuestionType.MultipleChoice:
        //        case QuestionType.Dropdown:
        //            if (!submission.AnswerId.HasValue)
        //            {
        //                return BadRequest("AnswerId is required for MultipleChoice and Dropdown questions.");
        //            }
        //            break;

        //        case QuestionType.Text:
        //            if (string.IsNullOrWhiteSpace(submission.TextResponse))
        //            {
        //                return BadRequest("TextResponse is required for Text questions.");
        //            }
        //            break;

        //        default:
        //            return BadRequest("Invalid question type.");
        //    }

        //    var connectionString = _connectionStringProvider.GetConnectionString("RRC_Test");
        //    // var connectionSQl = new Microsoft.Data.SqlClient.SqlConnection(connectionString);
        //    using (var connection = new SqlConnection(connectionString))
        //    {
        //        await connection.OpenAsync();

        //        // Get the UserId by email or create a new user if necessary
        //        var userId = await GetOrCreateUserId(submission.UserEmail, connection);

        //        // Insert the response
        //        var query = @"INSERT INTO Responses (SurveyId, QuestionId, AnswerId, TextResponse, UserId)
        //              VALUES (@SurveyId, @QuestionId, @AnswerId, @TextResponse, @UserId)";

        //        await connection.ExecuteAsync(query, new
        //        {
        //            SurveyId = submission.SurveyId,
        //            QuestionId = submission.QuestionId,
        //            AnswerId = submission.AnswerId,
        //            TextResponse = submission.TextResponse,
        //            UserId = userId
        //        });
        //    }

        //    return Ok("Answer submitted successfully.");
        //}

        private async Task<int> GetOrCreateUserId(string email, SqlConnection connection)
        {
            // Query to check if the user exists based on email
            var userId = await connection.QuerySingleOrDefaultAsync<int?>(
                "SELECT Id FROM Users WHERE Email = @Email", new { Email = email });

            // If the user already exists, return the UserId
            if (userId.HasValue)
            {
                return userId.Value;
            }

            // If the user does not exist, insert a new user record and get the new UserId
            var insertUserQuery = @"
        INSERT INTO Users (Email, Username) 
        VALUES (@Email, @Username);
        SELECT CAST(SCOPE_IDENTITY() as int);";

            // Execute the insert and retrieve the new UserId
            var newUserId = await connection.QuerySingleAsync<int>(insertUserQuery,
                new { Email = email, Username = email.Split('@')[0] }); // Use part of email as default username

            return newUserId;
        }


        [HttpGet("/rrc/api/[controller]/[action]")]
        [ProducesResponseType(typeof(Towns), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> LoginEmployee(string username, string password)
        {
            try
            {
                var connectionString = _connectionStringProvider.GetConnectionString("RRC");
                var connection = new Microsoft.Data.SqlClient.SqlConnection(connectionString);
                var data = await connection.QueryAsync<OP_Employee>("select * from RRC..OP_Employee where nuser='" + username + "' and password='" + password + "'");

                if (data.Count() <= 0)
                {

                    ResultObject patResult = new ResultObject
                    {
                        Status = false,
                        StatusCode = StatusCodes.Status401Unauthorized,
                        token = null,
                        Message = "User Name OR Password Does not Exists",
                        data = null
                    };
                    return Ok(patResult);
                }


                ResultObject patResult1 = new ResultObject
                {
                    Status = true,
                    StatusCode = StatusCodes.Status200OK,
                    token = null,
                    Message = "Data Found",
                    data = data
                };
                return Ok(patResult1);

            }
            catch (Exception ex)
            {
                ResultObject patResult = new ResultObject
                {
                    Status = true,
                    StatusCode = StatusCodes.Status422UnprocessableEntity,
                    token = null,
                    Message = ex.StackTrace,
                    data = null
                };
                return Ok(patResult);
            }

        }

        [HttpPost("/rrc/api/[controller]/[action]")]
        public async Task<IActionResult> SubmitSurvey([FromBody] SurveySubmissionRequest request)
        {
            if (request == null || request.Questions == null || !request.Questions.Any())
            {
                return BadRequest("Invalid survey submission payload.");
            }

            try
            {
                var connectionString = _connectionStringProvider.GetConnectionString("RRC_Test");

                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                // Check if the SurveyId exists in the Surveys table
                var surveyExists = await connection.ExecuteScalarAsync<int>(
                    "SELECT COUNT(1) FROM Surveys WHERE Id = @SurveyId",
                    new { SurveyId = request.SurveyId });

                if (surveyExists == 0)
                {
                    return NotFound($"Survey with Id {request.SurveyId} does not exist.");
                }

                // Check if all QuestionIds exist for the given SurveyId
                var validQuestions = await connection.QueryAsync<int>(
                    "SELECT Id FROM Questions WHERE SurveyId = @SurveyId",
                    new { SurveyId = request.SurveyId });

                var validQuestionIds = validQuestions.ToHashSet();
                foreach (var question in request.Questions)
                {
                    if (!validQuestionIds.Contains(question.QuestionId))
                    {
                        return NotFound($"Question with Id {question.QuestionId} does not exist in Survey {request.SurveyId}.");
                    }
                }

                // Insert the survey response for each question
                foreach (var question in request.Questions)
                {
                    string insertQuery = question.QuestionType switch
                    {
                        1 or 3 or 4 => @"
                    INSERT INTO SurveyResponsesInfo (SurveyId, UserId, QuestionId, AnswerId)
                    VALUES (@SurveyId, @UserId, @QuestionId, @AnswerId);",
                        2 => @"
                    INSERT INTO SurveyResponsesInfo (SurveyId, UserId, QuestionId, TextResponse)
                    VALUES (@SurveyId, @UserId, @QuestionId, @TextResponse);",
                        _ => throw new Exception("Unsupported question type.")
                    };

                    var parameters = new DynamicParameters();
                    parameters.Add("SurveyId", request.SurveyId);
                    parameters.Add("UserId", request.UserId);
                    parameters.Add("QuestionId", question.QuestionId);

                    if (question.QuestionType == 2)  // Text Response
                    {
                        parameters.Add("TextResponse", question.QuestionAnswer);
                    }
                    else
                    {
                        // Parse comma-separated answer IDs for Multiple Choice, Dropdown, or Radio
                        var answerIds = question.QuestionAnswer.Split(',');
                        foreach (var answerId in answerIds)
                        {
                            parameters.Add("AnswerId", int.Parse(answerId));
                            await connection.ExecuteAsync(insertQuery, parameters);
                        }
                    }
                }
                ResultObject patResult1 = new ResultObject
                {
                    Status = true,
                    StatusCode = StatusCodes.Status200OK,
                    token = null,
                    Message = "Survey submitted successfully.",
                    data = null
                };
                return Ok(patResult1);

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        //[HttpPost("/rrc/api/[controller]/[action]")]
        //public async Task<IActionResult> SubmitSurvey([FromBody] SurveySubmissionRequest request)
        //{
        //    if (request == null || request.Questions == null || !request.Questions.Any())
        //    {
        //        return BadRequest("Invalid survey submission payload.");
        //    }

        //    try
        //    {
        //        var connectionString = _connectionStringProvider.GetConnectionString("RRC_Test");

        //        using var connection = new SqlConnection(connectionString);
        //        await connection.OpenAsync();

        //        // Insert the survey response for each question
        //        foreach (var question in request.Questions)
        //        {
        //            string insertQuery = question.QuestionType switch
        //            {
        //                1 or 3 or 4 => @"
        //            INSERT INTO SurveyResponsesInfo (SurveyId, UserId, QuestionId, AnswerId)
        //            VALUES (@SurveyId, @UserId, @QuestionId, @AnswerId);",
        //                2 => @"
        //            INSERT INTO SurveyResponsesInfo (SurveyId, UserId, QuestionId, TextResponse)
        //            VALUES (@SurveyId, @UserId, @QuestionId, @TextResponse);",
        //                _ => throw new Exception("Unsupported question type.")
        //            };

        //            var parameters = new DynamicParameters();
        //            parameters.Add("SurveyId", request.SurveyId);
        //            parameters.Add("UserId", request.UserId);
        //            parameters.Add("QuestionId", question.QuestionId);

        //            if (question.QuestionType == 2)
        //            {
        //                parameters.Add("TextResponse", question.QuestionAnswer);
        //            }
        //            else
        //            {
        //                // Parse comma-separated answer IDs for Multiple Choice
        //                var answerIds = question.QuestionAnswer.Split(',');
        //                foreach (var answerId in answerIds)
        //                {
        //                    parameters.Add("AnswerId", int.Parse(answerId));
        //                    await connection.ExecuteAsync(insertQuery, parameters);
        //                }
        //            }
        //        }

        //        return Ok("Survey submitted successfully.");
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"Internal server error: {ex.Message}");
        //    }
        //}


        [HttpGet("/rrc/api/[controller]/[action]")]
        [ProducesResponseType(typeof(Towns), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AdminLoginEmployee(string username, string password)
        {
            try
            {
                var connectionString = _connectionStringProvider.GetConnectionString("RRC_Test");
                var connection = new Microsoft.Data.SqlClient.SqlConnection(connectionString);
                var data = await connection.QueryAsync<Op_Employee_Survey>("select * from RRC_Test..Op_Employee_Survey where Username='" + username + "' and Password='" + password + "' and Level='ADMIN'");

                if (data.Count() <= 0)
                {

                    ResultObject patResult = new ResultObject
                    {
                        Status = false,
                        StatusCode = StatusCodes.Status401Unauthorized,
                        token = null,
                        Message = "User Name OR Password Does not Exists",
                        data = null
                    };
                    return Ok(patResult);
                }


                ResultObject patResult1 = new ResultObject
                {
                    Status = true,
                    StatusCode = StatusCodes.Status200OK,
                    token = null,
                    Message = "Data Found",
                    data = data
                };
                return Ok(patResult1);

            }
            catch (Exception ex)
            {
                ResultObject patResult = new ResultObject
                {
                    Status = true,
                    StatusCode = StatusCodes.Status422UnprocessableEntity,
                    token = null,
                    Message = ex.StackTrace,
                    data = null
                };
                return Ok(patResult);
            }

        }

        [HttpGet("/rrc/api/[controller]/[action]")]
        [ProducesResponseType(typeof(Towns), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAllTowns()
        {
            try
            {
                var connectionString = _connectionStringProvider.GetConnectionString("RRC");
                var connection = new Microsoft.Data.SqlClient.SqlConnection(connectionString);
                var data = await connection.QueryAsync<Towns>("Select * from Towns");
                if (data.Count() <= 0)
                {

                    ResultObject patResult = new ResultObject
                    {
                        Status = false,
                        StatusCode = StatusCodes.Status401Unauthorized,
                        token = null,
                        Message = "No town exists",
                        data = null
                    };
                    return Ok(patResult);
                }


                ResultObject patResult1 = new ResultObject
                {
                    Status = true,
                    StatusCode = StatusCodes.Status200OK,
                    token = null,
                    Message = "Data Found",
                    data = data
                };
                return Ok(patResult1);

            }
            catch (Exception ex)
            {
                ResultObject patResult = new ResultObject
                {
                    Status = true,
                    StatusCode = StatusCodes.Status422UnprocessableEntity,
                    token = null,
                    Message = ex.StackTrace,
                    data = null
                };
                return Ok(patResult);
            }

        }

        [HttpGet("/rrc/api/[controller]/[action]")]
        [ProducesResponseType(typeof(Towns), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAllProperties(int accountNum, string townId = "")
        {
            if (Request.Headers.TryGetValue("townId", out StringValues townname))
            {
                townId = townname;
            }
            string dbame = string.Empty;
            if (!string.IsNullOrEmpty(townId))
            {
                dbame = "RRC_" + townId.TrimStart().TrimEnd();
            }
            else
            {
                dbame = "RRC_Agawam";
            }
            var connectionString = _connectionStringProvider.GetConnectionString(dbame);

            _context = DbContextFactory.Create(connectionString);
            List<property> properties = new List<property>();
            try
            {
                if (accountNum != 0 || accountNum != null)
                {
                    properties = await _context.property.Where(x => x.accountno == accountNum).ToListAsync();

                }
                else
                {
                    properties = await _context.property.ToListAsync();

                }
                if (properties.Count() <= 0)
                {

                    ResultObject patResult = new ResultObject
                    {
                        Status = false,
                        StatusCode = StatusCodes.Status401Unauthorized,
                        token = null,
                        Message = "No town exists",
                        data = null
                    };
                    return Ok(patResult);
                }


                ResultObject patResult1 = new ResultObject
                {
                    Status = true,
                    StatusCode = StatusCodes.Status200OK,
                    token = null,
                    Message = "Data Found",
                    data = properties
                };
                return Ok(patResult1);

            }
            catch (Exception ex)
            {
                ResultObject patResult = new ResultObject
                {
                    Status = true,
                    StatusCode = StatusCodes.Status422UnprocessableEntity,
                    token = null,
                    Message = ex.StackTrace,
                    data = null
                };
                return Ok(patResult);
            }

        }




        [HttpGet("/rrc/api/[controller]/[action]")]
        public async Task<IActionResult> GetTownDetails(int id)
        {

            var newUser = await _context.Towns.Where(u => u.TownId == id.ToString()).ToListAsync();
            if (newUser.Count() == 0)
            {
                ResultObject patResult = new ResultObject
                {
                    Status = false,
                    StatusCode = StatusCodes.Status204NoContent,
                    token = null,
                    data = null,
                    Message = "No town exists with this Id"
                };
                return Ok(patResult);
            }

            try
            {
                ResultObject successResult = new ResultObject
                {
                    Status = true,
                    StatusCode = StatusCodes.Status200OK,
                    token = null,
                    data = newUser,
                    Message = "Employee deleted successfully"
                };
                return Ok(successResult);

            }
            catch (DbUpdateConcurrencyException ex)
            {
                ResultObject patResult = new ResultObject
                {
                    Status = false,
                    StatusCode = StatusCodes.Status204NoContent,
                    token = null,
                    data = null,
                    Message = ex.Message
                };
                return Ok(patResult);
            }
            catch (Exception ex)
            {
                ResultObject patResult = new ResultObject
                {
                    Status = false,
                    StatusCode = StatusCodes.Status204NoContent,
                    token = null,
                    data = null,
                    Message = ex.Message
                };
                return Ok(patResult);
            }

        }


        [HttpGet("/rrc/api/[controller]/[action]")]
        [ProducesResponseType(typeof(Towns), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAllTaxPayers(string townId = "")
        {
            try
            {

                if (Request.Headers.TryGetValue("townId", out StringValues townname))
                {
                    townId = townname;
                }
                string dbame = string.Empty;
                if (!string.IsNullOrEmpty(townId))
                {
                    dbame = "RRC_" + townId.TrimStart().TrimEnd();
                }
                else
                {
                    dbame = "RRC_Agawam";
                }
                var connectionString = _connectionStringProvider.GetConnectionString(dbame);
                var connection = new Microsoft.Data.SqlClient.SqlConnection(connectionString);
                var taxPayers = connection.Query<TaxPayer>("Select * from TaxPayer").ToList();
                // var taxPayers = await _context.TaxPayer.ToListAsync();
                taxPayers = taxPayers
        .Select(e =>
        {
            if (e.user1 == null)
            {
                e.user1 = string.Empty;
            }
            if (e.user2 == null)
            {
                e.user2 = string.Empty;
            }
            if (e.user3 == null)
            {
                e.user3 = string.Empty;
            }
            if (e.user4 == null)
            {
                e.user4 = string.Empty;
            }

            return e;
        })
        .ToList();

                if (taxPayers.Count() <= 0)
                {

                    ResultObject patResult = new ResultObject
                    {
                        Status = false,
                        StatusCode = StatusCodes.Status401Unauthorized,
                        token = null,
                        Message = "No Taxpayers exists",
                        data = null
                    };
                    return Ok(patResult);
                }


                ResultObject patResult1 = new ResultObject
                {
                    Status = true,
                    StatusCode = StatusCodes.Status200OK,
                    token = null,
                    Message = "Data Found",
                    data = taxPayers
                };
                return Ok(patResult1);

            }
            catch (Exception ex)
            {
                ResultObject patResult = new ResultObject
                {
                    Status = true,
                    StatusCode = StatusCodes.Status422UnprocessableEntity,
                    token = null,
                    Message = ex.StackTrace,
                    data = null
                };
                return Ok(patResult);
            }

        }


        [HttpGet("/rrc/api/[controller]/[action]")]
        [ProducesResponseType(typeof(Towns), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdatepricingModel(string accountNo, string townId = "")
        {
            try
            {

                if (Request.Headers.TryGetValue("townId", out StringValues townname))
                {
                    townId = townname;
                }
                string dbame = string.Empty;
                if (!string.IsNullOrEmpty(townId))
                {
                    dbame = "RRC_" + townId.TrimStart().TrimEnd();
                }
                else
                {
                    dbame = "RRC_Agawam";
                }
                var connectionString = _connectionStringProvider.GetConnectionString(dbame);

                _context = DbContextFactory.Create(connectionString);
                var result = _context.Database.ExecuteSqlRaw("EXEC STP_PropCalc @AccountNo, @mdepcalc",
        new SqlParameter("@AccountNo", accountNo),
        new SqlParameter("@mdepcalc", "1"));
                ResultObject patResult1 = new ResultObject
                {
                    Status = true,
                    StatusCode = StatusCodes.Status200OK,
                    token = null,
                    Message = "Data Found",
                    data = "Property and TaxPayer Tables Updated"
                };
                return Ok(patResult1);

            }
            catch (Exception ex)
            {
                ResultObject patResult = new ResultObject
                {
                    Status = true,
                    StatusCode = StatusCodes.Status422UnprocessableEntity,
                    token = null,
                    Message = ex.StackTrace,
                    data = null
                };
                return Ok(patResult);
            }

        }


        [HttpGet("/rrc/api/[controller]/[action]")]
        [ProducesResponseType(typeof(Towns), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> LeaseDetails(string accountNo, string townId = "")
        {
            string dbame = string.Empty;

            if (Request.Headers.TryGetValue("townId", out StringValues townname))
            {
                townId = townname;
            }
            if (!string.IsNullOrEmpty(townId))
            {
                dbame = "RRC_" + townId.TrimStart().TrimEnd();
            }
            else
            {
                dbame = "RRC_Agawam";
            }
            var connectionString = _connectionStringProvider.GetConnectionString(dbame);

            _context = DbContextFactory.Create(connectionString);
            try
            {

                var connection = new Microsoft.Data.SqlClient.SqlConnection(connectionString);
                var data = connection.Query<LeaseObject>("select leasee,* from property  where isnull(leasee,0)=" + accountNo + "").ToList();
                if (data.Count > 0)
                {

                    List<LeaseObject> leaseObjects = new List<LeaseObject>();
                    foreach (var leaseObject in data)
                    {
                        leaseObjects.Add(leaseObject);
                    }

                    var strJson = JsonSerializer.Serialize(leaseObjects);
                    List<LeaseObject> LeaseObjects = JsonSerializer.Deserialize<List<LeaseObject>>(strJson);

                    string result = string.Empty;
                    var updatedJsonString = JsonSerializer.Serialize(LeaseObjects);


                    ResultObject patResult1 = new ResultObject
                    {
                        Status = true,
                        StatusCode = StatusCodes.Status200OK,
                        token = null,
                        Message = "Data Found",
                        data = LeaseObjects
                    };
                    return Ok(patResult1);
                }
                else
                {
                    ResultObject patResult1 = new ResultObject
                    {
                        Status = true,
                        StatusCode = StatusCodes.Status204NoContent,
                        token = null,
                        Message = "No Data Found",
                        data = null
                    };
                    return Ok(patResult1);
                }

            }
            catch (Exception ex)
            {
                ResultObject patResult = new ResultObject
                {
                    Status = true,
                    StatusCode = StatusCodes.Status422UnprocessableEntity,
                    token = null,
                    Message = ex.StackTrace,
                    data = null
                };
                return Ok(patResult);
            }

        }


        //[HttpGet("/rrc/api/[controller]/[action]")]
        //[ProducesResponseType(typeof(Towns), StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //public async Task<IActionResult> GetMenuandSubMenu()
        //{
        //    try
        //    {
        //        string ConnectionString = new DataService(_configuration)._connectionString;
        //        var connection = new Microsoft.Data.SqlClient.SqlConnection(ConnectionString);
        //        connection.Open();
        //        var products = await _context.OP_Security_Points.FromSqlRaw("SP_OP_UserRights 'cd','WINTHROP','50.203.98.226'")
        //.ToListAsync();


        //        //using (var command = connection.CreateCommand())
        //        //{
        //        //    command.CommandText = "SP_OP_UserRights";
        //        //    var nusernameParameter = new SqlParameter("@nusername", "cd");
        //        //    var pwdParameter = new SqlParameter("@pwd", "WINTHROP");
        //        //    var ipParameter = new SqlParameter("@ip", "50.203.98.226");
        //        //    command.Parameters.Add(nusernameParameter);
        //        //    command.Parameters.Add(pwdParameter);
        //        //    command.Parameters.Add(ipParameter);
        //        //    command.CommandType = System.Data.CommandType.StoredProcedure;

        //        //    using (var reader = command.ExecuteReader())
        //        //    {
        //        //        // Read Products
        //        //        var products = new List<SecDetails>();
        //        //        while (reader.Read())
        //        //        {
        //        //            var product = new SecDetails
        //        //            {

        //        //                UserModule = reader.GetString(1), // Assuming Name is the second column

        //        //            };
        //        //            products.Add(product);
        //        //        }

        //        //        reader.NextResult(); // Move to the next result set


        //        //    }
        //        //}



        //        var data = connection.Query<SecDetails>("EXEC  SP_OP_UserRights 'cd','WINTHROP','50.203.98.226'").ToList();
        //        string result = string.Empty;
        //        if (data != null)
        //        {
        //            result = JsonConvert.SerializeObject(data);
        //        }
        //        ResultObject patResult1 = new ResultObject
        //        {
        //            Status = true,
        //            StatusCode = StatusCodes.Status200OK,
        //            token = null,
        //            Message = "Data Found",
        //            data = result
        //        };
        //        return Ok(patResult1);

        //    }
        //    catch (Exception ex)
        //    {
        //        ResultObject patResult = new ResultObject
        //        {
        //            Status = true,
        //            StatusCode = StatusCodes.Status422UnprocessableEntity,
        //            token = null,
        //            Message = ex.StackTrace,
        //            data = null
        //        };
        //        return Ok(patResult);
        //    }

        //}

        [HttpPut("/rrc/api/[controller]/[action]")]
        public async Task<IActionResult> UpdateTown(Towns con)
        {

            var newUser = await _context.Towns.Where(u => u.TownId == con.TownId).ToListAsync();
            if (newUser == null || newUser.Count() == 0)
            {
                ResultObject patResult = new ResultObject
                {
                    Status = false,
                    StatusCode = StatusCodes.Status204NoContent,
                    token = null,
                    data = null,
                    Message = "No User exists with this User Id"
                };
                return Ok(patResult);
            }

            if (!string.IsNullOrEmpty(con.TownName))
            {
                newUser[0].TownName = con.TownName;
            }
            if (!string.IsNullOrEmpty(con.Contact))
            {
                newUser[0].Contact = con.Contact;
            }
            if (!string.IsNullOrEmpty(con.Address1))
            {
                newUser[0].Address1 = con.Address1;
            }
            if (!string.IsNullOrEmpty(con.Address2))
            {
                newUser[0].Address2 = con.Address2;
            }
            if (!string.IsNullOrEmpty(con.City))
            {
                newUser[0].City = con.City;
            }
            if (!string.IsNullOrEmpty(con.State))
            {
                newUser[0].State = con.State;
            }
            if (!string.IsNullOrEmpty(con.Zip))
            {
                newUser[0].Zip = con.Zip;
            }
            if (!string.IsNullOrEmpty(con.ContactNumber))
            {
                newUser[0].ContactNumber = con.ContactNumber;
            }
            if (!string.IsNullOrEmpty(con.DBName))
            {
                newUser[0].DBName = con.DBName;
            }
            if (!string.IsNullOrEmpty(con.Notes))
            {
                newUser[0].Notes = con.Notes;
            }
            if (!string.IsNullOrEmpty(con.MainPage))
            {
                newUser[0].MainPage = con.MainPage;
            }
            if (!string.IsNullOrEmpty(con.SnapShots))
            {
                newUser[0].SnapShots = con.SnapShots;
            }
            if (!string.IsNullOrEmpty(con.LoginTimeFrom))
            {
                newUser[0].LoginTimeFrom = con.LoginTimeFrom;
            }
            if (!string.IsNullOrEmpty(con.LoginTimeTo))
            {
                newUser[0].LoginTimeTo = con.LoginTimeTo;
            }
            if (!string.IsNullOrEmpty(con.AllowedIPs))
            {
                newUser[0].AllowedIPs = con.AllowedIPs;
            }
            if (!string.IsNullOrEmpty(con.FTPInfo))
            {
                newUser[0].FTPInfo = con.FTPInfo;
            }
            if (!string.IsNullOrEmpty(con.Website))
            {
                newUser[0].Website = con.Website;
            }
            if (con.lng != 0)
            {
                newUser[0].lng = con.lng;
            }
            if (con.lat != 0)
            {
                newUser[0].lat = con.lat;
            }
            if (con.Taxpayer != 0)
            {
                newUser[0].lng = con.Taxpayer;
            }
            if (con.Properties != 0)
            {
                newUser[0].lng = con.Properties;
            }
            if (con.TotalValue != 0)
            {
                newUser[0].lng = con.TotalValue;
            }
            if (con.GrowthValue != 0)
            {
                newUser[0].lng = con.GrowthValue;
            }
            if (con.GrowthValue != 0)
            {
                newUser[0].lng = con.GrowthValue;
            }
            if (con.TaxRate != 0)
            {
                newUser[0].lng = con.TaxRate;
            }
            if (con.GrowthAmt != 0)
            {
                newUser[0].lng = con.GrowthAmt;
            }
            if (con.GrowthYr != 0)
            {
                newUser[0].lng = con.GrowthYr;
            }
            _context.Update(newUser[0]);

            try
            {
                await _context.SaveChangesAsync();
                ResultObject successResult = new ResultObject
                {
                    Status = true,
                    StatusCode = StatusCodes.Status200OK,
                    token = null,
                    data = newUser[0],
                    Message = "Updated successfully"
                };
                return Ok(successResult);

            }
            catch (DbUpdateConcurrencyException ex)
            {
                ResultObject patResult = new ResultObject
                {
                    Status = false,
                    StatusCode = StatusCodes.Status204NoContent,
                    token = null,
                    data = null,
                    Message = ex.Message
                };
                return Ok(patResult);
            }
            catch (Exception ex)
            {
                ResultObject patResult = new ResultObject
                {
                    Status = false,
                    StatusCode = StatusCodes.Status204NoContent,
                    token = null,
                    data = null,
                    Message = ex.Message
                };
                return Ok(patResult);
            }

        }

        [HttpPost("/rrc/api/[controller]/[action]")]
        public async Task<IActionResult> AddTown(Towns con, int isUpdate)
        {


            string dbame = string.Empty;
            dbame = "RRC";
            var connectionString = _connectionStringProvider.GetConnectionString(dbame);

            _context = DbContextFactory.Create(connectionString);


            if (isUpdate == 1)
            {
                try
                {
                    var townNumber = await _context.Towns.Where(u => u.TownId == con.TownId).ToListAsync();
                    if (!string.IsNullOrEmpty(con.TownName))
                    {
                        townNumber[0].TownName = con.TownName;
                    }
                    if (!string.IsNullOrEmpty(con.Contact))
                    {
                        townNumber[0].Contact = con.Contact;
                    }
                    if (!string.IsNullOrEmpty(con.Address1))
                    {
                        townNumber[0].Address1 = con.Address1;
                    }
                    if (!string.IsNullOrEmpty(con.Address2))
                    {
                        townNumber[0].Address2 = con.Address2;
                    }
                    if (!string.IsNullOrEmpty(con.City))
                    {
                        townNumber[0].City = con.City;
                    }
                    if (!string.IsNullOrEmpty(con.State))
                    {
                        townNumber[0].State = con.State;
                    }
                    if (!string.IsNullOrEmpty(con.Zip))
                    {
                        townNumber[0].Zip = con.Zip;
                    }
                    if (!string.IsNullOrEmpty(con.ContactNumber))
                    {
                        townNumber[0].ContactNumber = con.ContactNumber;
                    }
                    if (!string.IsNullOrEmpty(con.DBName))
                    {
                        townNumber[0].DBName = con.DBName;
                    }
                    if (!string.IsNullOrEmpty(con.Notes))
                    {
                        townNumber[0].Notes = con.Notes;
                    }
                    if (!string.IsNullOrEmpty(con.MainPage))
                    {
                        townNumber[0].MainPage = con.MainPage;
                    }
                    if (!string.IsNullOrEmpty(con.SnapShots))
                    {
                        townNumber[0].SnapShots = con.SnapShots;
                    }
                    if (!string.IsNullOrEmpty(con.LoginTimeFrom))
                    {
                        townNumber[0].LoginTimeFrom = con.LoginTimeFrom;
                    }
                    if (!string.IsNullOrEmpty(con.LoginTimeTo))
                    {
                        townNumber[0].LoginTimeTo = con.LoginTimeTo;
                    }
                    if (!string.IsNullOrEmpty(con.AllowedIPs))
                    {
                        townNumber[0].AllowedIPs = con.AllowedIPs;
                    }
                    if (!string.IsNullOrEmpty(con.FTPInfo))
                    {
                        townNumber[0].FTPInfo = con.FTPInfo;
                    }
                    if (!string.IsNullOrEmpty(con.Website))
                    {
                        townNumber[0].Website = con.Website;
                    }
                    if (con.lng != 0)
                    {
                        townNumber[0].lng = con.lng;
                    }
                    if (con.lat != 0)
                    {
                        townNumber[0].lat = con.lat;
                    }
                    if (con.Taxpayer != 0)
                    {
                        townNumber[0].lng = con.Taxpayer;
                    }
                    if (con.Properties != 0)
                    {
                        townNumber[0].lng = con.Properties;
                    }
                    if (con.TotalValue != 0)
                    {
                        townNumber[0].lng = con.TotalValue;
                    }
                    if (con.GrowthValue != 0)
                    {
                        townNumber[0].lng = con.GrowthValue;
                    }
                    if (con.GrowthValue != 0)
                    {
                        townNumber[0].lng = con.GrowthValue;
                    }
                    if (con.TaxRate != 0)
                    {
                        townNumber[0].lng = con.TaxRate;
                    }
                    if (con.GrowthAmt != 0)
                    {
                        townNumber[0].lng = con.GrowthAmt;
                    }
                    if (con.GrowthYr != 0)
                    {
                        townNumber[0].lng = con.GrowthYr;
                    }
                    _context.Update(townNumber[0]);
                    await _context.SaveChangesAsync();
                    ResultObject successResult = new ResultObject
                    {
                        Status = true,
                        StatusCode = StatusCodes.Status200OK,
                        token = null,
                        data = con,
                        Message = "Updated successfully"
                    };
                    return Ok(successResult);
                }
                catch (Exception ex)
                {
                    await _context.SaveChangesAsync();
                    ResultObject successResult = new ResultObject
                    {
                        Status = true,
                        StatusCode = StatusCodes.Status200OK,
                        token = null,
                        data = con,
                        Message = "Updated successfully"
                    };
                    return Ok(successResult);
                }

            }
            else
            {
                con.TownId = con.TownName;
                if (!string.IsNullOrEmpty(con.TownName))
                {
                    con.TownName = con.TownName;
                }
                if (!string.IsNullOrEmpty(con.Contact))
                {
                    con.Contact = con.Contact;
                }
                if (!string.IsNullOrEmpty(con.Address1))
                {
                    con.Address1 = con.Address1;
                }
                if (!string.IsNullOrEmpty(con.Address2))
                {
                    con.Address2 = con.Address2;
                }
                if (!string.IsNullOrEmpty(con.City))
                {
                    con.City = con.City;
                }
                if (!string.IsNullOrEmpty(con.State))
                {
                    con.State = con.State;
                }
                if (!string.IsNullOrEmpty(con.Zip))
                {
                    con.Zip = con.Zip;
                }
                if (!string.IsNullOrEmpty(con.ContactNumber))
                {
                    con.ContactNumber = con.ContactNumber;
                }
                if (!string.IsNullOrEmpty(con.DBName))
                {
                    con.DBName = con.DBName;
                }
                if (!string.IsNullOrEmpty(con.Notes))
                {
                    con.Notes = con.Notes;
                }
                if (!string.IsNullOrEmpty(con.MainPage))
                {
                    con.MainPage = con.MainPage;
                }
                if (!string.IsNullOrEmpty(con.SnapShots))
                {
                    con.SnapShots = con.SnapShots;
                }
                if (!string.IsNullOrEmpty(con.LoginTimeFrom))
                {
                    con.LoginTimeFrom = con.LoginTimeFrom;
                }
                if (!string.IsNullOrEmpty(con.LoginTimeTo))
                {
                    con.LoginTimeTo = con.LoginTimeTo;
                }
                if (!string.IsNullOrEmpty(con.AllowedIPs))
                {
                    con.AllowedIPs = con.AllowedIPs;
                }
                if (!string.IsNullOrEmpty(con.FTPInfo))
                {
                    con.FTPInfo = con.FTPInfo;
                }
                if (!string.IsNullOrEmpty(con.Website))
                {
                    con.Website = con.Website;
                }
                if (con.lng != 0)
                {
                    con.lng = con.lng;
                }
                if (con.lat != 0)
                {
                    con.lat = con.lat;
                }
                if (con.Taxpayer != 0)
                {
                    con.lng = con.Taxpayer;
                }
                if (con.Properties != 0)
                {
                    con.lng = con.Properties;
                }
                if (con.TotalValue != 0)
                {
                    con.lng = con.TotalValue;
                }
                if (con.GrowthValue != 0)
                {
                    con.lng = con.GrowthValue;
                }
                if (con.GrowthValue != 0)
                {
                    con.lng = con.GrowthValue;
                }
                if (con.TaxRate != 0)
                {
                    con.lng = con.TaxRate;
                }
                if (con.GrowthAmt != 0)
                {
                    con.lng = con.GrowthAmt;
                }
                if (con.GrowthYr != 0)
                {
                    con.lng = con.GrowthYr;
                }
            }
            _context.Add(con);

            try
            {
                await _context.SaveChangesAsync();
                ResultObject successResult = new ResultObject
                {
                    Status = true,
                    StatusCode = StatusCodes.Status200OK,
                    token = null,
                    data = con,
                    Message = "Added successfully"
                };
                return Ok(successResult);

            }
            catch (DbUpdateConcurrencyException ex)
            {
                ResultObject patResult = new ResultObject
                {
                    Status = false,
                    StatusCode = StatusCodes.Status204NoContent,
                    token = null,
                    data = null,
                    Message = ex.Message
                };
                return Ok(patResult);
            }
            catch (Exception ex)
            {
                ResultObject patResult = new ResultObject
                {
                    Status = false,
                    StatusCode = StatusCodes.Status204NoContent,
                    token = null,
                    data = null,
                    Message = ex.Message
                };
                return Ok(patResult);
            }
        }

        [HttpPost("/rrc/api/[controller]/[action]")]
        public async Task<IActionResult> AddTaxPayer(int isUpdate, TaxPayer con, string townId = "")
        {
            if (Request.Headers.TryGetValue("townId", out StringValues townname))
            {
                townId = townname;
            }
            string dbame = string.Empty;
            if (!string.IsNullOrEmpty(townId))
            {
                dbame = "RRC_" + townId.TrimStart().TrimEnd();
            }
            else
            {
                dbame = "RRC_Agawam";
            }
            var connectionString = _connectionStringProvider.GetConnectionString(dbame);

            _context = DbContextFactory.Create(connectionString);
            try
            {
                if (isUpdate == 1)
                {
                    var taxPayerNumber = await _context.TaxPayer.Where(u => u.accountno == con.accountno).ToListAsync();
                    if (con.user1 == null)
                    {
                        con.user1 = "";
                    }
                    if (con.user2 == null)
                    {
                        con.user2 = "";
                    }
                    if (con.user3 == null)
                    {
                        con.user3 = "";
                    }
                    if (con.user4 == null)
                    {
                        con.user4 = "";
                    }
                    if (!string.IsNullOrEmpty(con.Action))
                    {
                        con.Action = con.Action.TrimStart().TrimEnd();
                    }
                    if (!string.IsNullOrEmpty(con.FOL))
                    {
                        con.FOL = con.FOL.TrimStart().TrimEnd();
                    }
                    taxPayerNumber[0].accountno = con.accountno;
                    taxPayerNumber[0].Action = con.Action;
                    taxPayerNumber[0].nbrhd = con.nbrhd;
                    taxPayerNumber[0].owner = con.owner;
                    taxPayerNumber[0].inputdate = con.inputdate;
                    taxPayerNumber[0].locnum = con.locnum;
                    taxPayerNumber[0].locsuffix = con.locsuffix;
                    taxPayerNumber[0].locstreet = con.locstreet;
                    taxPayerNumber[0].dba = con.dba;
                    taxPayerNumber[0].mailaddr1 = con.mailaddr1;
                    taxPayerNumber[0].mailaddr2 = con.mailaddr2;
                    taxPayerNumber[0].mailcity = con.mailcity;
                    taxPayerNumber[0].mailstate = con.mailstate;
                    taxPayerNumber[0].mailzip = con.mailzip;
                    taxPayerNumber[0].areacode = con.areacode;
                    taxPayerNumber[0].phone = con.phone;
                    taxPayerNumber[0].source = con.source;
                    taxPayerNumber[0].taxcode = con.taxcode;
                    taxPayerNumber[0].datalister = con.datalister;
                    taxPayerNumber[0].entryclerk = con.entryclerk;
                    taxPayerNumber[0].totalvalue = con.totalvalue;
                    taxPayerNumber[0].oldtotal1 = con.oldtotal1;
                    taxPayerNumber[0].oldtotal2 = con.oldtotal2;
                    taxPayerNumber[0].oldtotal3 = con.oldtotal3;
                    taxPayerNumber[0].listdate = con.listdate;
                    taxPayerNumber[0].busntype = con.busntype;
                    taxPayerNumber[0].user1 = con.user1;
                    taxPayerNumber[0].user2 = con.user2;
                    taxPayerNumber[0].user3 = con.user3;
                    taxPayerNumber[0].user4 = con.user4;
                    taxPayerNumber[0].lastinput = con.lastinput;
                    taxPayerNumber[0].status = con.status;
                    taxPayerNumber[0].growth = con.growth;
                    taxPayerNumber[0].notes = con.notes;



                    taxPayerNumber[0].penalty = con.penalty;
                    taxPayerNumber[0].exemption = con.exemption;
                    taxPayerNumber[0].emailid = con.emailid;
                    taxPayerNumber[0].penaltyval = con.penaltyval;

                    taxPayerNumber[0].penalty = con.penalty;
                    taxPayerNumber[0].exemption = con.exemption;
                    taxPayerNumber[0].emailid = con.emailid;
                    taxPayerNumber[0].penaltyval = con.penaltyval;

                    taxPayerNumber[0].netvalue = con.netvalue;
                    taxPayerNumber[0].commno = con.commno;
                    taxPayerNumber[0].iTotal = con.iTotal;
                    taxPayerNumber[0].fTotal = con.fTotal;

                    taxPayerNumber[0].pTotal = con.pTotal;
                    taxPayerNumber[0].oTotal = con.oTotal;
                    taxPayerNumber[0].mTotal = con.mTotal;

                    taxPayerNumber[0].FOL = con.FOL;
                    taxPayerNumber[0].WebAddress = con.WebAddress;


                    taxPayerNumber[0].FId = con.FId;
                    taxPayerNumber[0].EditUser = con.EditUser;
                    taxPayerNumber[0].EditDate = con.EditDate;

                    taxPayerNumber[0].EntryUser = con.EntryUser;
                    taxPayerNumber[0].EntryDate = con.EntryDate;
                    taxPayerNumber[0].Password = con.Password;
                    taxPayerNumber[0].FOLEmail = con.FOLEmail;
                    if (string.IsNullOrEmpty(con.busntype))
                    {
                        ResultObject patResult = new ResultObject
                        {
                            Status = false,
                            StatusCode = StatusCodes.Status204NoContent,
                            token = null,
                            data = null,
                            Message = "Please Select Business Type "
                        };
                        return Ok(patResult);
                    }

                    if (string.IsNullOrEmpty(con.taxcode))
                    {
                        ResultObject patResult = new ResultObject
                        {
                            Status = false,
                            StatusCode = StatusCodes.Status204NoContent,
                            token = null,
                            data = null,
                            Message = "Please Select Tax Code "
                        };
                        return Ok(patResult);
                    }

                    if (!string.IsNullOrEmpty(con.Action))
                    {
                        if (con.Action.Length > 2)
                        {
                            ResultObject patResult = new ResultObject
                            {
                                Status = false,
                                StatusCode = StatusCodes.Status204NoContent,
                                token = null,
                                data = null,
                                Message = "Action Must be 2 Characters"
                            };
                            return Ok(patResult);

                        }
                    }

                    _context.Update(taxPayerNumber[0]);

                    try
                    {
                        await _context.SaveChangesAsync();
                        ResultObject successResult = new ResultObject
                        {
                            Status = true,
                            StatusCode = StatusCodes.Status200OK,
                            token = null,
                            data = taxPayerNumber[0],
                            Message = "Updated successfully"
                        };
                        return Ok(successResult);

                    }
                    catch (DbUpdateConcurrencyException ex)
                    {
                        ResultObject patResult = new ResultObject
                        {
                            Status = false,
                            StatusCode = StatusCodes.Status204NoContent,
                            token = null,
                            data = null,
                            Message = ex.Message
                        };
                        return Ok(patResult);
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }
            List<TaxPayer> taxPayers = new List<TaxPayer>();
            Random random = new Random();
            if (con.accountno == 0 || con.accountno == null)
            {
                con.accountno = random.Next(5000, 9999);
            }
            if (!string.IsNullOrEmpty(con.nbrhd))
            {
                con.nbrhd = con.nbrhd;
            }
            if (!string.IsNullOrEmpty(con.owner))
            {
                con.owner = con.owner;
            }
            if (con.inputdate == null)
            {
                con.inputdate = DateTime.Now;
            }

            con.locnum = con.locnum;

            if (!string.IsNullOrEmpty(con.locsuffix))
            {
                con.locsuffix = con.locsuffix;
            }
            if (!string.IsNullOrEmpty(con.locstreet))
            {
                con.locstreet = con.locstreet;
            }
            if (!string.IsNullOrEmpty(con.dba))
            {
                con.dba = con.dba;
            }
            if (!string.IsNullOrEmpty(con.mailaddr1))
            {
                con.mailaddr1 = con.mailaddr1;
            }
            if (!string.IsNullOrEmpty(con.mailaddr2))
            {
                con.mailaddr2 = con.mailaddr2;
            }
            if (!string.IsNullOrEmpty(con.mailcity))
            {
                con.mailcity = con.mailcity;
            }
            if (!string.IsNullOrEmpty(con.mailstate))
            {
                con.mailstate = con.mailstate;
            }
            if (!string.IsNullOrEmpty(con.mailzip))
            {
                con.mailzip = con.mailzip;
            }
            if (!string.IsNullOrEmpty(con.areacode))
            {
                con.areacode = con.areacode;
            }
            if (!string.IsNullOrEmpty(con.phone))
            {
                con.phone = con.phone;
            }
            if (!string.IsNullOrEmpty(con.source))
            {
                con.source = con.source;
            }
            if (!string.IsNullOrEmpty(con.taxcode))
            {
                con.taxcode = con.taxcode;
            }
            con.datalister = con.datalister;
            con.entryclerk = con.entryclerk;
            con.totalvalue = con.totalvalue;
            con.oldtotal1 = con.oldtotal1;
            con.oldtotal2 = con.oldtotal2;
            con.oldtotal3 = con.oldtotal3;
            con.listdate = con.listdate;
            con.busntype = con.busntype?.TrimEnd();
            if (con.user1 == null)
            {
                con.user1 = "";
            }
            if (con.user2 == null)
            {
                con.user2 = "";
            }
            if (con.user3 == null)
            {
                con.user3 = "";
            }
            if (con.user4 == null)
            {
                con.user4 = "";
            }
            con.user1 = con.user1;
            con.user2 = con.user2;
            con.user3 = con.user3;
            con.user4 = con.user4;
            if (string.IsNullOrEmpty(con.locstreet))
            {
                ResultObject patResult = new ResultObject
                {
                    Status = false,
                    StatusCode = StatusCodes.Status204NoContent,
                    token = null,
                    data = null,
                    Message = "Please enter street "
                };
                return Ok(patResult);
            }
            if (string.IsNullOrEmpty(con.dba))
            {
                ResultObject patResult = new ResultObject
                {
                    Status = false,
                    StatusCode = StatusCodes.Status204NoContent,
                    token = null,
                    data = null,
                    Message = "Please enter dba"
                };
                return Ok(patResult);
            }

            if (string.IsNullOrEmpty(con.owner))
            {
                ResultObject patResult = new ResultObject
                {
                    Status = false,
                    StatusCode = StatusCodes.Status204NoContent,
                    token = null,
                    data = null,
                    Message = "Please enter owner"
                };
                return Ok(patResult);
            }
            if (string.IsNullOrEmpty(con.Action))
            {
                con.Action = "Y";
            }
            if (!string.IsNullOrEmpty(con.Action))
            {
                if (con.Action.Length > 2)
                {
                    ResultObject patResult = new ResultObject
                    {
                        Status = false,
                        StatusCode = StatusCodes.Status204NoContent,
                        token = null,
                        data = null,
                        Message = "Action Must be 2 Characters"
                    };
                    return Ok(patResult);

                }
            }
            if (string.IsNullOrEmpty(con.FOL))
            {
                con.FOL = "FO";
            }
            if (string.IsNullOrEmpty(con.FOL))
            {
                if (con.FOL.Length > 2)
                {
                    ResultObject patResult = new ResultObject
                    {
                        Status = false,
                        StatusCode = StatusCodes.Status204NoContent,
                        token = null,
                        data = null,
                        Message = "FOL Must be 2 Characters"
                    };
                    return Ok(patResult);

                }
            }
            if (!string.IsNullOrEmpty(con.FOL))
            {
                if (con.FOL.Length > 2)
                {
                    ResultObject patResult = new ResultObject
                    {
                        Status = false,
                        StatusCode = StatusCodes.Status204NoContent,
                        token = null,
                        data = null,
                        Message = "FOL Must be 2 Characters"
                    };
                    return Ok(patResult);

                }
            }
            if (string.IsNullOrEmpty(con.areacode))
            {
                con.areacode = "301";
            }
            if (!string.IsNullOrEmpty(con.areacode))
            {
                if (con.areacode.Length > 3)
                {
                    ResultObject patResult = new ResultObject
                    {
                        Status = false,
                        StatusCode = StatusCodes.Status204NoContent,
                        token = null,
                        data = null,
                        Message = "areacode Must be 3 Characters"
                    };
                    return Ok(patResult);

                }
            }

            //if (!string.IsNullOrEmpty(con.busntype))
            //{
            //    con.busntype = "ACC";
            //}
            if (!string.IsNullOrEmpty(con.busntype))
            {
                if (con.busntype.Length > 4)
                {
                    ResultObject patResult = new ResultObject
                    {
                        Status = false,
                        StatusCode = StatusCodes.Status204NoContent,
                        token = null,
                        data = null,
                        Message = "busntype Must be 4 Characters"
                    };
                    return Ok(patResult);

                }
            }
            if (string.IsNullOrEmpty(con.datalister))
            {
                con.datalister = "CY";
            }
            if (!string.IsNullOrEmpty(con.datalister))
            {
                if (con.datalister.Length > 3)
                {
                    ResultObject patResult = new ResultObject
                    {
                        Status = false,
                        StatusCode = StatusCodes.Status204NoContent,
                        token = null,
                        data = null,
                        Message = "datalister Must be 3 Characters"
                    };
                    return Ok(patResult);

                }
            }
            if (!string.IsNullOrEmpty(con.entryclerk))
            {
                con.entryclerk = "AA";
            }
            if (!string.IsNullOrEmpty(con.entryclerk))
            {
                if (con.entryclerk.Length > 3)
                {
                    ResultObject patResult = new ResultObject
                    {
                        Status = false,
                        StatusCode = StatusCodes.Status204NoContent,
                        token = null,
                        data = null,
                        Message = "entryclerk Must be 3 Characters"
                    };
                    return Ok(patResult);

                }
            }
            if (!string.IsNullOrEmpty(con.locsuffix))
            {
                con.locsuffix = "LOCS";
            }
            if (!string.IsNullOrEmpty(con.locsuffix))
            {
                if (con.locsuffix.Length > 4)
                {
                    ResultObject patResult = new ResultObject
                    {
                        Status = false,
                        StatusCode = StatusCodes.Status204NoContent,
                        token = null,
                        data = null,
                        Message = "locsuffix Must be 4 Characters"
                    };
                    return Ok(patResult);

                }
            }
            if (!string.IsNullOrEmpty(con.mailstate))
            {
                con.mailstate = "MA";
            }
            if (!string.IsNullOrEmpty(con.mailstate))
            {
                if (con.mailstate.Length > 2)
                {
                    ResultObject patResult = new ResultObject
                    {
                        Status = false,
                        StatusCode = StatusCodes.Status204NoContent,
                        token = null,
                        data = null,
                        Message = "mailstate Must be 2 Characters"
                    };
                    return Ok(patResult);

                }
            }
            if (!string.IsNullOrEmpty(con.mailstate))
            {
                if (con.mailstate.Length > 2)
                {
                    ResultObject patResult = new ResultObject
                    {
                        Status = false,
                        StatusCode = StatusCodes.Status204NoContent,
                        token = null,
                        data = null,
                        Message = "mailstate Must be 2 Characters"
                    };
                    return Ok(patResult);

                }
            }
            if (!string.IsNullOrEmpty(con.nbrhd))
            {
                con.nbrhd = "nbrh";
            }
            if (!string.IsNullOrEmpty(con.nbrhd))
            {
                if (con.nbrhd.Length > 4)
                {
                    ResultObject patResult = new ResultObject
                    {
                        Status = false,
                        StatusCode = StatusCodes.Status204NoContent,
                        token = null,
                        data = null,
                        Message = "nbrhd Must be 4 Characters"
                    };
                    return Ok(patResult);

                }
            }
            if (!string.IsNullOrEmpty(con.penalty))
            {
                con.penalty = "Y";
            }
            if (!string.IsNullOrEmpty(con.penalty))
            {
                if (con.penalty.Length > 1)
                {
                    ResultObject patResult = new ResultObject
                    {
                        Status = false,
                        StatusCode = StatusCodes.Status204NoContent,
                        token = null,
                        data = null,
                        Message = "penalty Must be 1 Character"
                    };
                    return Ok(patResult);

                }
            }
            if (!string.IsNullOrEmpty(con.status))
            {
                con.status = "U";
            }
            if (!string.IsNullOrEmpty(con.status))
            {
                if (con.status.Length > 1)
                {
                    ResultObject patResult = new ResultObject
                    {
                        Status = false,
                        StatusCode = StatusCodes.Status204NoContent,
                        token = null,
                        data = null,
                        Message = "status Must be 1 Character"
                    };
                    return Ok(patResult);

                }
            }
            if (!string.IsNullOrEmpty(con.phone))
            {
                if (con.phone.Length > 8)
                {
                    ResultObject patResult = new ResultObject
                    {
                        Status = false,
                        StatusCode = StatusCodes.Status204NoContent,
                        token = null,
                        data = null,
                        Message = "phone  Must be 8 Character"
                    };
                    return Ok(patResult);
                }
            }
            if (string.IsNullOrEmpty(con.busntype))
            {
                con.taxcode = "ACC";
            }
            if (string.IsNullOrEmpty(con.taxcode))
            {
                con.taxcode = "301";
            }

            if (!string.IsNullOrEmpty(con.taxcode))
            {
                if (con.taxcode.Length > 3)
                {
                    ResultObject patResult = new ResultObject
                    {
                        Status = false,
                        StatusCode = StatusCodes.Status204NoContent,
                        token = null,
                        data = null,
                        Message = "taxcode Must be 3 Character"
                    };
                    return Ok(patResult);

                }
            }
            //con.RowVer=new byte[4];

            _context.Add(con);

            try
            {
                await _context.SaveChangesAsync();
                taxPayers = await _context.TaxPayer?.ToListAsync();
                ResultObject successResult = new ResultObject
                {
                    Status = true,
                    StatusCode = StatusCodes.Status200OK,
                    token = null,
                    data = con,
                    Message = "Added successfully"
                };
                return Ok(successResult);

            }
            catch (DbUpdateConcurrencyException ex)
            {
                ResultObject patResult = new ResultObject
                {
                    Status = false,
                    StatusCode = StatusCodes.Status204NoContent,
                    token = null,
                    data = null,
                    Message = ex.Message
                };
                return Ok(patResult);
            }
            catch (Exception ex)
            {
                ResultObject patResult = new ResultObject
                {
                    Status = false,
                    StatusCode = StatusCodes.Status204NoContent,
                    token = null,
                    data = null,
                    Message = ex.Message + ex?.StackTrace?.ToString()
                };
                return Ok(patResult);
            }

        }


        [HttpGet("/rrc/api/[controller]/[action]")]
        [ProducesResponseType(typeof(Towns), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetLookUPValues(string townId = "")
        {


            if (Request.Headers.TryGetValue("townId", out StringValues townname))
            {
                townId = townname;
            }

            List<pricingManual> manuals = new List<pricingManual>();
            string dbame = string.Empty;
            try
            {


                dbame = "RRC";


                var connectionStringRoot = _connectionStringProvider.GetConnectionString(dbame);

                _context = DbContextFactory.Create(connectionStringRoot);
                manuals = await _context.pricingManual.ToListAsync();
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }

            if (!string.IsNullOrEmpty(townId))
            {
                dbame = "RRC_" + townId.TrimStart().TrimEnd();
            }
            else
            {
                dbame = "RRC_Agawam";
            }
            var connectionString = _connectionStringProvider.GetConnectionString(dbame);

            _context = DbContextFactory.Create(connectionString);


            try
            {
                LookUPData lookUPData = new LookUPData();


                // manuals = manuals.DistinctBy(x => x.unitcost).ToList();
                List<pricingManualP> usersP = new List<pricingManualP>();

                // manuals = manuals.Take(100).ToList();
                var propertyType = await _context.propertyType.ToListAsync();
                List<propertyTypeP> propertyTypeP = new List<propertyTypeP>();
                List<TaxCode> taxCodes = new List<TaxCode>();
                List<Penalty> penalties = new List<Penalty>();
                List<Status> statuses = new List<Status>();
                var Deprec = await _context.Deprec.ToListAsync();
                List<DeprecP> DeprecP = new List<DeprecP>();

                taxCodes.Add(new TaxCode() { descript = "ACCOUNTING", entrydescval = "ACCT      -ACCOUNTING", entryval = "ACC" });
                taxCodes.Add(new TaxCode() { descript = "ADVERTISING", entrydescval = "ADVR      -ADVERTISING", entryval = "ADV" });
                taxCodes.Add(new TaxCode() { descript = "AMUSEMENT/ARCADE/PARK", entrydescval = "AMUZ      -AMUSEMENT/ARCADE/PARK", entryval = "AMU" });

                List<BusinessType> businessTypes = new List<BusinessType>();
                businessTypes.Add(new BusinessType() { descript = "2", entrydescval = "501 -INDIVIDUAL, PARTNERSHIP", entryval = "501" });
                businessTypes.Add(new BusinessType() { descript = "3", entrydescval = "502 -CORPORATION (DOMESTIC/FOREIGN)", entryval = "502" });

                businessTypes.Add(new BusinessType() { descript = "4", entrydescval = "503 -MANUFACTURING CORPORATION (M)", entryval = "503" });
                penalties.Add(new Penalty() { descript = "2", entrydescval = "Yes", entryval = "Y" });
                penalties.Add(new Penalty() { descript = "3", entrydescval = "No", entryval = "N" });
                statuses.Add(new Status() { descript = "2", entrydescval = "Delete", entryval = "D" });
                statuses.Add(new Status() { descript = "3", entrydescval = "Exempt", entryval = "E" });

                statuses.Add(new Status() { descript = "2", entrydescval = "Hold", entryval = "H" });
                statuses.Add(new Status() { descript = "3", entrydescval = "Non Taxable", entryval = "N" });

                statuses.Add(new Status() { descript = "2", entrydescval = "Retired", entryval = "R" });
                statuses.Add(new Status() { descript = "3", entrydescval = "Under Taxable", entryval = "U" });



                foreach (var stat in propertyType)
                {
                    propertyTypeP.Add(new Models.propertyTypeP() { descript = stat.descript, entrydescval = stat.descript, exemption = stat.exemption, proptype = stat.proptype });
                }


                foreach (var stat in manuals)
                {
                    usersP.Add(new Models.pricingManualP() { descript = stat.descript, entrydescval = stat.descript, category = stat.category, PMYear = stat.PMYear, pricecode = stat.pricecode, unitcost = stat.unitcost });

                }


                foreach (var stat in Deprec)
                {
                    DeprecP.Add(new Models.DeprecP() { entrydescval = stat.cond, cond = stat.cond, age = stat.age, Dpercent = stat.Dpercent });

                }
                if (DeprecP?.Count > 0)
                {
                    DeprecP = DeprecP.DistinctBy(x => x.age).ToList();
                }


                lookUPData.taxcode = taxCodes;
                lookUPData.propertyTypeP = propertyTypeP;
                lookUPData.pricingManualP = usersP;
                lookUPData.businesstype = businessTypes;
                lookUPData.taxcode = taxCodes;
                lookUPData.penalty = penalties;
                lookUPData.DeprecP = DeprecP;
                lookUPData.status = statuses;
                if (manuals.Count() <= 0)
                {

                    ResultObject patResult = new ResultObject
                    {
                        Status = false,
                        StatusCode = StatusCodes.Status401Unauthorized,
                        token = null,
                        Message = "No Taxpayers exists",
                        data = null
                    };
                    return Ok(patResult);
                }


                ResultObject patResult1 = new ResultObject
                {
                    Status = true,
                    StatusCode = StatusCodes.Status200OK,
                    token = null,
                    Message = "Data Found",
                    data = lookUPData
                };
                return Ok(patResult1);

            }
            catch (Exception ex)
            {
                ResultObject patResult = new ResultObject
                {
                    Status = true,
                    StatusCode = StatusCodes.Status422UnprocessableEntity,
                    token = null,
                    Message = ex.StackTrace,
                    data = null
                };
                return Ok(patResult);
            }

        }

        [HttpPost("/rrc/api/[controller]/[action]")]
        public async Task<IActionResult> AddProperty(int isUpdate, property con, string townId = "")
        {

            if (Request.Headers.TryGetValue("townId", out StringValues townname))
            {
                townId = townname;
            }
            string dbame = string.Empty;
            if (!string.IsNullOrEmpty(townId))
            {
                dbame = "RRC_" + townId.TrimStart().TrimEnd();
            }
            else
            {
                dbame = "RRC_Agawam";
            }
            var connectionString = _connectionStringProvider.GetConnectionString(dbame);

            _context = DbContextFactory.Create(connectionString);
            try
            {
                if (isUpdate == 1)
                {
                    var taxPayerNumber = await _context?.property.Where(u => u.PropertyNo == con.PropertyNo).ToListAsync();
                    taxPayerNumber[0].PropertyNo = con.PropertyNo;
                    taxPayerNumber[0].deprecode = con.deprecode;
                    taxPayerNumber[0].deptotal = con.deptotal;
                    taxPayerNumber[0].descrption = con.descrption;
                    taxPayerNumber[0].proptype = con.proptype;
                    taxPayerNumber[0].status = con.status;
                    taxPayerNumber[0].Acquired = con.Acquired;
                    taxPayerNumber[0].replmtcost = con.replmtcost;
                    taxPayerNumber[0].itemcost = con.itemcost;
                    taxPayerNumber[0].EditDate = con.EditDate;
                    taxPayerNumber[0].EditUser = con.EditUser;
                    _context.Update(taxPayerNumber[0]);

                    try
                    {
                        await _context.SaveChangesAsync();
                        ResultObject successResult = new ResultObject
                        {
                            Status = true,
                            StatusCode = StatusCodes.Status200OK,
                            token = null,
                            data = taxPayerNumber[0],
                            Message = "Updated successfully"
                        };
                        return Ok(successResult);

                    }
                    catch (DbUpdateConcurrencyException ex)
                    {
                        ResultObject patResult = new ResultObject
                        {
                            Status = false,
                            StatusCode = StatusCodes.Status204NoContent,
                            token = null,
                            data = null,
                            Message = ex.Message
                        };
                        return Ok(patResult);
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }
            List<property> properties = new List<property>();
            if (!string.IsNullOrEmpty(con.deprecode))
            {
                if (con.deprecode.Length > 2)
                {
                    ResultObject patResult = new ResultObject
                    {
                        Status = false,
                        StatusCode = StatusCodes.Status204NoContent,
                        token = null,
                        data = null,
                        Message = "deprecode Must be 2 Character"
                    };
                    return Ok(patResult);

                }
            }

            if (!string.IsNullOrEmpty(con.status))
            {
                if (con.status.Length > 2)
                {
                    ResultObject patResult = new ResultObject
                    {
                        Status = false,
                        StatusCode = StatusCodes.Status204NoContent,
                        token = null,
                        data = null,
                        Message = "status Must be 1 Character"
                    };
                    return Ok(patResult);

                }
            }

            if (!string.IsNullOrEmpty(con.pricecode))
            {
                if (con.pricecode.Length > 5)
                {
                    ResultObject patResult = new ResultObject
                    {
                        Status = false,
                        StatusCode = StatusCodes.Status204NoContent,
                        token = null,
                        data = null,
                        Message = "pricecode Must be 5 Character"
                    };
                    return Ok(patResult);

                }
            }

            if (!string.IsNullOrEmpty(con.proptype))
            {
                if (con.proptype.Length > 2)
                {
                    ResultObject patResult = new ResultObject
                    {
                        Status = false,
                        StatusCode = StatusCodes.Status204NoContent,
                        token = null,
                        data = null,
                        Message = "proptype Must be 2 Character"
                    };
                    return Ok(patResult);

                }
            }

            //con.RowVer=new byte[4];

            _context.Add(con);


            try
            {
                await _context.SaveChangesAsync();
                properties = await _context?.property?.ToListAsync();
                ResultObject successResult = new ResultObject
                {
                    Status = true,
                    StatusCode = StatusCodes.Status200OK,
                    token = null,
                    data = properties,
                    Message = "Property added successfully"
                };
                return Ok(successResult);

            }
            catch (DbUpdateConcurrencyException ex)
            {
                ResultObject patResult = new ResultObject
                {
                    Status = false,
                    StatusCode = StatusCodes.Status204NoContent,
                    token = null,
                    data = null,
                    Message = ex.Message
                };
                return Ok(patResult);
            }
            catch (Exception ex)
            {
                ResultObject patResult = new ResultObject
                {
                    Status = false,
                    StatusCode = StatusCodes.Status204NoContent,
                    token = null,
                    data = null,
                    Message = ex.Message
                };
                return Ok(patResult);
            }

        }

    }

}
