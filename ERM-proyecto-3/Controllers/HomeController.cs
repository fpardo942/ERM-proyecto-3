using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ERM_proyecto_3.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace ERM_proyecto_3.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<Employee> _employeeManager;
        private readonly SignInManager<Employee> _signInManager;
        public HomeController(ILogger<HomeController> logger, UserManager<Employee> employeeManager, SignInManager<Employee> signInManager)
        {
            _logger = logger;
            _employeeManager = employeeManager;
            _signInManager = signInManager;
        }

        public IActionResult Index()
        {
            bool isSigned = _signInManager.IsSignedIn(User);
            if (isSigned)
            {
                return View();
            }

            else 
            {
                return Redirect("Identity/Account/Login");
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Authorize(Roles = "manager")]
        public async Task<IActionResult> ModifyEmployees()
        {
            List<Employee> employees = await _employeeManager.Users.ToListAsync();
            return View(employees);
        }

        public IActionResult EmployeeData()
        {
            return View();
        }

        [Authorize(Roles = "manager")]
        public async Task<IActionResult> MakeManager(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Employee user = await _employeeManager.FindByIdAsync(id);
            if (id != null)
            {
                await _employeeManager.AddToRoleAsync(user, "manager"); //Dar rol

            }
            return RedirectToAction(nameof(ModifyEmployees));
        }

        [Authorize(Roles = "manager")]
        public async Task<IActionResult> RemoveManager(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Employee user = await _employeeManager.FindByIdAsync(id);
            if (id != null)
            {

                await _employeeManager.RemoveFromRoleAsync(user, "manager"); //Quitar rol

            }
            return RedirectToAction(nameof(ModifyEmployees));
        }

    }
}
