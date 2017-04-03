using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WDT_S3546932
{
    class Program
    {
        static void Main(string[] args)
        {
            /* ---------- Driver Displaying Main Menu ------------- */
            Menu.mainMenu Main = new Menu.mainMenu();
            Main.displayMenu();
        }
    }
}
