using Microsoft.AspNetCore.Mvc;
using ChatApp.Models;
using System.Diagnostics;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace ChatApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ChatContext _context;
        private const string UserKey = "USER_KEY";

        public HomeController(ILogger<HomeController> logger, ChatContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            var userName = HttpContext.Session.GetString(UserKey);

            if (string.IsNullOrEmpty(userName))
            {
                _logger.LogInformation("User is not signed in, redirecting to SignIn.");
                return RedirectToAction("SignIn");
            }

            try
            {
                var messages = _context.ChatMessages.OrderByDescending(m => m.CreatedOn).Take(50).ToList();

                if (messages == null || !messages.Any())
                {
                    _logger.LogWarning("No messages found in the database.");
                }

                var vm = new IndexVm
                {
                    UserName = userName,
                    Messages = messages
                };

                _logger.LogInformation($"Loaded {messages.Count} messages for user {userName}.");

                return View(vm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while loading messages.");
                return View(new IndexVm { UserName = userName, Messages = new List<ChatMessage>() });
            }
        }

        [HttpGet]
        public IActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SignIn(SignInVm vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var user = _context.ChatUsers.SingleOrDefault(u => u.UserName == vm.UserName);
            if (user == null || !VerifyPassword(vm.Password, user.Password))
            {
                ModelState.AddModelError("", "Invalid username or password");
                return View(vm);
            }

            SignInUser(vm.UserName);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(RegisterVm vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            if (_context.ChatUsers.Any(u => u.UserName == vm.UserName))
            {
                ModelState.AddModelError("UserName", "Username already exists");
                return View(vm);
            }

            var user = new ChatUser
            {
                UserName = vm.UserName,
                Password = HashPassword(vm.Password)
            };

            _context.ChatUsers.Add(user);
            _context.SaveChanges();

            SignInUser(vm.UserName);
            return RedirectToAction("Index");
        }

        private void SignInUser(string userName)
        {
            HttpContext.Session.SetString(key: UserKey, value: userName);
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }

        private bool VerifyPassword(string enteredPassword, string storedPassword)
        {
            var hashedEnteredPassword = HashPassword(enteredPassword);
            return hashedEnteredPassword == storedPassword;
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult TestSession()
        {
            HttpContext.Session.SetString("TestKey", "TestValue");
            var value = HttpContext.Session.GetString("TestKey");
            if (value == "TestValue")
            {
                return Content("Sesja działa poprawnie.");
            }
            else
            {
                return Content("Sesja nie działa.");
            }
        }
    }
}
