using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ERM_proyecto_3.Data;
using ERM_proyecto_3.Models;
using Microsoft.AspNetCore.Identity;
using System.Windows;
using Xceed.Wpf.Toolkit;
using Microsoft.AspNetCore.Authorization;

namespace ERM_proyecto_3.Controllers
{
    public class hollidaysController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Employee> _employeeManager;

        public hollidaysController(ApplicationDbContext context, UserManager<Employee> employeeManager)
        {
            _context = context;
            _employeeManager = employeeManager;
        }

        // GET: hollidays
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.hollidays.Include(h => h.Employee);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: hollidays/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hollidays = await _context.hollidays
                .Include(h => h.Employee)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (hollidays == null)
            {
                return NotFound();
            }

            return View(hollidays);
        }

        // GET: hollidays/Create
        public IActionResult Create()
        {
            ViewData["EmployeeId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: hollidays/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,StartDate,EndDate,Aproved,EmployeeId")] hollidays hollidays)
        {
            if (ModelState.IsValid)
            {
                int totalDays;
                Employee employee = await _employeeManager.GetUserAsync(User); //added
                hollidays.EmployeeId = employee.Id;
                totalDays = Convert.ToInt32((hollidays.EndDate - hollidays.StartDate).TotalDays);
                if (employee.RestantDaysOfhollidays > totalDays && hollidays.StartDate > DateTime.Now)
                {
                    if (hollidays.Aproved)
                    {
                        employee.RestantDaysOfhollidays -= totalDays;
                    }

                    _context.Add(hollidays);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    bool action = false;
                    return View("ErrorView");
                }
                
            }
            ViewData["EmployeeId"] = new SelectList(_context.Users, "Id", "Id", hollidays.EmployeeId);
            return View(hollidays);
        }

        // GET: hollidays/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hollidays = await _context.hollidays.FindAsync(id);
            if (hollidays == null)
            {
                return NotFound();
            }
            ViewData["EmployeeId"] = new SelectList(_context.Users, "Id", "Id", hollidays.EmployeeId);
            return View(hollidays);
        }

        // POST: hollidays/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,StartDate,EndDate,Aproved,EmployeeId")] hollidays hollidays)
        {
            if (id != hollidays.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(hollidays);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!hollidaysExists(hollidays.Id))
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
            ViewData["EmployeeId"] = new SelectList(_context.Users, "Id", "Id", hollidays.EmployeeId);
            return View(hollidays);
        }

        // GET: hollidays/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hollidays = await _context.hollidays
                .Include(h => h.Employee)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (hollidays == null)
            {
                return NotFound();
            }

            return View(hollidays);
        }

        // POST: hollidays/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hollidays = await _context.hollidays.FindAsync(id);
            _context.hollidays.Remove(hollidays);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool hollidaysExists(int id)
        {
            return _context.hollidays.Any(e => e.Id == id);
        }

        public async Task<IActionResult> ManageHolidays(int id)
        {
                var hollidays = await _context.hollidays.ToListAsync();
                return View(hollidays);       
        }

        [Authorize(Roles = "manager")]
        public async Task<IActionResult> DenyHolidays(int id)
        { 
            var holidays = _context.hollidays.Where(x => x.Id == id).FirstOrDefault();
            Employee employee = await _employeeManager.FindByIdAsync(holidays.EmployeeId);
            int totalDaysOfHolidays = Convert.ToInt32((holidays.EndDate - holidays.StartDate).TotalDays) + 1;


            employee.RestantDaysOfhollidays += totalDaysOfHolidays;
            _context.Update(employee);

            holidays.Aproved = false;
            _context.Update(holidays);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(ManageHolidays));
        }

        [Authorize(Roles = "manager")]
        public async Task<IActionResult> AproveHolidays(int id)
        {
            var holidays = _context.hollidays.Where(x => x.Id == id).FirstOrDefault();
            int totalDaysOfHolidays = Convert.ToInt32((holidays.EndDate - holidays.StartDate).TotalDays) + 1;

            Employee employee = await _employeeManager.FindByIdAsync(holidays.EmployeeId);
            employee.RestantDaysOfhollidays -= totalDaysOfHolidays;
            _context.Update(employee);

            holidays.Aproved = true;
            _context.Update(holidays);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(ManageHolidays));
        }
    }
}
