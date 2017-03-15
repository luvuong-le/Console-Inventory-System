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
        private UI UserIfce = new Misc(); 
        public abstract void displayMenu();

        private static string string_usr_inp; private static int usrInp;

        /* ---------- Displays The Main Menu  ------------------ */ 
        public class mainMenu : Menu
        {
            public override void displayMenu()
            {
                int usrInp;
                do
                {
                    UserIfce.displayTitle(" \n Welcome to Marvellous Magic");
                    UserIfce.displayMessage(" 1. Owner \n 2. Franchise Owner \n 3. Customer \n 4. Quit");
                    Console.Write("\n Enter Option [1] - [4]: "); string string_usr_inp = Console.ReadLine();
                    Int32.TryParse(string_usr_inp, out usrInp);

                /* Begin Switch Statement */
                switch (usrInp)
                {
                    case 1: Menu.OwnerMenu Omenu = new Menu.OwnerMenu(); Omenu.displayMenu(); continue;
                    case 2: Menu.FranchiseOwner Fmenu = new Menu.FranchiseOwner(); Fmenu.displayMenu(); UserIfce.displayMessage("Franchise Owner"); continue;
                    case 3: Menu.Customer Cmenu = new Menu.Customer(); Cmenu.displayMenu(); UserIfce.displayMessage("Customer Menu"); continue;
                    case 4: Environment.Exit(0); break;
                    default: UserIfce.displayError("Must be in range! [1] - [4]"); continue;

                } 
            } while (usrInp != 4); }
        }

        /* ---------- Owner Class Derived from Base Class []----------- */
        public class OwnerMenu : Menu
        {
            public override void displayMenu()
            {
                UserIfce.displayTitle(" \n Welcome to Marvellous Magic (Owner)");
                do
                {
                    UserIfce.displayMessage(" 1. Display All Stock Requests \n 2. Display Stock Requests (True/False) \n 3. Display All Product Lines \n 4. Return to Main Menu \n 5. Exit");
                    Console.Write("\n Enter Option [1] - [4]: ");
                    string_usr_inp = Console.ReadLine(); Int32.TryParse(string_usr_inp, out usrInp);

                    switch (usrInp)
                    {
                        case 1: UserIfce.displayMessage("Displaying All Stock Requests"); UserIfce.displayAllStockRequests(); continue;
                        case 2: UserIfce.displayMessage("Displaying Stock Requests (True/False)"); continue;
                        case 3: UserIfce.displayMessage("Displaying All Product Lines"); UserIfce.displayAllProductLines(); continue;
                        case 4: Menu.mainMenu main = new Menu.mainMenu();  main.displayMenu(); continue;
                        case 5: Environment.Exit(0); continue;
                        default: UserIfce.displayError("Must be in Range of [1] - [5]!"); continue;
                    }
                } while (usrInp != 5); }
        }

        /* ---------- Franchise Owner Class Derived from Base Class []----------- */
        public class FranchiseOwner : Menu
        {
            public override void displayMenu()
            {
                do
                {
                    UserIfce.displayMessage("Enter Store ID: "); string storeID = Console.ReadLine(); UserIfce.displayMessage("Store ID: " + storeID);
                    UserIfce.displayTitle("\n Welcome to Marvellous Magic (Franchise Holder - Olinda)");
                    UserIfce.displayMessage(" 1. Display Inventory \n 2. Display Inventory (Threshold) \n 3. Add New Inventory Item \n 4. Return to Main Menu \n 5. Exit");
                    Console.Write("\n Enter Option [1] - [4]: "); string_usr_inp = Console.ReadLine(); Int32.TryParse(string_usr_inp, out usrInp);

                    switch (usrInp)
                    {
                        case 1: UserIfce.displayMessage("Displaying All Stock Requests"); continue;
                        case 2: UserIfce.displayMessage("Displaying Stock Requests (True/False)"); continue;
                        case 3: UserIfce.displayMessage("Displaying All Product Lines"); continue;
                        case 4: Menu.mainMenu main = new Menu.mainMenu(); main.displayMenu(); continue;
                        case 5: Environment.Exit(0); continue;
                        default: UserIfce.displayError("Must be in Range of [1] - [5]!"); continue;
                    }
                } while (usrInp != 5);
            } 
        }

        /* ---------- Customer Class Derived from Base Class []----------- */
        public class Customer : Menu
        {
            public override void displayMenu()
            {
                do
                { 
                    UserIfce.displayTitle(" \n Welcome to Marvellous Magic (Customer)");
                    UserIfce.displayMessage(" 1. Display Products \n 2. Display Workshops \n 3. Return to Main Menu \n 4. Exit");
                    Console.Write("\n Enter Option [1] - [4]: "); string_usr_inp = Console.ReadLine(); Int32.TryParse(string_usr_inp, out usrInp);

                switch (usrInp)
                {
                    case 1: UserIfce.displayMessage("Displaying Products"); continue;
                    case 2: UserIfce.displayMessage("Displaying Workshops"); continue;
                    case 3: Menu.mainMenu main = new Menu.mainMenu(); main.displayMenu(); continue;
                    case 4: Environment.Exit(0); continue;
                    default: UserIfce.displayError("Must be in Range of [1] - [4]!"); continue;
                }
            } while (usrInp != 4); }
        }
    }
}
