using MaintenanceCallSystem6.Models;

namespace MaintenanceCallSystem6.ViewModels
{
    public class TechnicianCompositeViewModel
    {
        public List<TechnicianViewModel> Technicians { get; set; }
        public List<MaintenanceCall> MaintenanceCalls { get; set; }
    }
}
