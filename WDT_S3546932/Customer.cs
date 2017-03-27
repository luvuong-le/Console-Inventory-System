﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WDT_S3546932
{
    class Customer : CustomerCLI
    {
        Utility command = new Utility();

        JsonUtility jsonCommand = new JsonUtility();

        int productListCount; int purchaseNumber = 0; double purchaseTotal = 100.00; int bookedTotal = 0;

        bool purchaseComplete = false;

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
                    }
                    else if (product.ID < productListCount && productListCount >= store.Count) { command.displayError("End Of Items...Returning to first page"); productListCount = 0; displayProduct(storeName); break; }

                }
                customerOptions(storeName, store);
            }
            else if (user_inp == "B" || user_inp == "b")
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
                if (purchaseComplete == true && itemCart.Count != 0) { printReciept(itemCart, bookedTotal); }
                else if (purchaseComplete == false) { purchaseComplete = true; printReciept(itemCart, bookedTotal); }
            }
            else if (jsonCommand.matchID(storeName, item_ID) == true) //Checks if the ID input was valid
            {
                command.displayMessageOneLine("Please Enter the Amount you would like to Purchase: "); string quant = Console.ReadLine();
                int Quantity = command.convertInt(quant);
                if (command.checkInt(quant, Quantity) == true)
                {
                    if (purchaseComplete == false)
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
                                        command.displayMessage("Keep Purchasing [Yes/No]"); string more = Console.ReadLine();
                                        if (more == "yes" || more == "Yes" || more == "y")
                                        {
                                            productListCount = 0;
                                            displayProduct(storeName);
                                            continue;
                                        }
                                        else if (more == "no" || more == "No" || more == "n")
                                        {
                                            purchaseComplete = true;
                                            command.displayMessage("Would you like to book into a workshop? [Yes/No]"); string workshop = Console.ReadLine();
                                            //Compare workshops entered to purchasecOMPLETE to see if discount is added // //Workshopbooked = true/false //
                                            if (workshop == "Yes" || workshop == "yes") { displayWorkShop(storeName); bookWorkshop(workshopTimes, storeName); return; } else { command.displayMessage("Ok. Returning to Menu"); return; }
                                        }
                                    }
                                    else if (choice == "No" || choice == "no" || choice == "n")
                                    {
                                        purchaseComplete = false;
                                        command.displayMessage("Would you like to book into a workshop? [Yes/No]"); string workshop = Console.ReadLine();
                                        //Compare workshops entered to purchasecOMPLETE to see if discount is added // //Workshopbooked = true/false //
                                        if (workshop == "Yes" || workshop == "yes") { displayWorkShop(storeName); bookWorkshop(workshopTimes, storeName); return; } else { command.displayMessage("Ok. Returning to Menu"); return; }
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
                    else if (purchaseComplete == true) { command.displayError("Your Shopping Session has Finished, Please exit and Log back in to Start Fresh"); }
                }
            }
        }

        #endregion

        #region displayWorkshops
        public override void displayWorkShop(string storeName)
        {
            List<WorkshopTimes> workshopsTimes = JsonConvert.DeserializeObject<List<WorkshopTimes>>(jsonCommand.JsonReader(command.getJsonDataDirectory(storeName, "/Workshops/") + "_workshopTimes.json"));

            Console.WriteLine("{0,15} {1,25} {2,25} {3,15} {4,15} {5,25}", "ID", "Type", "Session Times", "Number of People Booked", "Places left", "Workshop Availability");
            foreach (var session in workshopsTimes)
            {
                Console.WriteLine("{0,15} {1,25} {2,25} {3,15} {4,15} {5,25}", session.ID, session.type, session.sessionTimes, session.numBooking, session.avabililty, session.full);
            }

            Console.WriteLine();

            List<Workshop> workshopBookings = JsonConvert.DeserializeObject<List<Workshop>>(jsonCommand.JsonReader(command.getJsonDataDirectory(storeName, "/Workshops/") + "_bookings.json"));

            Console.WriteLine("{0,15} {1,25} {2,25} {3,15}", "Name", "Session", "Time", "Booking Reference");

            foreach (var bookings in workshopBookings)
            {
                Console.WriteLine("{0,15} {1,25} {2,25} {3,15}", bookings.Name, bookings.Session, bookings.Time, bookings.BookingRef);
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

        public List<customerPurchase> printReciept(List<customerPurchase> products, int bookedTotal)
        {
            if (bookedTotal == 0)
            {
                command.displayTitle("Printed Reciept: ");
                command.displayMessage("You have Purchased " + itemCart.Count + " Items");

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
            else
            {
                command.displayTitle("Printed Reciept: Booked Into Workshop! Added 10% Discount on Purchase! ");
                command.displayMessage("You have Purchased " + itemCart.Count + " Items");

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
                        foreach (var item in purchasedProducts)
                        {
                            if (item.ProductName == productName)
                            {
                                item.Quantity += Quantity;
                            }
                        }
                    }
                    else
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

        public void bookWorkshop(List<WorkshopTimes> workshopTimes, string storeName)
        {
            Console.WriteLine("You have Booked into " + bookedTotal + " Workshops");
            command.displayMessageOneLine("\n\nEnter the Workshop ID you would like to book into: "); string book = Console.ReadLine();
            int workshopTime = command.convertInt(book);
            if (command.checkInt(book, workshopTime))
            {
                command.displayMessage("Youve chosen: " + workshopTime);
                foreach (var chosen in workshopTimes)
                {
                    if (chosen.ID == workshopTime)
                    {
                        Console.WriteLine("{0,15} {1,25} {2,25} {3,15} {4,15} {5,25}", chosen.ID, chosen.type, chosen.sessionTimes, chosen.numBooking, chosen.avabililty, chosen.full);
                    }

                    if (command.Continue("Confirm Booking?") == true)
                    {
                        command.displayMessageOneLine("Enter Name: "); string name = Console.ReadLine();
                        Console.WriteLine();
                        string bookingRef = command.generateBookingReference(7);
                        command.displayMessageOneLine(name + " Your Booking Reference is: " + bookingRef + "\n");
                        addBooking(workshopTimes, workshopTime, storeName, name, bookingRef, chosen.sessionTimes, chosen.type);
                        break;
                    }
                    else
                    {
                        command.displayError("Please pick a valid ID or Type 'Exit' to Exit");
                        displayWorkShop(storeName);
                        bookWorkshop(workshopTimes, storeName);
                        break;
                    }
                }
            }
        }

        public void addBooking(List<WorkshopTimes> workshopTimes, int ID, string storename, string name, string bookingRef, string time, string session)
        {
            command.displayMessage("Added Booking");

            //Creating a new Local List of type Stock//
            List<Workshop> workshop = new List<Workshop>();

            //Reading through the stockrequests.json file and with that jsonData, converting the json data into Object List<Stock> //
            using (var streamReader = new StreamReader(command.getJsonDataDirectory(storename, "/Workshops/") + "_bookings.json"))
            {
                var jsonData = streamReader.ReadToEnd(); workshop = JsonConvert.DeserializeObject<List<Workshop>>(jsonData);
            }

            foreach (var times in workshopTimes)
            {
                if (ID == times.ID)
                {
                    workshop.Add(new Workshop(name, session, time, bookingRef));
                    foreach (var booking in workshop)
                    {
                        Console.WriteLine("|Name: " + workshop.LastOrDefault().Name + " |Session: " + workshop.LastOrDefault().Session + " |Time: " +
                                            workshop.LastOrDefault().Time + " |Booking Reference: " + workshop.LastOrDefault().BookingRef); break;
                    }

                    updateBookingDetails(workshopTimes, ID, storename, name, bookingRef, time, session);
                }
            }

            var addBooking = JsonConvert.SerializeObject(workshop, Formatting.Indented);
            File.WriteAllText(command.getJsonDataDirectory(storename, "/Workshops/") + "_bookings.json", addBooking);
            bookedTotal++;
        }

        public List<WorkshopTimes> updateBookingDetails(List<WorkshopTimes> workshopTimes, int ID, string storename, string name, string bookingRef, string time, string session)
        {
            command.displayMessage("Updating Booking..");
            foreach (var times in workshopTimes)
            {
                if (ID == times.ID)
                {
                    command.displayMessage("BOOKING FOUND");
                    if (session == times.type && time == times.sessionTimes)
                    {
                        command.displayMessage("Updating....");
                        Thread.Sleep(2000);
                        times.numBooking = times.numBooking + 1; times.avabililty = times.avabililty - 1;
                        command.displayMessage("Nmber Of People Booked In: " + times.numBooking); command.displayMessage("Nmber Of Places Left: " + times.avabililty);
                        command.displayMessage("Update Complete");
                        if(times.avabililty == 0) { times.full = true; }
                        break;
                    }
                    else
                    {
                        command.displayMessage("Cannot Update.");
                    }
                }
            }

            var updatedBookingDetails = JsonConvert.SerializeObject(workshopTimes, Formatting.Indented);
            File.WriteAllText(command.getJsonDataDirectory(storename, "/Workshops/") + "_workshopTimes.json", updatedBookingDetails);
            return workshopTimes;
        }
    }
}
