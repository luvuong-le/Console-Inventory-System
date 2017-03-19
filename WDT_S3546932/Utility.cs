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
    class Utility : ICommands
    {
        public  string displayTitle(String title) { Console.WriteLine(title); Console.WriteLine("---------------------------");  return title; }

        public  string displayMessage(String message) { Console.WriteLine("\n" + message + "\n"); return message; }

        public  string displayError(String error) { Console.WriteLine("\n [ERROR] " + error + "\n"); return error; }

        public  string JsonReader(string fileName)
        {
            String json = " ";

            try
            {
                StreamReader r = new StreamReader(fileName);

                json = r.ReadToEnd();

                r.Close();

            }catch(FileNotFoundException e){
                displayMessage(e.Message);
                Menu.OwnerMenu Omenu = new Menu.OwnerMenu(); Omenu.displayMenu();
            }
            return json;
        }
    }
}

