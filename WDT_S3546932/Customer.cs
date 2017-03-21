using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WDT_S3546932
{
    class Customer : CustomerCLI
    {
        Utility command = new Utility();

        JsonUtility jsonCommand = new JsonUtility();

        public override List<StoreStock> displayProduct(string store)
        {
            List<StoreStock> products = JsonConvert.DeserializeObject<List<StoreStock>>(jsonCommand.JsonReader(command.getJsonDataDirectory(store, "/Stores/") + "_inventory.json"));

            Console.WriteLine("{0,5} {1,15} {2,15}", "ID", "Product Name", "Current Stock");

            foreach (var product in products)
            {
                Console.WriteLine("{0,5} {1,15} {2,15}", product.ID, product.ProductName, product.CurrentStock);
            }

            command.displayMessage("[Legend: 'P' Next Page | 'R' Return to Menu | 'C' Complete Transaction | ID Number Based on Item]");

            command.displayMessageOneLine("Enter Item ID Number to Purchase or Function: "); string user_inp = Console.ReadLine(); int item_ID; Int32.TryParse(user_inp, out item_ID);

            if(user_inp == "P" || user_inp == "p")
            {
                command.displayMessage("Going to Next Page");
            }else if(user_inp == "R" || user_inp == "r")
            {
                Menu.CustomerMenu Cmenu = new Menu.CustomerMenu(); Cmenu.displayMenu();
            }
            else if(user_inp == "C" || user_inp == "c")
            {
                command.displayMessage("Completing Transaction");
            }else if (jsonCommand.matchID(store, item_ID) == true) //Checks if the ID input was valid
            {
                //Prompt them for Quantity Number
                command.displayMessageOneLine("Please Enter the Amount you would like to Purchase Also: "); string quant = Console.ReadLine(); int Quantity; Int32.TryParse(quant, out Quantity);
            }
            return products;
        }

        public override void displayWorkShop()
        {
            throw new NotImplementedException();
        }
    }
}
