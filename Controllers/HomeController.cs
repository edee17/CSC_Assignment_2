using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TheLifeTimeTalents.Helpers;
using TheLifeTimeTalents.Models;
using TheLifeTimeTalents.Services;

namespace TheLifeTimeTalents.Controllers
{
    public class HomeController : Controller
    {
        private IUserService _userService;
        private readonly AppSettings _appSettings;

        public HomeController(
                  IUserService userService,
                  IOptions<AppSettings> appSettings)
        {
            _userService = userService;
            _appSettings = appSettings.Value;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        public IActionResult ViewAccount()
        {
            return View();
        }
        public IActionResult Profile()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Login([FromForm] IFormCollection data)
        {
            if ((data["uname"].ToString().Trim() == "") || (data["psw"].ToString().Trim() == ""))
            {
                //Make a ViewBag
                //ViewBag lifecycle only last for this request cycle.
                ViewBag.Message = "User name or password is missing";
                return View();
            }

            var userLoad = await _userService.AuthenticateAsync(data["uname"].ToString().ToLower(), data["psw"].ToString());

            if (userLoad == null)
            {
                ViewBag.Message = "User name or password is wrong";
                return View();
            }

            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);

            var identity = new ClaimsIdentity(new Claim[] {
                new Claim(ClaimTypes.Name, userLoad.UserID.ToString()),
                new Claim("ID", userLoad.UserID.ToString()),
                new Claim("FullName", userLoad.Username.ToString()),
                new Claim("RoleName", userLoad.Role.RoleName.ToString()),
                new Claim(ClaimTypes.Role, userLoad.Role.RoleName.ToString())
            }, "MyCookieAuthenticationScheme");
            //identity.AddClaim(new Claim(ClaimTypes.Name, user.Id));

            await HttpContext.SignInAsync("MyCookieAuthenticationScheme", new ClaimsPrincipal(identity));
            return Redirect("/Home/Index");
        }

        public async Task<IActionResult> RegisterUp([FromForm] IFormCollection data)
        {
            if ((data["uname"].ToString().Trim() == "") || (data["psw"].ToString().Trim() == "") || (data["email"].ToString().Trim() == ""))
            {
                //Make a ViewBag
                //ViewBag lifecycle only last for this request cycle.
                ViewBag.Message = "User name, email or password is missing";
                return View();
            }

            User newUser = new User();
            newUser.Username = data["uname"].ToString();
            newUser.Email = data["email"].ToString().ToLower();
            newUser.RoleID = 2;

            var userLoad = await _userService.CreateUser(newUser, data["psw"].ToString());

            return Redirect("/Home/Login");
        }
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("MyCookieAuthenticationScheme");
            return Redirect("/Home/Login");
        }
    }
}
