using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MaintenanceCallSystem6.Data;
using MaintenanceCallSystem6.Models;
using MaintenanceCallSystem6.ViewModels;
using BCrypt.Net;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;


using Microsoft.AspNetCore.Identity;


public class AccountController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;

    public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = new User
            {
                UserName = model.Email, // Identity uses UserName for login
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                PhoneNumber = model.PhoneNumber ?? "",
                Address = model.Address,
                City = model.City,
                Province = model.Province,
                PostalCode = model.PostalCode
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home"); // Or another page after registration
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
        return View(model);
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: true);
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "MaintenanceCall"); // Redirect to the desired page
            }
            else if (result.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "Your account is locked out.");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }
        }
        return View(model);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }














//2
//public class AccountController : Controller
//{
//    private readonly ApplicationDbContext _context;

//    public AccountController(ApplicationDbContext context)
//    {
//        _context = context;
//    }

//    [HttpGet]
//    public IActionResult Register()
//    {
//        return View();
//    }

//    [HttpPost]
//    public async Task<IActionResult> Register(RegisterViewModel model)
//    {
//        if (ModelState.IsValid)
//        {
//            var user = new User
//            {
//                FirstName = model.FirstName,
//                LastName = model.LastName,
//                Email = model.Email,
//                PhoneNumber = model.PhoneNumber,
//                Password = BCrypt.Net.BCrypt.HashPassword(model.Password),
//                Address = model.Address,
//                City = model.City,
//                Province = model.Province,
//                PostalCode = model.PostalCode
//            };

//            _context.Users.Add(user);
//            await _context.SaveChangesAsync();
//            return RedirectToAction("Login");
//        }
//        return View(model);
//    }

//    [HttpGet]
//    public IActionResult Login()
//    {
//        return View();
//    }

//    [HttpPost]
//    public async Task<IActionResult> Login(LoginViewModel model)
//    {
//        if (ModelState.IsValid)
//        {
//            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
//            if (user != null && BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
//            {
//                HttpContext.Session.SetString("UserId", user.Id.ToString());
//                return RedirectToAction("Index", "maintenanceCall");
//            }
//            ModelState.AddModelError("", "Invalid login attempt.");
//        }
//        return View(model);
//    }
//    [Authorize]
//    public async Task<IActionResult> Logout()
//    {
//        await HttpContext.SignOutAsync();
//        return RedirectToAction("Index", "Home");
//    }


    //[HttpPost]
    //public IActionResult Logout()
    //{
    //    HttpContext.Session.Remove("UserId");
    //    return RedirectToAction("Index", "Home");
    //}
    //}

    //using Microsoft.AspNetCore.Mvc;
    //using Microsoft.EntityFrameworkCore;
    //using MaintenanceCallSystem6.Data;
    //using MaintenanceCallSystem6.Models;
    //using MaintenanceCallSystem6.ViewModels;
    //using BCrypt.Net;
    //using System.Threading.Tasks;
    //using Microsoft.AspNetCore.Authentication;
    //using Microsoft.AspNetCore.Authorization;

    //public class AccountController : Controller
    //{
    //    private readonly ApplicationDbContext _context;

    //    public AccountController(ApplicationDbContext context)
    //    {
    //        _context = context;
    //    }

    //    // GET: Account/Register
    //    [HttpGet]
    //    public IActionResult Register()
    //    {
    //        return View();
    //    }

    //    // POST: Account/Register
    //    [HttpPost]
    //    public async Task<IActionResult> Register(RegisterViewModel model)
    //    {
    //        if (ModelState.IsValid)
    //        {
    //            var userExists = await _context.Users.AnyAsync(u => u.Email == model.Email);

    //            if (userExists)
    //            {
    //                ModelState.AddModelError("", "Email already registered.");
    //                return View(model);
    //            }

    //            var user = new User
    //            {
    //                FirstName = model.FirstName,
    //                LastName = model.LastName,
    //                Email = model.Email,
    //                PhoneNumber = model.PhoneNumber,
    //                Password = BCrypt.Net.BCrypt.HashPassword(model.Password),
    //                Address = model.Address,
    //                City = model.City,
    //                Province = model.Province,
    //                PostalCode = model.PostalCode
    //            };

    //            try
    //            {
    //                _context.Users.Add(user);
    //                await _context.SaveChangesAsync();
    //                return RedirectToAction("Login");
    //            }
    //            catch (DbUpdateException)
    //            {
    //                ModelState.AddModelError("", "Error saving user. Please try again.");
    //            }
    //        }
    //        return View(model);
    //    }

    //    // GET: Account/Login
    //    [HttpGet]
    //    public IActionResult Login()
    //    {
    //        return View();
    //    }

    //    // POST: Account/Login
    //    [HttpPost]
    //    public async Task<IActionResult> Login(LoginViewModel model)
    //    {
    //        if (ModelState.IsValid)
    //        {
    //            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);

    //            if (user != null && BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
    //            {
    //                HttpContext.Session.SetString("UserId", user.Id.ToString());
    //                return RedirectToAction("Index", "Home");
    //            }

    //            ModelState.AddModelError("", "Invalid login attempt.");
    //        }
    //        return View(model);
    //    }

    //    // POST: Account/Logout
    //    //[HttpPost]
    //    //public IActionResult Logout()
    //    //{
    //    //    HttpContext.Session.Remove("UserId");
    //    //    return RedirectToAction("Index", "Home");
    //    //}

    //[Authorize]
    //public async Task<IActionResult> Logout()
    //{
    //    await HttpContext.SignOutAsync();
    //    return RedirectToAction("Index", "Home");
    //}
}

