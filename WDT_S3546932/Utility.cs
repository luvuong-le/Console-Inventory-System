using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WDT_S3546932
{
    class Utility : ICommands
    {
        public  string displayTitle(String title) { Console.WriteLine(title); Console.WriteLine("---------------------------");  return title; }

        public  string displayMessage(String message) { Console.WriteLine("\n" + message + "\n"); return message; }

        public string displayMessageOneLine(String message) { Console.Write(message); return message; }

        public  string displayError(String error) { Console.WriteLine("\n [ERROR] " + error + "\n"); return error; }

        public  string getCurrentDirectory() { string path = Directory.GetCurrentDirectory(); return path; }

        public string getJsonDataDirectory(string filename) { string path = Directory.GetCurrentDirectory() + "/JsonData/" + filename; return path; } 
    }
}


