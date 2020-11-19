using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ERM_proyecto_3.Models
{
    public class hollidays
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool Aproved { get; set; }
        public string EmployeeId { get; set; }
        public Employee Employee { get; set; }
    }
}
