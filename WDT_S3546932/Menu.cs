using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*  --------------Polymorphic Implementation of Menu types ------------- */

namespace WDT_S3546932
{
    /* ---------- Base Class []----------- */
    abstract class Menu
    {
        public abstract void displayMenu();

        static JsonUtility jsonCommand = new JsonUtility();

        private static string string_usr_inp; private static int usrInp; private static string storeName;

        static Utility command = new Utility(); static bool StoreName = false; static bool storeNameSet = false;


        /* ---------- Displays The Main Menu  ------------------ */
        public class mainMenu : Menu
        {
            public override void displayMenu()
            {
                int usrInp;
                do
                {
                    command.displayTitle(" \n Welcome to Marvellous Magic");
                    command.displayMessage(" 1. Owner \n 2. Franchise Owner \n 3. Customer \n 4. Quit");
                    Console.ForegroundColor = ConsoleColor.White; Console.Write("\n Enter Option [1] - [4]: "); string string_usr_inp = Console.ReadLine(); command.colourReset();
                    Int32.TryParse(string_usr_inp, out usrInp);

                /* Begin Switch Statement */
                switch (usrInp)
                {
                    case 1: Menu.OwnerMenu Omenu = new Menu.OwnerMenu(); Omenu.displayMenu(); continue;
                    case 2: Menu.FranchiseOwner Fmenu = new Menu.FranchiseOwner(); Fmenu.displayMenu(); command.displayMessage("Franchise Owner"); continue;
                    case 3: Menu.CustomerMenu Cmenu = new Menu.CustomerMenu(); Cmenu.displayMenu(); command.displayMessage("Customer Menu"); continue;
                    case 4: Environment.Exit(0); break;
                    default: command.displayError("Must be in range! [1] - [4]"); continue;

                } 
            } while (usrInp != 4); }
        }

        /* ---------- Owner Class Derived from Base Class []----------- */
        public class OwnerMenu : Menu
        {
            Owner Owner = new Owner();
            private List<Stock> productList = JsonConvert.DeserializeObject<List<Stock>>(jsonCommand.JsonReader(command.getJsonDataDirectory("stockrequests", "/Stock/") + ".json"));

            public override void displayMenu()
            {
                command.displayTitle(" \n Welcome to Marvellous Magic (Owner)");
                do
                {
                    command.displayMessage(" 1. Display All Stock Requests \n 2. Display Stock Requests (True/False) \n 3. Display All Product Lines \n 4. Return to Main Menu \n 5. Exit");
                    Console.ForegroundColor = ConsoleColor.White;  Console.Write("\n Enter Option [1] - [4]: "); command.colourReset();
                    string_usr_inp = Console.ReadLine(); Int32.TryParse(string_usr_inp, out usrInp);

                    switch (usrInp)
                    {
                        case 1: command.displayTitle("Displaying All Stock Requests"); Owner.displayAllStockRequests(productList); continue;
                        case 2: command.displayTitle("Displaying Stock Requests (True/False)"); Owner.displayAllStockRequestBool(productList); continue;
                        case 3: command.displayTitle("Displaying All Product Lines"); Owner.displayAllProductLines(); continue;
                        case 4: Menu.mainMenu main = new Menu.mainMenu();  main.displayMenu(); continue;
                        case 5: Environment.Exit(0); continue;
                        default: command.displayError("Must be in Range of [1] - [5]!"); continue;
                    }
                } while (usrInp != 5); }
        }

        /* ---------- Franchise Owner Class Derived from Base Class []----------- */
        public class FranchiseOwner : Menu
        {
            Franchise franchiseOwner = new Franchise();

            public override void displayMenu()
            {
                do
                {
                    if (storeNameSet == false)
                    {
                        command.displayMessageOneLine("\nCurrent Stores are: "); command.printAllStoreNames(command.getStoreNames());
                        command.displayMessageOneLine("\n\nEnter Store Name: "); string storeNameOption = Console.ReadLine(); //Change to Store ID for submission //
                        if (command.checkStoreName(storeNameOption, command.getStoreNames()) == true)
                        {
                            StoreName = true;
                            storeName = storeNameOption.ToUpper();
                            storeNameSet = true;
                        }
                        else { command.displayError("No Such Store"); displayMenu(); }
                    }else if(storeNameSet == true && StoreName == true)
                    //Check if Store name equals Current Store Names //
                    while (StoreName == true)
                    {
                        command.displayTitle("\n Welcome to Marvellous Magic (Franchise Holder - Olinda) Store: " + storeName);
                        command.displayMessage(" 1. Display Inventory \n 2. Display Inventory (Threshold) \n 3. Add New Inventory Item \n 4. Return to Main Menu \n 5. Exit");
                        Console.ForegroundColor = ConsoleColor.White; Console.Write("\n Enter Option [1] - [4]: "); string_usr_inp = Console.ReadLine(); Int32.TryParse(string_usr_inp, out usrInp); command.colourReset();

                        switch (usrInp)
                        {
                            case 1: command.displayMessage("Displaying Inventory for: " + storeName); franchiseOwner.displayInventory(storeName); continue;
                            case 2: command.displayMessage("Displaying Inventory Threshold"); franchiseOwner.displayInventoryThres(storeName);  continue;
                            case 3: command.displayMessage("Adding New Inventory Item"); franchiseOwner.AddNewInventory(storeName); continue;
                            case 4: Menu.mainMenu main = new Menu.mainMenu(); storeNameSet = false; StoreName = false; main.displayMenu(); continue;
                            case 5: Environment.Exit(0); continue;
                            default: command.displayError("Must be in Range of [1] - [5]!"); continue;
                        }
                    }
                } while (usrInp != 5);
            } 
        }

        /* ---------- Customer Class Derived from Base Class []----------- */
        public class CustomerMenu : Menu
        {
            Customer customer = new Customer();

            public override void displayMenu()
            {
                do
                {
                    if (storeNameSet == false)
                    {
                        command.displayMessageOneLine("\nCurrent Stores are: "); command.printAllStoreNames(command.getStoreNames());
                        command.displayMessageOneLine("\n\nEnter Store Name: "); string storeNameOption = Console.ReadLine(); //Change to Store ID for submission //

                        if (command.checkStoreName(storeNameOption, command.getStoreNames()) == true)
                        {
                            StoreName = true;
                            storeName = storeNameOption.ToUpper();
                            storeNameSet = true;
                        }
                        else { command.displayError("No Such Store"); displayMenu(); }
                    }
                    else if (storeNameSet == true && StoreName == true)
                    {
                        //Check if Store name equals Current Store Names //
                        while (StoreName == true)
                        {
                            command.displayTitle(" \n Welcome to Marvellous Magic (Customer) Store:" + storeName);
                            command.displayMessage(" 1. Display Products \n 2. Display Workshops \n 3. Return to Main Menu \n 4. Exit");
                            Console.Write("\n Enter Option [1] - [4]: "); string_usr_inp = Console.ReadLine(); Int32.TryParse(string_usr_inp, out usrInp);

                            switch (usrInp)
                            {
                                case 1: command.displayMessage("Displaying Products"); customer.displayProduct(storeName); continue;
                                case 2: command.displayMessage("Displaying Workshops"); customer.displayWorkShop(storeName); continue;
                                case 3: Menu.mainMenu main = new Menu.mainMenu(); storeNameSet = false; StoreName = false; main.displayMenu(); continue;
                                case 4: Environment.Exit(0); continue;
                                default: command.displayError("Must be in Range of [1] - [4]!"); continue;
                            }
                        }
                    }
            } while (usrInp != 4); }
        }
    }
}
