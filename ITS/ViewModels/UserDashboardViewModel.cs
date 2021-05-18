using KobiAsITS.Models;
using System.Collections.Generic;

namespace KobiAsITS.ViewModels
{
    public class UserDashboardViewModel
    {
        public int WaitingRequests { get; set; }
        public int ConfirmRequests { get; set; }
        public int DeniedRequests { get; set; }
        public List<Process> Processes { get; set; }
    }
}
