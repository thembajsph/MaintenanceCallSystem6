using MaintenanceCallSystem6.Data; // Assuming your DbContext is in the Data folder
using MaintenanceCallSystem6.Models; // Assuming Technician and MaintenanceCall models are here
using MaintenanceCallSystem6.ViewModels; // Assuming you have a view model to handle assignments
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using Microsoft.EntityFrameworkCore; // Added for enabling sensitive data logging

namespace MaintenanceCallSystem6.Controllers
{
    public class TechnicianController : Controller
    {
        private readonly ApplicationDbContext _context; // Assuming ApplicationDbContext is your DbContext

        public TechnicianController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var viewModel = new TechnicianCompositeViewModel
            {
                Technicians = _context.Technicians
                    .Select(t => new TechnicianViewModel
                    {
                        Id = t.Id,
                        TechnicianName = t.TechnicianName,
                        Department = t.Department,
                        Available = t.Available
                    }).ToList(),

                MaintenanceCalls = _context.MaintenanceCalls
                    .Where(m => m.TechnicianAssigned == null)
                    .ToList()
            };

            return View(viewModel);
        }





        // GET: Technician/AssignTechnician/1
        public IActionResult AssignTechnician(int callId)
        {
            var maintenanceCall = _context.MaintenanceCalls.Find(callId);
            if (maintenanceCall == null)
            {
                return NotFound();
            }

            var department = GetDepartmentForIssueType(maintenanceCall.IssueTypeEnum);
            Console.WriteLine($"Selected Department: {department}"); // Debugging log

            var technicians = _context.Technicians
                .Where(t => t.Department == department && t.Available)
                .ToList();

            var technicianSelectList = technicians.Select(t => new SelectListItem
            {
                Value = t.Id.ToString(),
                Text = t.TechnicianName
            }).ToList();

            var viewModel = new TechnicianAssignViewModel
            {
                MaintenanceCall = maintenanceCall,
                Technicians = technicianSelectList // Ensure list is passed to view
            };

            return View(viewModel);
        }

        // POST: Technician/AssignTechnician
        // POST: Technician/AssignTechnician
        // POST: Technician/AssignTechnician
        [HttpPost]
        public IActionResult AssignTechnician(TechnicianAssignViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Check if MaintenanceCall is null
                if (model.MaintenanceCall == null)
                {
                    return NotFound();  // Or handle appropriately
                }

                // Check if IssueTypeEnum is null or empty
                var department = GetDepartmentForIssueType(model.MaintenanceCall.IssueTypeEnum);
                if (string.IsNullOrEmpty(department))
                {
                    ModelState.AddModelError(string.Empty, "Invalid Issue Type.");
                    return View(model);
                }

                // Check if TechnicianId is valid
                var technician = _context.Technicians.Find(model.TechnicianId);
                if (technician == null)
                {
                    ModelState.AddModelError(string.Empty, "Selected technician not found.");
                    return View(model);
                }

                var maintenanceCall = _context.MaintenanceCalls.Find(model.MaintenanceCall.Id);
                if (maintenanceCall == null)
                {
                    return NotFound();  // Handle missing maintenance call
                }

                maintenanceCall.TechnicianAssigned = technician.TechnicianName;
                technician.Available = false;

                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            // If validation fails, repopulate technicians
            var departmentForIssue = GetDepartmentForIssueType(model.MaintenanceCall?.IssueTypeEnum ?? string.Empty);

            // Fetch the available technicians based on department
            var technicians = _context.Technicians
                .Where(t => t.Department == departmentForIssue && t.Available)
                .ToList();

            if (technicians == null)
            {
                technicians = new List<Technician>();  // Ensure it's never null
            }

            model.Technicians = technicians.Select(t => new SelectListItem
            {
                Value = t.Id.ToString(),
                Text = t.TechnicianName
            }).ToList();

            return View(model);
        }



        // Helper method to map issue types to departments
        private string GetDepartmentForIssueType(string issueType)
        {
            return issueType switch
            {
                "Road" => "Roads Department",
                "Billing" => "Rates and Taxes Department",
                "Sewer" => "Sewer Department",
                _ => "General Maintenance Department",
            };
        }
    }
}
