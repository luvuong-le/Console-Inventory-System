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

        public string getJsonDataDirectory(string filename, string folder) { string path = Directory.GetCurrentDirectory() + folder + filename; return path; } 

        public void printAllStoreNames(String[] filenames)
        {
            foreach (string store in filenames)
            {
                Console.Write("[" + Path.GetFileNameWithoutExtension(store).Split('_')[0] + "]" + " ");
            }
        }

        public String[] getStoreNames()
        { 
            String[] filenames = Directory.GetFiles(getCurrentDirectory() + "/Stores");
            
            return filenames;
        }

        public bool checkStoreName(string storeName, String[] filenames)
        {
            string nameOfStore = "";

            for (int i = 0; i < filenames.Length; i++)
            {
                nameOfStore = Path.GetFileNameWithoutExtension(filenames[i]).Split('_')[0];
                if (storeName == nameOfStore) { return true; }

                else if (storeName != nameOfStore) { continue; }
            }
            return false;
        }

        public bool checkInt(string request, int Quantity)
        {
            int returnedQuant;

            if (!Int32.TryParse(request, out returnedQuant))
            {
                displayError("Input must be a number!");
                return false;
            }
            return true;
        }

        public int convertInt(string StringToConvert)
        {
            int convertedInt;

            Int32.TryParse(StringToConvert, out convertedInt);

            return convertedInt;

        }
    }
}


