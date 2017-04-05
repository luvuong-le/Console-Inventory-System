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
        Random random = new Random();

        public  string displayTitle(String title) { Console.ForegroundColor = ConsoleColor.White;  Console.WriteLine("\n" + title); Console.WriteLine("-----------------------------------------------"); colourReset(); return title; }

        public  string displayMessage(String message) { Console.WriteLine("\n" + message + "\n"); return message; }

        public string displayMessageOneLine(String message) { Console.Write("\n" + message); return message; }

        public  string displayError(String error) { Console.ForegroundColor = ConsoleColor.Red;  Console.WriteLine("\n [ERROR] " + error + "\n"); colourReset(); return error; }

        public  string getCurrentDirectory() { string path = Directory.GetCurrentDirectory(); return path; }

        public string getJsonDataDirectory(string filename, string folder) { string path = Directory.GetCurrentDirectory() + folder + filename; return path; } 

        public void printAllStoreNames(String[] filenames)
        {
            foreach (string store in filenames)
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Write("[" + Path.GetFileNameWithoutExtension(store).Split('_')[0] + "]" + " "); colourReset();
            }
        }

        public String[] getFileNames(string folderName)
        { 
            String[] filenames = Directory.GetFiles(getCurrentDirectory() + "/" + folderName);
            
            return filenames;
        }

        public bool checkStoreName(string storeName, String[] filenames)
        {

            string nameOfStore = "";
            for (int i = 0; i < filenames.Length; i++)
            {
                nameOfStore = Path.GetFileNameWithoutExtension(filenames[i]).Split('_')[0];
                if (nameOfStore.Trim().Equals(storeName.Trim(), StringComparison.OrdinalIgnoreCase)){ return true; }

                else if (!nameOfStore.Trim().Equals(storeName.Trim(), StringComparison.OrdinalIgnoreCase)) { continue; }
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

        public string generateBookingReference(int size)
        {
            string numbers = "0123456789";
            var chars = Enumerable.Range(0, size).
                Select(x => numbers[random.Next(0, numbers.Length)]);

            return new string(chars.ToArray());
        }

        public bool Continue(string message)
        {
            displayMessageOneLine(message + "[Yes/No]: ");
            string yesorno = Console.ReadLine();

            if (yesorno.Trim().Equals("Yes".Trim(), StringComparison.OrdinalIgnoreCase)){
                return true;
            } else if (yesorno.Trim().Equals("No".Trim(), StringComparison.OrdinalIgnoreCase)){
                return false;
            }   return false;
        }

        public void colourChange() { Console.ForegroundColor = ConsoleColor.Green; }

        public void colourReset() { Console.ResetColor(); }

    }
}


