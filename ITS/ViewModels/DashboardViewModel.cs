using KobiAsITS.Dtos;
using KobiAsITS.Models;
using System.Collections.Generic;

namespace KobiAsITS.ViewModels
{
    public class DashboardViewModel
    {
        public dynamic SendTheMostRequestByDepartment { get; set; }
        public dynamic WaitingRequests { get; set; }
        public dynamic ConfirmRequests { get; set; }
        public dynamic DeniedRequests { get; set; }
        public List<RequestReportModel> Requests { get; set; }
        public List<Request> RequestsDb { get; set; }
    }
}
