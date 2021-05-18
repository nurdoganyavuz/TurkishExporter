using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KobiAsITS.Enums
{
    public class ValueEnums
    {
        public const string waitingStatus = "Beklemede";
        public const string confirmStatus = "Onaylandı";
        public const string deniedStatus = "Reddedildi";

        public const byte waitingByteStatus = 0;
        public const byte confirmByteStatus = 1;
        public const byte deniedByteStatus = 2;

        //----------------------------------------
        public const string LowPriority = "Low";
        public const string MediumPriority = "Medium";
        public const string HighPriority = "High";
        public const string UrgentPriority = "Urgent";

        public const byte LowPriorityByte = 0;
        public const byte MediumPriorityByte = 1;
        public const byte HighPriorityByte = 2;
        public const byte UrgentPriorityByte = 3;

        //----------------------------------------
        public const string Continue = "Devam ediyor";
        public const string Pause = "Durduruldu";
        public const string Completed = "Tamamlandı";

        public const byte ContinueByteStatus = 0;
        public const byte PauseByteStatus = 1;
        public const byte CompletedByteStatus = 2;

        //----------------------------------------
        public const bool StatusOn = true;
        public const bool StatusOff = false;
    }
}
