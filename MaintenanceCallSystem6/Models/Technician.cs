namespace MaintenanceCallSystem6.Models
{
    public class Technician
    {
        public int Id { get; set; }
        public string TechnicianName { get; set; }
        public string Department { get; set; } // e.g., "Roads", "Rates and Taxes", "Sewer"
        public bool Available { get; set; }
    }
}
