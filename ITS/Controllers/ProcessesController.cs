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
using Microsoft.EntityFrameworkCore.Query;
using System.IO;

namespace KobiAsITS.Controllers
{
    [Authorize()]
    public class ProcessesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProcessesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Processes
        public async Task<IActionResult> Index()
        {
            IQueryable<Process> list = _context.Processes.Include(p => p.Request).Include(p => p.User).AsQueryable();
            if (!User.IsInRole("Admin"))   //kullanıcı sadece kendi birimine ait işleme alınan talepleri listelesin, yalnızca admin yetkisi olanlar tümünü görebilsin diye.
            {
                var departmantId = Convert.ToInt32(User?.FindFirst(p => p.Type == "DepartmentId")?.Value ?? "0");
                list = list.Where(r => r.Request.User.Department.Id == departmantId);
            }
            return View(await list.ToListAsync());
        }

        // GET: Processes/Details/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var process = await _context.Processes
                .Include(p => p.Request)
                .Include(p=> p.Request.User)
                .Include(p => p.Request.User.Department)
                .FirstOrDefaultAsync(m => m.Id == id);
            ViewData["RequestFiles"] = null;
            var requestFile = await _context.RequestFiles.Where(u => u.RequestId == process.RequestId).ToListAsync(); //talebe ait dosyaları listeleme
            ViewData["RequestFiles"] = requestFile;
            if (process == null)
            {
                return NotFound();
            }

            return View(process);
        }

        public FileResult Download(string FilePath) //ek dosyaları indirme
        {
            var FileVirtualPath = "~/storage/img/requests/" + FilePath;
            return File(FileVirtualPath, "application/force-download", Path.GetFileName(FileVirtualPath));
        }

        // GET: Processes/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var process = await _context.Processes.Where(p => p.Id == id).FirstOrDefaultAsync();         
            if (process == null)
            {
                return NotFound();
            }
            ViewData["RequestId"] = new SelectList(_context.Requests.ToList(), "Id", "RequestDescription" , "RequestProgressStatus");
           
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", process.UserId);
            return View(process);
        }
        
        // POST: Processes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, Process process, int RequestProgressStatus)
        {
            if (id != process.Id)
            {
                return NotFound();
            }
            var request = _context.Requests.Where(x => x.Id == process.RequestId).FirstOrDefault();

            if (ModelState.IsValid)
            {
                try
                {
                    request.RequestTitle = process.Request.RequestTitle;
                    request.RequestDescription = process.Request.RequestDescription;
                    request.RequestProgressStatus = RequestProgressStatus;
                    if (process.Status == ValueEnums.CompletedByteStatus) //statusu tamamlandı olarak seçildiğinde ilerleme durumu otomatik olarak %100 atanıyor, kesin bitiş tarihi sistem saati olarak default veriliyor.
                    {
                        process.ExpirationDate = DateTime.Now;
                        request.RequestProgressStatus = 100;
                    }
                    else if (RequestProgressStatus == 100) //ilerleme durumu %100 yapıldıgında statusu tamamlandı olarak atanıyor, kesin bitiş tarihi sistem saati olarak default veriliyor.
                    {
                        process.ExpirationDate = DateTime.Now;
                        process.Status = ValueEnums.CompletedByteStatus;
                    }                    
                    process.Request = request;
                    _context.Update(process);
                    await _context.SaveChangesAsync();

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProcessExists(process.Id))
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
            ViewData["RequestId"] = new SelectList(_context.Requests.ToList(), "Id", "RequestDescription", "RequestProgressStatus");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", process.UserId);
            return View(process);
        }

        // GET: Processes/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var process = await _context.Processes
                .Include(p => p.Request)
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (process == null)
            {
                return NotFound();
            }

            return View(process);
        }
        // POST: Processes/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id, int requestId)
        {
            var process = await _context.Processes.FindAsync(id);
            _context.Processes.Remove(process);            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        private bool ProcessExists(int id)
        {
            return _context.Processes.Any(e => e.Id == id);
        }
    }
}
