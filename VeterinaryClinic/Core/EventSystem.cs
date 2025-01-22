using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VeterinaryClinic.Core
{
    public class EventSystem
    {

        public static event EventHandler EventBlackBackground; // событие для затемнение главного экрана
        public static event EventHandler EventUpdateRecords; // событие для обновления записей
        public static event EventHandler EventUpdateEmployee; // событие для обновления записей

        public static void InvokeEventBlackBackground()
        {
            EventBlackBackground?.Invoke(null,null);
        }

        public static void InvokeUpdateRecords()
        {
            EventUpdateRecords?.Invoke(null, null);
        }
        public static void InvokeUpdateEmployee()
        {
            EventUpdateEmployee?.Invoke(null, null);
        }
    }
}
