using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.Numerics;
using System;
using TownsApi.Data;
using TownsApi.Models;
using static Azure.Core.HttpHeader;
using Microsoft.IdentityModel.Tokens;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

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

        [HttpGet("/rrc/api/[controller]/[action]")]
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

        [HttpGet("/rrc/api/[controller]/[action]")]
        [ProducesResponseType(typeof(Towns), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAllProperties(int accountNum)
        {
            List<property> users = new List<property>();
            try
            {
                if (accountNum != 0 || accountNum != null)
                {
                    users = await _context.property.Where(x => x.accountno == accountNum).ToListAsync();

                }
                else
                {
                    users = await _context.property.ToListAsync();

                }
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
        public async Task<IActionResult> GetAllTaxPayers()
        {
            try
            {
                var users = await _context.TaxPayer.ToListAsync();

                if (users.Count() <= 0)
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
        public async Task<IActionResult> AddTown(Towns con)
        {


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
        public async Task<IActionResult> AddTaxPayer(TaxPayer con)
        {
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
            con.busntype = con.busntype;
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
            if (!string.IsNullOrEmpty(con.Action))
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
            if (!string.IsNullOrEmpty(con.FOL))
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
            if (!string.IsNullOrEmpty(con.areacode))
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

            if (!string.IsNullOrEmpty(con.busntype))
            {
                con.busntype = "ACC";
            }
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
            if (!string.IsNullOrEmpty(con.datalister))
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
        public async Task<IActionResult> GetLookUPValues()
        {
            try
            {
                LookUPData lookUPData = new LookUPData();

                var users = await _context.pricingManual.ToListAsync();
                List<pricingManualP> usersP = new List<pricingManualP>();

                users = users.Take(100).ToList();
                var propertyType = await _context.propertyType.ToListAsync();
                List<propertyTypeP> propertyTypeP = new List<propertyTypeP>();
                List<TaxCode> taxCodes = new List<TaxCode>();
                List<Penalty> penalties = new List<Penalty>();
                List<Status> statuses = new List<Status>();
                var Deprec = await _context.Deprec.ToListAsync();
                List<DeprecP> DeprecP = new List<DeprecP>();

                taxCodes.Add(new TaxCode() { descript = "ACCOUNTING", entrydescval = "ACCT      -ACCOUNTING", entryval = "ACCT" });
                taxCodes.Add(new TaxCode() { descript = "ADVERTISING", entrydescval = "ADVR      -ADVERTISING", entryval = "ADVR" });
                taxCodes.Add(new TaxCode() { descript = "AMUSEMENT/ARCADE/PARK", entrydescval = "AMUZ      -AMUSEMENT/ARCADE/PARK", entryval = "AMUZ" });

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
                    propertyTypeP.Add(new Models.propertyTypeP() { descript =stat.descript, entrydescval =stat.descript, exemption =stat.exemption, proptype =stat.proptype});
                }


                foreach (var stat in users)
                {
                    usersP.Add(new Models.pricingManualP() { descript = stat.descript, entrydescval = stat.descript,category=stat.category,PMYear=stat.PMYear,pricecode=stat.pricecode,unitcost=stat.unitcost });

                }

               
                foreach (var stat in Deprec)
                {
                    DeprecP.Add(new Models.DeprecP() { entrydescval = stat.cond,cond=stat.cond, age= stat.age, Dpercent=stat.Dpercent  });

                }


                lookUPData.taxcode = taxCodes;
                lookUPData.propertyTypeP = propertyTypeP;
                lookUPData.pricingManualP = usersP;
                lookUPData.businesstype = businessTypes;
                lookUPData.taxcode = taxCodes;
                lookUPData.penalty = penalties;
                lookUPData.DeprecP = DeprecP;
                lookUPData.status = statuses;
                if (users.Count() <= 0)
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
        public async Task<IActionResult> AddProperty(property con)
        {
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
