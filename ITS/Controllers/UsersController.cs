using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KobiAsITS.Data;
using KobiAsITS.Models;
using KobiAsITS.Enums;
using Microsoft.AspNetCore.Authorization;
using Core.Utilities.Security.Hashing;

namespace KobiAsITS.Controllers
{
    [Authorize()]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Users
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var users = _context.Users.Include(u => u.Department).Include(u => u.Role);
            return View(await users.ToListAsync());
        }

        // GET: Users/Details/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(string Uuid)
        {
            if (Uuid == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.Department)
                .Include(u => u.Role)
                .FirstOrDefaultAsync(m => m.Uuid == Uuid);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }


        // GET: Users/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {

            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "DepartmentName");
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Name");
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(User user, string password)
        {
            if (ModelState.IsValid)
            {
                byte[] passwordHash, passwordSalt;
                HashingHelper.CreatePasswordHash(password, out passwordHash, out passwordSalt);
                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
                user.Uuid = Guid.NewGuid().ToString();
                user.Status = ValueEnums.StatusOn;
                user.CreateDate = DateTime.Now;
                user.UpdateDate = DateTime.Now;
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "DepartmentName", user.DepartmentId);
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Name", user.RoleId);
            return View(user);
        }

        // GET: Users/Edit/5
        [Authorize()]
        public async Task<IActionResult> Edit(string Uuid)
        {
            if (Uuid == null)
            {
                return NotFound();
            }

            var user = await _context.Users.Where(x => x.Uuid == Uuid).FirstOrDefaultAsync();
            if (user == null)
            {
                return NotFound();
            }
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "DepartmentName", user.DepartmentId);
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Name", user.RoleId);
            return View(user);
        }

        // POST: Users/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize()]
        public async Task<IActionResult> Edit(int id, User user, string password)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (password != null)
                    {
                        byte[] passwordHash, passwordSalt;
                        HashingHelper.CreatePasswordHash(password, out passwordHash, out passwordSalt);

                        user.PasswordHash = passwordHash;
                        user.PasswordSalt = passwordSalt;
                    }
                    user.UpdateDate = DateTime.Now;
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                if (User.IsInRole("Admin"))
                {
                    return RedirectToAction("Index");
                }
                else
                {
                return RedirectToAction("Index","Home");
                }
            }
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "DepartmentName", user.DepartmentId);
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Name", user.RoleId);
            return View(user);
        }

        // GET: Users/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string? Uuid)
        {
            if (Uuid == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.Department)
                .Include(u => u.Role)
                .FirstOrDefaultAsync(m => m.Uuid == Uuid);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users.FindAsync(id);
            user.Status = ValueEnums.StatusOff;
            user.UpdateDate = DateTime.Now;
            _context.Update(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}