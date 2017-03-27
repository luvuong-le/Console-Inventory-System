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

        int productListCount; int purchaseNumber = 0; double purchaseTotal = 100.00;

        bool purchaseComplete = false; bool bookedWorkShop = false;

        List<customerPurchase> itemCart = new List<customerPurchase>();

        #region displayProduct
        public override List<StoreStock> displayProduct(string store)
        {
            productListCount = 0;
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
        #endregion

        #region CustomerOptions
        public void customerOptions(string storeName, List<StoreStock> store)
        {
            List<WorkshopTimes> workshopTimes = JsonConvert.DeserializeObject<List<WorkshopTimes>>(jsonCommand.JsonReader(command.getJsonDataDirectory(storeName, "/Workshops/") + "_workshopTimes.json"));

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
                if (purchaseComplete == true) { printReciept(itemCart);  }
                else if (purchaseComplete == false) { purchaseComplete = true;  printReciept(itemCart); }
            }
            else if (jsonCommand.matchID(storeName, item_ID) == true) //Checks if the ID input was valid
            { 
                command.displayMessageOneLine("Please Enter the Amount you would like to Purchase: "); string quant = Console.ReadLine();
                int Quantity = command.convertInt(quant);
                if (command.checkInt(quant, Quantity) == true)
                {
                    while (purchaseComplete == false)
                    {
                        foreach (var product in store)
                        {
                            if (product.ID == item_ID)
                            {
                                if (product.CurrentStock >= Quantity)
                                {

                                    command.displayMessageOneLine("\nYouve chosen the Product");

                                    Console.WriteLine("\n{0,0} {1,15} {2,15}", product.ID, product.ProductName, "Quantity: " + Quantity);

                                    command.displayMessage("Would you like to Continue [Yes/No]"); string choice = Console.ReadLine();

                                    if (choice == "yes" || choice == "Yes" || choice == "y")
                                    {
                                        //Process purchase product  //
                                        addProduct(itemCart, store, product.ProductName, product.Store, Quantity);
                                        purchaseProduct(product.ProductName, storeName, Quantity);
                                        purchaseTotal++;
                                        command.displayMessage("Keep Purchasing [Yes/No]"); string more = Console.ReadLine();
                                        if (more == "yes" || more == "Yes" || more == "y")
                                        {
                                            productListCount = 0;
                                            displayProduct(storeName);
                                            continue;
                                        }
                                        else if (more == "no" || more == "No" || more == "n")
                                        {
                                            //Create Variable here such as purchaseComplete = true; //
                                            purchaseComplete = true;
                                            command.displayMessage("Would you like to book into a workshop? [Yes/No]"); string workshop = Console.ReadLine();
                                            //Compare workshops entered to purchasecOMPLETE to see if discount is added // //Workshopbooked = true/false //
                                            if (workshop == "Yes" || workshop == "yes") { displayWorkShop(storeName); bookWorkshop(workshopTimes); bookedWorkShop = true; return; } else { command.displayMessage("Ok. Returning to Menu"); bookedWorkShop = false; return; }
                                        }
                                    }
                                    else if (choice == "No" || choice == "no" || choice == "n")
                                    {
                                        purchaseComplete = false;
                                        command.displayMessage("Would you like to book into a workshop? [Yes/No]"); string workshop = Console.ReadLine();
                                        //Compare workshops entered to purchasecOMPLETE to see if discount is added // //Workshopbooked = true/false //
                                        if (workshop == "Yes" || workshop == "yes") { displayWorkShop(storeName); bookWorkshop(workshopTimes); bookedWorkShop = true; return; } else { command.displayMessage("Ok. Returning to Menu"); bookedWorkShop = false; return; }
                                    }
                                }
                                else if (product.CurrentStock < Quantity) { command.displayError("Not enough stock"); displayProduct(storeName); return; }
                                else if (product.CurrentStock == 0) { command.displayError("0 in Stock "); displayProduct(storeName); return; }
                            }
                            else
                            {
                                continue;
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region displayWorkshops
        public override void displayWorkShop(string storeName)
        {
            List<WorkshopTimes> workshops = JsonConvert.DeserializeObject<List<WorkshopTimes>>(jsonCommand.JsonReader(command.getJsonDataDirectory(storeName, "/Workshops/") + "_workshopTimes.json"));

            Console.WriteLine("{0,15} {1,25} {2,25} {3,15} {4,15}", "Type", "Session Times", "Number of People Booked", "Availability", "Full");
            foreach (var session in workshops)
            {
                Console.WriteLine("{0,15} {1,25} {2,25} {3,15} {4,15}", session.type, session.sessionTimes, session.numBooking, session.avabililty, session.full);
            }
        }
        #endregion

        #region purchaseProduct
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
        #endregion

        public List<customerPurchase> printReciept(List<customerPurchase> products)
        {
            command.displayTitle("Printed Reciept: ");
            command.displayMessage("Items: ");

            for (int i = 0; i < products.Count; i++)
            {
                Console.WriteLine(" Item Number: " + products[i].purchaseItemNumber + 
                    " Product: " + products[i].ProductName + 
                    " Quantity: " + products[i].Quantity + 
                    " Store Purchased: " + products[i].Store);
                Console.WriteLine();
            }
            command.displayMessage("Total Cost:" + purchaseTotal);
            return products;
        }

        public void addProduct(List<customerPurchase> purchasedProducts, List<StoreStock> store, String productName, String StoreName, int Quantity)
        {
            //Looping through the Stock Class//
            foreach (var product in store)
            {
                if (productName == product.ProductName && product.CurrentStock >= Quantity)
                {
                    if (purchasedProducts.Any(purchase => purchase.ProductName == productName))
                    {
                            //Adding a new Object using the already available stock object //
                        foreach(var item in purchasedProducts)
                        {
                            if (item.ProductName == productName)
                            {
                                item.Quantity += Quantity;
                            }
                        }
                    }else
                    {
                        //Adding a new Object using the already available stock object //
                        purchasedProducts.Add(new customerPurchase(purchaseNumber++, StoreName, productName, Quantity));
                    }
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

        public void bookWorkshop(List<WorkshopTimes> workshopTimes)
        {
            command.displayMessageOneLine("Enter the Workshop ID you would like to book into: "); string book = Console.ReadLine();
            int workshopTime = command.convertInt(book);
            if (command.checkInt(book, workshopTime))
            {
                command.displayMessage("Youve chosen: " + workshopTime);
            }

            command.displayMessageOneLine("Enter Name: "); string name = Console.ReadLine();
            Console.WriteLine();
            string bookingRef = command.generateBookingReference(7);
            command.displayMessageOneLine(name + " Your Booking Reference is: " + bookingRef + "\n");
            //addBooking(name, bookingRef);
        }

        public void addBooking(string name, string bookingRef, string time, string session)
        {

        }
    }
}
