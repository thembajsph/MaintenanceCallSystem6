namespace MaintenanceCallSystem6.ViewModels;
using MaintenanceCallSystem6.Models;


    using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

public class TechnicianAssignViewModel
{
    public MaintenanceCall MaintenanceCall { get; set; }
    public int TechnicianId { get; set; }
    public IEnumerable<SelectListItem> Technicians { get; set; } // This should be populated with technicians

    public List<MaintenanceCall> MaintenanceCalls { get; set; }

    




}

