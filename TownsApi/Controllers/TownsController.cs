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




            if (!string.IsNullOrEmpty(con.nbrhd))
            {
                con.nbrhd = con.nbrhd;
            }
            if (!string.IsNullOrEmpty(con.owner))
            {
                con.owner = con.owner;
            }

            con.inputdate = con.inputdate;

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
            //con.RowVer=new byte[4];

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


        [HttpGet("/rrc/api/[controller]/[action]")]
        [ProducesResponseType(typeof(Towns), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetLookUPValues()
        {
            try
            {
                LookUPData lookUPData = new LookUPData();
                var users = await _context.pricingManual.ToListAsync();
                users = users.Take(100).ToList();
                var propertyType = await _context.propertyType.ToListAsync();
                var Deprec = await _context.Deprec.ToListAsync();
                lookUPData.propertyType = propertyType;
                lookUPData.pricingManual = users;
                lookUPData.Deprec= Deprec;
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
    }

}
