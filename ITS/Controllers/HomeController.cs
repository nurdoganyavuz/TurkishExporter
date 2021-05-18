using Core.Extensions;
using KobiAsITS.Data;
using KobiAsITS.Dtos;
using KobiAsITS.Enums;
using KobiAsITS.Models;
using KobiAsITS.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace KobiAsITS.Controllers
{
    [Authorize()]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        private int[] convertHelper(List<RequestReportModel> data, byte status)
        {
            var datas = new int[12];
            foreach (var item in data.Where(p => p.Status == status))
                datas[item.Month - 1] = item.Count;
            return datas;
        }
        public IActionResult UserDashboard()
        {
            var model = new UserDashboardViewModel();
            var userId = Convert.ToInt32(User?.FindFirst(p => p.Type == "UserId")?.Value ?? "0");
            model.WaitingRequests = _context.Requests.Count(p => p.UserId == userId && p.Status == ValueEnums.waitingByteStatus);
            model.ConfirmRequests = _context.Requests.Count(p => p.UserId == userId && p.Status == ValueEnums.confirmByteStatus);
            model.DeniedRequests = _context.Requests.Count(p => p.UserId == userId && p.Status == ValueEnums.deniedByteStatus);

            model.Processes = _context.Processes.Include(p => p.Request).Include(p => p.User).Where(p => p.Request.UserId == userId).ToList();

            return View(model);
        }

        public IActionResult Index([FromQuery] int? year)
        {
            if (!User.IsInRole("Admin"))
                return RedirectToAction("UserDashboard");
            year = year ?? DateTime.Now.Year;
            ViewBag.Year = year;
            var model = new DashboardViewModel()
            {
                SendTheMostRequestByDepartment = (from R in _context.Requests
                                                  join U in _context.Users on R.UserId equals U.Id
                                                  join D in _context.Departments on U.DepartmentId equals D.Id
                                                  group D by D.DepartmentName into groupDepartments
                                                  select new
                                                  {
                                                      label = $"{groupDepartments.Key} ({groupDepartments.Count()})",
                                                      data = groupDepartments.Count()
                                                  }).Take(10).ToList().ToJson(),
                Requests = (from R in _context.Requests
                            where R.CreateDate.Year == year.Value
                            group R by new { R.CreateDate.Month, R.Status } into groupRequests
                            select new RequestReportModel
                            {
                                Month = groupRequests.Key.Month,
                                Status = groupRequests.Key.Status,
                                Count = groupRequests.Count()
                            }).ToList()
            };
            model.WaitingRequests = convertHelper(model.Requests, ValueEnums.waitingByteStatus).ToJson();
            model.ConfirmRequests = convertHelper(model.Requests, ValueEnums.confirmByteStatus).ToJson();
            model.DeniedRequests = convertHelper(model.Requests, ValueEnums.deniedByteStatus).ToJson();
            return View(model);
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
    }
}
