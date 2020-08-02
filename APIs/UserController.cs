using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using TheLifeTimeTalents.Data;
using TheLifeTimeTalents.Helpers;
using TheLifeTimeTalents.Models;
using TheLifeTimeTalents.Services;

namespace TheLifeTimeTalents.APIs
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private ApplicationDbContext Database;
        private AppSettings _appSettings;
        private IUserService _userService;
        //Create a Constructor, so that the .NET engine can pass in the 
        //ApplicationDbContext object which represents the database session.
        public UserController(IUserService userService,
            ApplicationDbContext database,
            IOptions<AppSettings> appSettings)
        {
            //Reference the database object which is residing inside the Web App's
            //service container.
            //       _appSettings = appSettings.Value;
            Database = database;
            _userService = userService;
            _appSettings = appSettings.Value;

        }

        // GET: api/<UserController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            List<User> userList = new List<User>();

            try
            {

                userList = await _userService.GetAll();

            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            return Ok(new { response = userList });
        }

        // GET api/<UserController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {

            User user = new User();
            //       var foundOneUser = Database.Users
            //.Where(eachUser => eachUser.UserID == id).Single();
            try
            {
                user = await _userService.GetById(id);
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            return Ok(new { response = user });
        }

        // PUT api/<UserController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromForm] IFormCollection data)
        {
            int userId = int.Parse(User.FindFirst("UserID").Value);
            //string customMessage = "";
            //var OneUser = Database.Users
            //.Where(eachUser => eachUser.UserID == id).Single();
            User OneUser = new User();
            OneUser.Username = data["Name"];
            OneUser.Email = data["Email"];
            //OneUser.Password = data["Password"];
            OneUser.RoleID = int.Parse(data["roleID"]); //1

            try
            {
                //Database.SaveChanges();
                await _userService.UpdateUser(OneUser, id);
            }
            catch (AppException ex)
            {
                //customMessage = "The request could not be processed " +
                //"due to internal errors. Please, try it later";
                return BadRequest(new { message = ex.Message });
            }//End of try .. catch block on saving data
             //Send back an OK with 200 status code
            return Ok(new
            {
                message = "Updated User Data."
            });
        }
    }
}
