using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WDT_S3546932
{
    class CLI : ICommands
    {
        public  void displayTitle(String title) { Console.WriteLine(title); Console.WriteLine("---------------------------"); }

        public  void displayMessage(String message) { Console.WriteLine("\n" + message + "\n"); }

        public  void displayError(String error) { Console.WriteLine("\n [ERROR] " + error + "\n"); }

        public  String JsonReader(String fileName)
        {
            StreamReader r = new StreamReader(fileName);

            string json = r.ReadToEnd();

            r.Close();

            return json;
        }
    }
}

