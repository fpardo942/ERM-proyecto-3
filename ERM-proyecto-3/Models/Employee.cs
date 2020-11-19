using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ERM_proyecto_3.Models
{
    public class Employee : IdentityUser
    {
        public string EmployeeName { get; set; }
        public string EmployeeSurname { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int Phone { get; set; }
        public int TotalDaysOfhollidays { get; set; }
        public int RestantDaysOfhollidays { get; set; }
        public List<hollidays> hollidaysList { get; set; }
    }
}