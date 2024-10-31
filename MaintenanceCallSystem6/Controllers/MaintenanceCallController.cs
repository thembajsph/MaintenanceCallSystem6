using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MaintenanceCallSystem6.Models;
using MaintenanceCallSystem6.ViewModels;
using MaintenanceCallSystem6.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace MaintenanceCallSystem6.Controllers
{
    [Authorize]
    public class MaintenanceCallController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public MaintenanceCallController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: MaintenanceCall
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var calls = await _context.MaintenanceCalls
                .Select(call => new MaintenanceCall
                {
                    Id = call.Id,
                    Description = call.Description,
                    Status = call.Status ?? "Unknown",
                    TechnicianAssigned = call.TechnicianAssigned ?? "Unassigned",
                    DateAttended = call.DateAttended,
                    FirstName = call.FirstName ?? "Unknown",
                    LastName = call.LastName ?? "Unknown",
                    DateLogged = call.DateLogged,
                    Address = call.Address ?? "No Address",
                    City = call.City ?? "Default City",
                    Province = call.Province ?? "Unknown Province",
                    PostalCode = call.PostalCode ?? "Unknown",
                })
                .ToListAsync();

            return View(calls);
        }

        // GET: MaintenanceCall/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: MaintenanceCall/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MaintenanceCall model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                model.FirstName = user.FirstName ?? "Unknown";
                model.LastName = user.LastName ?? "Unknown";
                model.Address = user.Address ?? "No Address";
                model.City = model.City ?? "Default City";
                model.Province = user.Province ?? "Unknown Province";
                model.PostalCode = user.PostalCode ?? "Unknown";
            }

            if (!User.IsInRole("Admin"))
            {
                model.Status = "Pending";
                model.TechnicianAssigned = "Unassigned";
                model.DateAttended = null;
            }

            _context.MaintenanceCalls.Add(model);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: MaintenanceCall/Edit/5
        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var maintenanceCall = await _context.MaintenanceCalls.FindAsync(id);
            if (maintenanceCall == null)
            {
                return NotFound();
            }

            maintenanceCall.City ??= "Default City";
            maintenanceCall.Province ??= "Unknown Province";
            maintenanceCall.TechnicianAssigned ??= "Unassigned";

            ViewData["CanEditRestrictedFields"] = User.IsInRole("Admin");

            return View(maintenanceCall);
        }

        // POST: MaintenanceCall/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Description,Status,TechnicianAssigned,DateAttended,FirstName,LastName,DateLogged,Address,City,Province,PostalCode")] MaintenanceCall maintenanceCall)
        {
            if (id != maintenanceCall.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(maintenanceCall);
            }

            try
            {
                var existingCall = await _context.MaintenanceCalls.FindAsync(id);

                if (existingCall == null)
                {
                    return NotFound();
                }

                if (!User.IsInRole("Admin"))
                {
                    existingCall.Description = maintenanceCall.Description;
                    existingCall.DateLogged = maintenanceCall.DateLogged;
                    existingCall.FirstName = maintenanceCall.FirstName;
                    existingCall.LastName = maintenanceCall.LastName;
                    existingCall.Address = maintenanceCall.Address ?? "No Address";
                    existingCall.City = maintenanceCall.City ?? "Default City";
                    existingCall.Province = maintenanceCall.Province ?? "Unknown Province";
                    existingCall.PostalCode = maintenanceCall.PostalCode ?? "Unknown";
                }
                else
                {
                    existingCall.Description = maintenanceCall.Description;
                    existingCall.Status = maintenanceCall.Status ?? "Unknown";
                    existingCall.TechnicianAssigned = maintenanceCall.TechnicianAssigned ?? "Unassigned";
                    existingCall.DateAttended = maintenanceCall.DateAttended;
                    existingCall.FirstName = maintenanceCall.FirstName ?? "Unknown";
                    existingCall.LastName = maintenanceCall.LastName ?? "Unknown";
                    existingCall.DateLogged = maintenanceCall.DateLogged;
                    existingCall.Address = maintenanceCall.Address ?? "No Address";
                    existingCall.City = maintenanceCall.City ?? "Default City";
                    existingCall.Province = maintenanceCall.Province ?? "Unknown Province";
                    existingCall.PostalCode = maintenanceCall.PostalCode ?? "Unknown";
                }

                _context.Update(existingCall);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MaintenanceCallExists(maintenanceCall.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: MaintenanceCall/Delete/5
        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var call = await _context.MaintenanceCalls
                .FirstOrDefaultAsync(m => m.Id == id);

            if (call == null) return NotFound();

            return View(call);
        }

        // POST: MaintenanceCall/Delete/5
        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var call = await _context.MaintenanceCalls.FindAsync(id);
            if (call == null)
            {
                return NotFound();
            }

            try
            {
                _context.MaintenanceCalls.Remove(call);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MaintenanceCallExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                // Log or handle other types of exceptions
                // _logger.LogError(ex, "An error occurred while deleting the maintenance call.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }

            return RedirectToAction(nameof(Index));
        }

        private bool MaintenanceCallExists(int id)
        {
            return _context.MaintenanceCalls.Any(e => e.Id == id);
        }
    }
}






//3 now correct
//namespace MaintenanceCallSystem6.Controllers
//{
//    [Authorize]
//    public class MaintenanceCallController : Controller
//    {
//        private readonly ApplicationDbContext _context;
//        private readonly UserManager<User> _userManager;

//        public MaintenanceCallController(ApplicationDbContext context, UserManager<User> userManager)
//        {
//            _context = context;
//            _userManager = userManager;
//        }

//        // GET: MaintenanceCall
//        [HttpGet]
//        public IActionResult Index()
//        {
//            var calls = _context.MaintenanceCalls.ToList();
//            return View(calls);
//        }

//        // GET: MaintenanceCall/Create
//        [HttpGet]
//        public IActionResult Create()
//        {
//            return View();
//        }

//        // POST: MaintenanceCall/Create
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Create(MaintenanceCall model)
//        {
//            if (User.IsInRole("Admin"))
//            {
//                // For Admin users, all fields are required
//                if (ModelState.IsValid)
//                {
//                    _context.MaintenanceCalls.Add(model);
//                    await _context.SaveChangesAsync();
//                    return RedirectToAction("Index");
//                }
//            }
//            else
//            {
//                // For non-admin users, remove validation for specific fields
//                ModelState.Remove("Status");
//                ModelState.Remove("TechnicianAssigned");
//                ModelState.Remove("DateAttended");

//                // Manually set fields for non-admin users
//                if (ModelState.IsValid)
//                {
//                    // Get the current user
//                    var user = await _userManager.GetUserAsync(User);

//                    if (user != null)
//                    {
//                        // Populate FirstName and LastName from the user
//                        model.FirstName = user.FirstName;
//                        model.LastName = user.LastName;
//                    }

//                    _context.MaintenanceCalls.Add(model);
//                    await _context.SaveChangesAsync();
//                    return RedirectToAction("Index");
//                }
//            }

//            // Return the view with the model if not valid
//            return View(model);
//        }


//        // GET: MaintenanceCall/Edit/5
//        [HttpGet]
//        public async Task<IActionResult> Edit(int? id)
//        {
//            if (id == null)
//            {
//                return NotFound();
//            }

//            var maintenanceCall = await _context.MaintenanceCalls.FindAsync(id);
//            if (maintenanceCall == null)
//            {
//                return NotFound();
//            }

//            // Check if the user is not admin and restrict fields accordingly
//            if (!User.IsInRole("Admin"))
//            {
//                // Set fields that normal users cannot edit to be null
//                maintenanceCall.Status = null;
//                maintenanceCall.TechnicianAssigned = null;
//                maintenanceCall.DateAttended = null;
//            }

//            return View(maintenanceCall);
//        }

//        // POST: MaintenanceCall/Edit/5
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Edit(int id, [Bind("Id,Description,Status,TechnicianAssigned,DateAttended,FirstName,LastName,DateLogged")] MaintenanceCall maintenanceCall)
//        {
//            if (id != maintenanceCall.Id)
//            {
//                return NotFound();
//            }

//            if (ModelState.IsValid)
//            {
//                try
//                {
//                    var existingCall = await _context.MaintenanceCalls.FindAsync(id);

//                    if (existingCall == null)
//                    {
//                        return NotFound();
//                    }

//                    if (!User.IsInRole("Admin"))
//                    {
//                        // Update only the fields allowed for normal users
//                        existingCall.Description = maintenanceCall.Description;
//                        existingCall.DateLogged = maintenanceCall.DateLogged;
//                        existingCall.FirstName = maintenanceCall.FirstName;
//                        existingCall.LastName = maintenanceCall.LastName;
//                    }
//                    else
//                    {
//                        // Admin can update all fields
//                        existingCall.Description = maintenanceCall.Description;
//                        existingCall.Status = maintenanceCall.Status;
//                        existingCall.TechnicianAssigned = maintenanceCall.TechnicianAssigned;
//                        existingCall.DateAttended = maintenanceCall.DateAttended;
//                        existingCall.FirstName = maintenanceCall.FirstName;
//                        existingCall.LastName = maintenanceCall.LastName;
//                        existingCall.DateLogged = maintenanceCall.DateLogged;
//                    }

//                    _context.Update(existingCall);
//                    await _context.SaveChangesAsync();
//                }
//                catch (DbUpdateConcurrencyException)
//                {
//                    if (!MaintenanceCallExists(maintenanceCall.Id))
//                    {
//                        return NotFound();
//                    }
//                    else
//                    {
//                        throw;
//                    }
//                }
//                return RedirectToAction(nameof(Index));
//            }
//            return View(maintenanceCall);
//        }

//        // GET: MaintenanceCall/Delete/5
//        [HttpGet]
//        public async Task<IActionResult> Delete(int? id)
//        {
//            if (id == null) return NotFound();

//            var call = await _context.MaintenanceCalls.FindAsync(id);
//            if (call == null) return NotFound();

//            return View(call);
//        }

//        // POST: MaintenanceCall/Delete/5
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> DeleteConfirmed(int id)
//        {
//            var call = await _context.MaintenanceCalls.FindAsync(id);
//            if (call == null)
//            {
//                return NotFound();
//            }

//            _context.MaintenanceCalls.Remove(call);
//            await _context.SaveChangesAsync();
//            return RedirectToAction(nameof(Index));
//        }

//        private bool MaintenanceCallExists(int id)
//        {
//            return _context.MaintenanceCalls.Any(e => e.Id == id);
//        }
//    }
//}

//namespace MaintenanceCallSystem6.Controllers
//{
//    //[Authorize(Policy = "AdminOnly")]
//    public class MaintenanceCallController : Controller
//    {
//        private readonly ApplicationDbContext _context;
//        private readonly UserManager<User> _userManager;

//        public MaintenanceCallController(ApplicationDbContext context, UserManager<User> userManager)
//        {
//            _context = context;
//            _userManager = userManager;
//        }

//        // GET: MaintenanceCall
//        [HttpGet]
//        public IActionResult Index()
//        {
//            var calls = _context.MaintenanceCalls.ToList();
//            return View(calls);
//        }

//        // GET: MaintenanceCall/Create
//        //[Authorize(Roles = "Admin")]
//        [HttpGet]
//        public IActionResult Create()
//        {
//            return View();
//        }

//        // POST: MaintenanceCall/Create
//        //[Authorize(Roles = "Admin")]
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Create(MaintenanceCall model)
//        {
//            if (ModelState.IsValid)
//            {
//                // Get the current user
//                var user = await _userManager.GetUserAsync(User);

//                if (user != null)
//                {
//                    // Populate FirstName and LastName from the user
//                    model.FirstName = user.FirstName;
//                    model.LastName = user.LastName;
//                }

//                _context.MaintenanceCalls.Add(model);
//                await _context.SaveChangesAsync();
//                return RedirectToAction("Index");
//            }
//            return View(model);
//        }

//        // GET: MaintenanceCall/Edit/5
//        //[Authorize(Roles = "Admin")]
//        [HttpGet]
//        public async Task<IActionResult> Edit(int? id)
//        {
//            if (id == null)
//            {
//                return NotFound();
//            }

//            var maintenanceCall = await _context.MaintenanceCalls.FindAsync(id);
//            if (maintenanceCall == null)
//            {
//                return NotFound();
//            }
//            return View(maintenanceCall);
//        }

//        // POST: MaintenanceCall/Edit/5
//        [Authorize(Roles = "Admin")]
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Edit(int id, [Bind("Id,Description,Status,TechnicianAssigned,DateAttended,FirstName,LastName,DateLogged")] MaintenanceCall maintenanceCall)
//        {
//            if (id != maintenanceCall.Id)
//            {
//                return NotFound();
//            }

//            if (ModelState.IsValid)
//            {
//                try
//                {
//                    _context.Update(maintenanceCall);
//                    await _context.SaveChangesAsync();
//                }
//                catch (DbUpdateConcurrencyException)
//                {
//                    if (!MaintenanceCallExists(maintenanceCall.Id))
//                    {
//                        return NotFound();
//                    }
//                    else
//                    {
//                        throw;
//                    }
//                }
//                return RedirectToAction(nameof(Index));
//            }
//            return View(maintenanceCall);
//        }

//        private bool MaintenanceCallExists(int id)
//        {
//            return _context.MaintenanceCalls.Any(e => e.Id == id);
//        }

//        // GET: MaintenanceCall/Delete/5
//        [Authorize(Roles = "Admin")]
//        [HttpGet]
//        public async Task<IActionResult> Delete(int? id)
//        {
//            if (id == null) return NotFound();

//            var call = await _context.MaintenanceCalls.FindAsync(id);
//            if (call == null) return NotFound();

//            return View(call);
//        }

//        [HttpPost]
//        [Authorize(Roles = "Admin")]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> DeleteConfirmed(int id)
//        {
//            var call = await _context.MaintenanceCalls.FindAsync(id);
//            if (call == null)
//            {
//                return NotFound();
//            }

//            _context.MaintenanceCalls.Remove(call);
//            await _context.SaveChangesAsync();
//            return RedirectToAction(nameof(Index));
//        }

//        /*[Authorize(Roles = "Admin")]*/  // Ensure only admins can access this controller
//        public class AdminController : Controller
//        {
//            public IActionResult Index()
//            {
//                return View();
//            }
//        }

//    }
//}

