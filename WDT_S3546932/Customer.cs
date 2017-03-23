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

        int productListCount = 0;

        public override List<StoreStock> displayProduct(string store)
        {
            List<StoreStock> products = JsonConvert.DeserializeObject<List<StoreStock>>(jsonCommand.JsonReader(command.getJsonDataDirectory(store, "/Stores/") + "_inventory.json"));

            Console.WriteLine("{0,10} {1,25} {2,25}", "ID", "Product Name", "Current Stock");

            foreach (var product in products)
            {
                if (productListCount < 5)
                {
                    Console.WriteLine("{0,10} {1,25} {2,25}", product.ID, product.ProductName, product.CurrentStock);
                    productListCount++;
                }
            }

            customerOptions(store, products);

            return products;
        }

        public void customerOptions(string storeName, List<StoreStock> store)
        {
            command.displayMessage("[Legend: 'P' Next Page | 'R' Return to Menu  | 'B' Previous Page | 'C' Complete Transaction | ID Number Based on Item]");

            command.displayMessageOneLine("Enter Item ID Number to Purchase or Function: "); string user_inp = Console.ReadLine(); int item_ID; Int32.TryParse(user_inp, out item_ID);

            if (user_inp == "P" || user_inp == "p")
            {
                command.displayMessage("Going to Next Page");

                foreach (var product in store)
                {
                        if (product.ID > productListCount && productListCount <= store.Count)
                        {
                            Console.WriteLine("{0,10} {1,25} {2,25}", product.ID, product.ProductName, product.CurrentStock);
                            productListCount++;
                        }else if(product.ID < productListCount && productListCount >= store.Count) { command.displayError("End Of Items...Returning to first page"); productListCount = 0; displayProduct(storeName); break; }

                }
                customerOptions(storeName, store);
            }
            else if(user_inp == "B" || user_inp == "b")
            {
                command.displayMessage("Going to Previous Page");
                productListCount = 0;
                Console.WriteLine("{0,10} {1,25} {2,25}", "ID", "Product Name", "Current Stock");
                foreach (var product in store)
                {
                    if (productListCount < 5)
                    {
                        Console.WriteLine("{0,10} {1,25} {2,25}", product.ID, product.ProductName, product.CurrentStock);
                        productListCount++;
                    }
                }
                customerOptions(storeName, store);
            }
            else if (user_inp == "R" || user_inp == "r")
            {
                Menu.CustomerMenu Cmenu = new Menu.CustomerMenu(); Cmenu.displayMenu();
            }
            else if (user_inp == "C" || user_inp == "c")
            {
                command.displayMessage("Completing Transaction");
            }
            else if (jsonCommand.matchID(storeName, item_ID) == true) //Checks if the ID input was valid
            {
                int Quantity;
                command.displayMessageOneLine("Please Enter the Amount you would like to Purchase: "); string quant = Console.ReadLine();
                while (!Int32.TryParse(quant, out Quantity))
                {
                    command.displayError("Input must be a number!");
                    productListCount = 0;
                    return;
                }

                foreach (var product in store)
                {
                    if (product.ID == item_ID)
                    {
                        command.displayMessageOneLine("\nYouve chosen the Product");

                        Console.WriteLine("\n{0,0} {1,15} {2,15}", product.ID, product.ProductName, "Quantity: " + Quantity);

                        command.displayMessage("Would you like to Continue [Yes/No]"); string choice = Console.ReadLine();

                        if (choice == "yes" || choice == "Yes" || choice == "y")
                        {
                            //Process purchase product  //
                            purchaseProduct(product.ProductName, storeName, Quantity);
                            command.displayMessage("Keep Purchasing [Yes/No]"); string more = Console.ReadLine();
                            if (more == "yes" || more == "Yes" || more == "y")
                            {
                                productListCount = 0;
                                displayProduct(storeName);
                            }
                            else if (more == "no" || more == "No" || more == "n")
                            {
                                //Create Variable here such as purchaseComplete = true; //
                                command.displayMessage("Ok. Returning to Menu"); return;
                            }
                        }
                        else if (choice == "No" || choice == "no" || choice == "n")
                        {
                            //purchaseComplete = false //
                            command.displayMessage("Would you like to book into a workshop? [Yes/No]"); string workshop = Console.ReadLine();
                            //Compare workshops entered to purchasecOMPLETE to see if discount is added // //Workshopbooked = true/false //
                            if (workshop == "Yes") { command.displayMessage("WorkShop Times: "); break; } else { command.displayMessage("Ok. Returning to Menu"); }
                        }
                    }else
                    {
                        command.displayError("Not enough Stock");
                        break;
                    }
                }
            }
        }

        public override void displayWorkShop(string storeName)
        {
            List<Workshop> workshops = JsonConvert.DeserializeObject<List<Workshop>>(jsonCommand.JsonReader(command.getJsonDataDirectory(storeName, "/Workshops/") + "_workshop.json"));

            Console.WriteLine("{0,15} {1,15} {2,25} {3,25}", "Name", "Session", "Time", "Booking Reference");
            foreach (var booking in workshops)
            {
                Console.WriteLine("{0,15} {1,15} {2,25} {3,25}", booking.Name, booking.Session, booking.Time, booking.BookingRef);
            }
        }

        public override void purchaseProduct(String productName, String StoreName, int Quantity)
        {
            List<StoreStock> storeStock = JsonConvert.DeserializeObject<List<StoreStock>>(jsonCommand.JsonReader(command.getJsonDataDirectory(StoreName, "/Stores/") + "_inventory.json"));

            //Looping through the Stock Class//
            foreach (var product in storeStock)
            {
                if (productName == product.ProductName && product.CurrentStock >= Quantity)
                {
                    command.displayMessage("There is Stock Left");
                    jsonCommand.updateQuantityStore(command.getJsonDataDirectory(StoreName, "/Stores/") + "_inventory.json", productName, Quantity, "minus");
                    break;
                }
                else if (productName == product.ProductName && product.CurrentStock < Quantity)
                {
                    command.displayError("Sorry We have: " + product.CurrentStock + " In Stock at the moment");
                    return;
                }
                else if (productName == product.ProductName && product.CurrentStock == 0)
                {
                    command.displayError("We do not have any in stock at the moment");
                    return;
                }
            }
        }
    }
}
