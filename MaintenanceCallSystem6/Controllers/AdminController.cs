using MaintenanceCallSystem6.Models;
using MaintenanceCallSystem6.ViewModels; // Ensure you have this namespace for the ViewModels
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // For Include and ToListAsync
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic; // For List
using MaintenanceCallSystem6.Data; // Adjust this if the namespace is different


namespace MaintenanceCallSystem6.Controllers
{
    public class AdminController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context; // Add your DbContext

        public AdminController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Dashboard()
        {
            return View();
        }

        [Route("Admin/[action]")]
        [HttpPost]
        public async Task<IActionResult> AssignAdminRole(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("Email is required.");
            }

            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return NotFound($"User with email {email} not found.");
            }

            if (!await _roleManager.RoleExistsAsync("Admin"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            if (await _userManager.IsInRoleAsync(user, "Admin"))
            {
                return Ok("User is already in the Admin role.");
            }

            var result = await _userManager.AddToRoleAsync(user, "Admin");

            if (result.Succeeded)
            {
                return Ok("Admin role assigned successfully.");
            }
            else
            {
                return BadRequest("Failed to assign admin role: " + string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }

        //public async Task<IActionResult> Reports()
        //{
        //    var completedCalls = await _context.MaintenanceCalls
        //        .Where(c => c.Status == "Completed" && c.DateAttended.HasValue)
        //        .Include(c => c.User) // Include User navigation property
        //        .ToListAsync();

        //    var technicianReports = completedCalls
        //        .GroupBy(c => c.TechnicianAssigned)
        //        .Select(group => new TechnicianReportViewModel
        //        {
        //            TechnicianName = group.Key,
        //            Calls = group.Select(call => new CallReportViewModel
        //            {
        //                Description = call.Description,
        //                DateLogged = call.DateLogged ?? DateTime.MinValue, // Default value if null
        //                DateAttended = call.DateAttended ?? DateTime.MinValue, // Default value if null
        //                TimeTaken = (call.DateAttended ?? DateTime.MinValue) - (call.DateLogged ?? DateTime.MinValue), // Handle nulls
        //                UserName = call.User != null ? call.User.UserName : "Unknown" // Check if User is not null
        //            }).ToList()
        //        })
        //        .ToList();

        //    return View(technicianReports);
        //}


    }
}

