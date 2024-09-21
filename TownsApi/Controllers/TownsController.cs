using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TownsApi.Data;
using TownsApi.Models;

namespace TownsApi.Controllers
{
    [Route("towns/api/[controller]")]
    [ApiController]
    public class TownsController : Controller
    {
        private readonly TownDBContext _context;
        public TownsController(TownDBContext context)
        {
            _context = context;
        }

        [HttpGet("/inouts/api/[controller]/[action]")]
        [ProducesResponseType(typeof(Towns), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAllTowns()
        {
            try
            {
                var users = await _context.Towns.ToListAsync();

                if (users.Count() <= 0)
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
                    data = users
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
        //[HttpGet("/inouts/api/[controller]/[action]")]
        //[Authorize]
        //public async Task<IActionResult> GetTownDetails(int id)
        //{

        //    var newUser = await _context.Towns.Where(u => u.TownId == id).ToListAsync();
        //    if (newUser.Count() == 0)
        //    {
        //        ResultObject patResult = new ResultObject
        //        {
        //            Status = false,
        //            StatusCode = StatusCodes.Status204NoContent,
        //            token = null,
        //            data = null,
        //            Message = "No town exists with this Id"
        //        };
        //        return Ok(patResult);
        //    }

        //    try
        //    {
        //        ResultObject successResult = new ResultObject
        //        {
        //            Status = true,
        //            StatusCode = StatusCodes.Status200OK,
        //            token = null,
        //            data = newUser,
        //            Message = "Employee deleted successfully"
        //        };
        //        return Ok(successResult);

        //    }
        //    catch (DbUpdateConcurrencyException ex)
        //    {
        //        ResultObject patResult = new ResultObject
        //        {
        //            Status = false,
        //            StatusCode = StatusCodes.Status204NoContent,
        //            token = null,
        //            data = null,
        //            Message = ex.Message
        //        };
        //        return Ok(patResult);
        //    }
        //    catch (Exception ex)
        //    {
        //        ResultObject patResult = new ResultObject
        //        {
        //            Status = false,
        //            StatusCode = StatusCodes.Status204NoContent,
        //            token = null,
        //            data = null,
        //            Message = ex.Message
        //        };
        //        return Ok(patResult);
        //    }

        //}
    }

}
