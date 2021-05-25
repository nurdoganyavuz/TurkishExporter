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
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using KobiAsITS.Constants;
using Microsoft.AspNetCore.Authorization;

namespace KobiAsITS.Controllers
{
    [Authorize()]
    public class RequestsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RequestsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Requests
        public async Task<IActionResult> Index()
        {
            var List = _context.Requests
                .Include(r => r.User.Department)
                .Include(r => r.User)
                .Where(r => r.Status == ValueEnums.waitingByteStatus || r.Status == ValueEnums.deniedByteStatus);

            if (!User.IsInRole("Admin")) //kullanıcı sadece kendi birimine ait talepleri listelesin, yalnızca admin yetkisi olanlar tüm talepleri görebilsin diye.
            {
                var departmentId = Convert.ToInt32(User?.FindFirst(p => p.Type == "DepartmentId")?.Value ?? "0");
                List = List.Where(r => r.User.Department.Id == departmentId);
            }
            return View(await List.ToListAsync());
        }

        
        // GET: Requests/Details/5
        public async Task<IActionResult> Details(string Uuid)
        {
            if (Uuid == null)
            {
                return NotFound();
            }

            var request = await _context.Requests.Include(r => r.User.Department)
                .FirstOrDefaultAsync(m => m.Uuid == Uuid);
            ViewData["RequestFiles"] = null;

            var requestFile = await _context.RequestFiles.Where(u => u.RequestId == request.Id).ToListAsync();
            ViewData["RequestFiles"] = requestFile;
            if (request == null)
            {
                return NotFound();
            }
            ViewBag.UserId = new SelectList(_context.Users.ToList(), "Id", "UserFirstName");

            return View(request);
        }

        // GET: Requests/Create
        public IActionResult Create()
        {
            ViewBag.UserId = new SelectList(_context.Users.ToList(), "Id", "UserFirstName");
            return View();
        }


        // GET: Requests/Create
        public FileResult Download(string FilePath)
        {
            var FileVirtualPath = "~/storage/img/requests/" + FilePath;
            return File(FileVirtualPath, "application/force-download", Path.GetFileName(FileVirtualPath));
        }

        // POST: Requests/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Request request, List<IFormFile> files)
        {
            if (ModelState.IsValid)
            {
                if (!VerifyFileExtension(files))
                {
                    TempData["message"] = Messages.InvalidFileExtension;
                    TempData.Keep();
                    return RedirectToAction("Create", "Requests");
                }
                var WhoIsUser = User?.FindFirst(p => p.Type == "Uuid")?.Value;
                var WhoUser = _context.Users.Where(x => x.Uuid == WhoIsUser).FirstOrDefault(); //Talep ekleme kısmında giriş yapan kimse "talep gönderen kişiye" o atansın, giriş yapan kullanıcı tekrar isim girmesin diye.
                request.UserId = WhoUser.Id;
                request.Uuid = Guid.NewGuid().ToString();
                request.Status = ValueEnums.waitingByteStatus;
                request.CreateDate = DateTime.Now;
                request.UpdateDate = DateTime.Now;
                _context.Add(request);
                await _context.SaveChangesAsync();

                if (files.Count > 0)
                {
                    foreach (IFormFile file in files)
                    {
                        var fileSave = new RequestFile { RequestId = request.Id };

                        string fileName = Path.GetFileNameWithoutExtension(file.FileName);
                        string extension = Path.GetExtension(file.FileName);
                        fileName = fileName + "_" + request.Id + extension;// filename = ahmet + " " + "5" + ".png" 
                        fileSave.FilePath = fileName;
                        var directory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "storage", "img", "requests");
                        if (!Directory.Exists(directory))
                            Directory.CreateDirectory(directory);
                        var upload = Path.Combine(directory, fileName);
                        using (var fileStream = new FileStream(upload, FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);
                        }
                        fileSave.CreateDate = DateTime.Now;
                        fileSave.Uuid = Guid.NewGuid().ToString();
                        _context.Add(fileSave);
                        await _context.SaveChangesAsync();
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.UserId = new SelectList(_context.Users.ToList(), "Id", "UserFirstName");
            return View(request);
        }

        // GET: Requests/Edit/5
        public async Task<IActionResult> Edit(string Uuid)
        {
            if (Uuid == null)
            {
                return NotFound();
            }
            var request = await _context.Requests.Where(r => r.Uuid == Uuid).FirstOrDefaultAsync();
            ViewData["RequestFiles"] = null;

            var requestFile = await _context.RequestFiles.Where(u => u.RequestId == request.Id).ToListAsync();
            ViewData["RequestFiles"] = requestFile;
            if (request == null)
            {
                return NotFound();
            }
            ViewBag.UserId = new SelectList(_context.Users.ToList(), "Id", "UserFirstName");
            return View(request);
        }

        // POST: Requests/Edit/5        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Request request, string refuseMessage)
        {
            if (id != request.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    request.UpdateDate = DateTime.Now;
                    _context.Update(request);
                    if (request.Status == ValueEnums.confirmByteStatus)
                    {
                        var WhoIsUser = User?.FindFirst(p => p.Type == "Uuid")?.Value;
                        var WhoUser = _context.Users.Where(x => x.Uuid == WhoIsUser).FirstOrDefault(); //talebi onaylayan kişiyi çekebilmek için yazıldı.
                        var process = new Process { RequestId = request.Id };
                        process.VerificationDate = DateTime.Now;
                        process.UserId = WhoUser.Id;
                        process.Status = ValueEnums.ContinueByteStatus;
                        _context.Processes.Add(process);
                        _context.SaveChanges();

                    }
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RequestExists(request.Id))
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
            return View(request);
        }
        // GET: Requests/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string Uuid)
        {
            if (Uuid == null)
            {
                return NotFound();
            }

            var request = await _context.Requests
                .FirstOrDefaultAsync(r => r.Uuid == Uuid);
            if (request == null)
            {
                return NotFound();
            }
            ViewBag.UserId = new SelectList(_context.Users.ToList(), "Id", "UserFirstName");
            return View(request);
        }
        // POST: Requests/Delete/5
      
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var request = _context.Requests.Include(r => r.RequestFiles).FirstOrDefault(r => r.Id == id);

            foreach (var item in request.RequestFiles)
            {
                var directory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "storage", "img", "requests");
                var fullFilePath = Path.Combine(directory, item.FilePath);
                if (System.IO.File.Exists(fullFilePath)) // Burada dosya var mı diye kontrol ediyoruz
                    System.IO.File.Delete(fullFilePath); // Eğer dosya varsa siliyoruz
            }

            _context.Requests.Remove(request);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        private bool VerifyFileExtension(List<IFormFile> files)
        {
            if (files.Count > 0)
            {
                foreach (IFormFile file in files)
                {
                    string extension = Path.GetExtension(file.FileName);
                    bool isValidFileExtension = Messages.ValidFileTypes.Any(t => t == Path.GetExtension(files[0].FileName).ToUpper());
                    if (!isValidFileExtension)
                    {

                        return false;
                    }
                }
            }
            return true;
        }
        private bool RequestExists(int id)
        {
            return _context.Requests.Any(r => r.Id == id);
        }
    }
}