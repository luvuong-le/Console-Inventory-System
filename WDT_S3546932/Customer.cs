﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace WDT_S3546932
{
    class Customer : CustomerCLI
    {
        Utility command = new Utility();

        JsonUtility jsonCommand = new JsonUtility();

        static int productListCount; int purchaseNumber = 0; double purchaseTotal = 0; int bookedTotal = 0; string bookingRef = null;

        bool purchaseComplete = false; bool bookRef = false;

        List<customerPurchase> itemCart = new List<customerPurchase>();

        #region displayProduct
        public List<StoreStock> displayProduct(string store, List<StoreStock> storeStock)
        {
            productListCount = 0;

            Console.ForegroundColor = ConsoleColor.White;  Console.WriteLine("{0,10} {1,25} {2,25} {3,15}", "ID", "Product Name", "Current Stock", "Cost"); command.colourReset();
            foreach (var product in storeStock)
            {
                if (productListCount < 5 && product.CurrentStock == 0)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("{0,10} {1,25} {2,25} {3,15}", product.ID, product.ProductName, product.CurrentStock, "$" + product.Cost.ToString("N2")); 
                    productListCount++;
                    command.colourReset();
                }else if(productListCount < 5 && product.CurrentStock > 0)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("{0,10} {1,25} {2,25} {3,15}", product.ID, product.ProductName, product.CurrentStock, "$" + product.Cost.ToString("N2"));
                    productListCount++;
                    command.colourReset();
                }
            }

            customerOptions(store, storeStock);

            return storeStock;
        }
        #endregion

        /*
         * Customer Options: Displays the Options the Customer has once the products has been loaded 
         * "P": Next Page
         * "W": Book a Workshop
         * "S": Search Product
         * "R": Return to Menu
         * "C": Complete Transaction 
         * ID: Choose a number and the product is chosen for purchase
         */
        #region CustomerOptions
        public void customerOptions(string storeName, List<StoreStock> storeStock)
        {
            List<WorkshopTimes> workshopTimes = jsonCommand.getWorkShopTimes(storeName);
            command.colourChange();  command.displayMessage("Green: [Stock Available]"); command.colourReset(); Console.ForegroundColor = ConsoleColor.Red; command.displayMessage("Red: [Out of Stock]"); command.colourReset();
            Console.ForegroundColor = ConsoleColor.White;  command.displayMessage("[Legend: 'P' Next Page | 'R' Return to Menu  | 'B' Previous Page | 'C' Complete Transaction | 'W' Book Workshop | ID Number Based on Item | 'S' Search by Name]"); command.colourReset();

            command.displayMessageOneLine("Enter Item ID Number to Purchase or Function: "); string user_inp = Console.ReadLine(); int item_ID; Int32.TryParse(user_inp, out item_ID);

            if (user_inp.Equals("P".Trim(), StringComparison.OrdinalIgnoreCase))
            {
                Console.ForegroundColor = ConsoleColor.White;  command.displayMessage("Going to Next Page"); command.colourReset();

                foreach (var product in storeStock)
                {
                    if (product.ID > productListCount && productListCount <= storeStock.Count && product.CurrentStock == 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("{0,10} {1,25} {2,25} {3,15}", product.ID, product.ProductName, product.CurrentStock, "$" + product.Cost.ToString("N2"));
                        productListCount++;
                        command.colourReset();
                    } else if (product.ID > productListCount && productListCount <= storeStock.Count && product.CurrentStock > 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("{0,10} {1,25} {2,25} {3,15}", product.ID, product.ProductName, product.CurrentStock, "$" + product.Cost.ToString("N2"));
                        productListCount++;
                        command.colourReset();
                    }
                    else if (product.ID < productListCount && productListCount >= storeStock.Count) { command.displayError("End Of Items...Returning to first page"); productListCount = 0; displayProduct(storeName, storeStock); break; }

                }
                customerOptions(storeName, storeStock);
            }
            else if (user_inp.Equals("B".Trim(), StringComparison.OrdinalIgnoreCase))
            {
                command.displayMessage("Going to Previous Page");
                productListCount = 0;
                Console.ForegroundColor = ConsoleColor.White; Console.WriteLine("{0,10} {1,25} {2,25}", "ID", "Product Name", "Current Stock"); command.colourReset();
                foreach (var product in storeStock)
                {
                    if (productListCount < 5 && product.CurrentStock == 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("{0,10} {1,25} {2,25} {3,15}", product.ID, product.ProductName, product.CurrentStock, "$" + product.Cost.ToString("N2"));
                        productListCount++;
                        command.colourReset();
                    }
                    else if (productListCount < 5 && product.CurrentStock > 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("{0,10} {1,25} {2,25} {3,15}", product.ID, product.ProductName, product.CurrentStock, "$" + product.Cost.ToString("N2"));
                        productListCount++;
                        command.colourReset();
                    }
                }
                customerOptions(storeName, storeStock);
            }
            else if (user_inp.Equals("R".Trim(), StringComparison.OrdinalIgnoreCase))
            {
                command.displayMessageOneLine("Would you like book into a workshop before leaving? [Yes/No]: "); string bookWorkshopChoice = Console.ReadLine();
                if (bookWorkshopChoice.Equals("Yes".Trim(), StringComparison.OrdinalIgnoreCase)) { bookWorkshop(workshopTimes, storeName); } else { Menu.CustomerMenu Cmenu = new Menu.CustomerMenu(); Cmenu.displayMenu();  }
            }
            else if (user_inp.Equals("C".Trim(), StringComparison.OrdinalIgnoreCase))
            {
                command.displayMessage("Completing Transaction");
                if (purchaseComplete == true && itemCart.Count != 0) { printReciept(itemCart, storeStock, bookedTotal, storeName); }
                else if (purchaseComplete == false && itemCart.Count != 0) { purchaseComplete = true; printReciept(itemCart, storeStock, bookedTotal, storeName); }
                else if (itemCart.Count == 0) { purchaseComplete = false; printReciept(itemCart, storeStock, bookedTotal, storeName); }
            }
            else if (user_inp.Equals("W".Trim(), StringComparison.OrdinalIgnoreCase)) { showWorkShopTimes(storeName, jsonCommand.getWorkShopTimes(storeName)); bookWorkshop(workshopTimes, storeName); }
            else if (user_inp.Equals("S".Trim(), StringComparison.OrdinalIgnoreCase)) { command.displayMessageOneLine("Enter the Name of the Product: "); string productSearch = Console.ReadLine();  searchByProduct(storeStock, productSearch); }
            else if (jsonCommand.matchID(storeName, item_ID) == true) //Checks if the ID input was valid
            {
                command.displayMessageOneLine("Please Enter the Amount you would like to Purchase: "); string quant = Console.ReadLine(); int Quantity = command.convertInt(quant);
                if (command.checkInt(quant, Quantity) == true)
                {
                    if (purchaseComplete == false)
                    {
                        foreach (var product in storeStock)
                        {
                            if (product.ID == item_ID)
                            {
                                if (product.CurrentStock >= Quantity)
                                {
                                    command.displayMessageOneLine("|Product: " + product.ProductName + " \n|Quantity: " + Quantity + "\n");

                                    command.displayMessageOneLine("Would you like to Continue [Yes/No]: "); string choice = Console.ReadLine();

                                    if (choice.Trim().Equals("Yes".Trim(), StringComparison.OrdinalIgnoreCase))
                                    {
                                        //Process purchase product //
                                        addProduct(itemCart, storeStock, product.ProductName, product.Store, Quantity);
                                        purchaseProduct(product.ProductName, storeName, Quantity, storeStock);
                                        purchaseTotal += product.Cost * Quantity; displayItemCart();
                                        command.displayMessageOneLine("Keep Purchasing [Yes/No]: "); string more = Console.ReadLine();
                                        if (more.Trim().Equals("Yes".Trim(), StringComparison.OrdinalIgnoreCase))
                                        {
                                            displayProduct(storeName, jsonCommand.getStoreData(storeName));
                                            continue;
                                        }
                                        else if (more.Trim().Equals("No".Trim(), StringComparison.OrdinalIgnoreCase))
                                        {
                                            command.displayMessageOneLine("Would you like to book into a workshop? [Yes/No]: "); string workshop = Console.ReadLine();
                                            if (workshop.Trim().Equals("Yes".Trim(), StringComparison.OrdinalIgnoreCase)) { showWorkShopTimes(storeName, jsonCommand.getWorkShopTimes(storeName)); bookWorkshop(workshopTimes, storeName); return; } else { command.displayMessage("Ok. Returning to Menu"); return; }
                                        }
                                    }
                                    else if (choice.Trim().Equals("No".Trim(), StringComparison.OrdinalIgnoreCase))
                                    {
                                        command.displayMessageOneLine("Would you like to book into a workshop? [Yes/No]: "); string workshop = Console.ReadLine();
                                        if (workshop.Trim().Equals("Yes".Trim(), StringComparison.OrdinalIgnoreCase)) { showWorkShopTimes(storeName, jsonCommand.getWorkShopTimes(storeName)); bookWorkshop(workshopTimes, storeName); return; } else { command.displayMessage("Ok. Returning to Menu"); return; }
                                    }
                                    else
                                    {
                                        command.displayError("Invalid Input! Must be Yes/No");
                                    }
                                }
                                else if (product.CurrentStock < Quantity) { command.displayError("Not enough stock"); displayProduct(storeName, storeStock); return; }
                                else if (product.CurrentStock == 0) { command.displayError(product.CurrentStock + " in Stock "); displayProduct(storeName, storeStock); return; }
                            }
                            else
                            {
                                continue;
                            }
                        }
                    }
                    else if (purchaseComplete == true) { command.displayError("Your Shopping Session has Finished, Please exit and Log back in to Start Fresh"); }
                }
            }else { displayProduct(storeName, storeStock); }
        }

        #endregion

        /*
         * ShowWorkShopTimes: Displays Workshop Times to the user. If the specific workshop has 0 places available then the colour code will be red to display so. Otherwise 
         * Will display green if there is still space in the workshop time. 
         */
        public List<WorkshopTimes> showWorkShopTimes(string storeName, List<WorkshopTimes> workshopTimes)
        {
            command.displayTitle("Workshop Times");
            Console.WriteLine("{0,5} {1,25} {2,25} {3,15} {4,15} {5,30}", "ID", "Type", "Session Times", "No. of People Booked", "Spaces left", "Workshop Filled [True/False]");
            foreach (var session in workshopTimes)
            {
                if (session.avabililty != 0)
                {
                    command.colourChange();  Console.WriteLine("{0,5} {1,25} {2,25} {3,15} {4,15} {5,25}", session.ID, session.type, session.sessionTimes, session.numBooking, session.avabililty, session.full); command.colourReset();
                }else if(session.avabililty == 0)
                {
                    Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine("{0,5} {1,25} {2,25} {3,15} {4,15} {5,25}", session.ID, session.type, session.sessionTimes, session.numBooking, session.avabililty, session.full); command.colourReset();
                }
            }

            command.colourChange(); command.displayMessage("Green: [Session Not Full]"); command.colourReset(); Console.ForegroundColor = ConsoleColor.Red; command.displayMessage("Red: [Session Full]"); command.colourReset();

            Console.WriteLine();

            return workshopTimes;
        }

        /*
         * Displays the Current Item Cart To the User 
         */
        public void displayItemCart()
        {
            Console.WriteLine("{0,15} {1,15}", "Product Name", "Quantity");
            foreach (var item in itemCart) { Console.WriteLine("{0,15} {1,15}", item.ProductName, item.Quantity); }
        }

        public List<Workshop> showWorkShopBookings(string storeName, List<Workshop> workshopBookings)
        {
            command.displayTitle("Current Bookings");
            Console.WriteLine("{0,15} {1,25} {2,25} {3,25}", "Name", "Session", "Time", "Booking Reference");

            foreach (var bookings in workshopBookings)
            {
                Console.WriteLine("{0,15} {1,25} {2,25} {3,25}", bookings.Name, bookings.Session, bookings.Time, bookings.BookingRef);
            }
            Console.WriteLine();
            return workshopBookings;
        }

        #region displayWorkshops
        public void displayWorkShop(string storeName)
        {
            showWorkShopTimes(storeName, jsonCommand.getWorkShopTimes(storeName));
            command.displayMessageOneLine("Would you like to book into a workshop? [Yes/No]: "); string workshop = Console.ReadLine();
            if (workshop.Trim().Equals("Yes".Trim(), StringComparison.OrdinalIgnoreCase)) { bookWorkshop(jsonCommand.getWorkShopTimes(storeName), storeName); return; }
            else { command.displayMessage("Ok. Returning to Menu"); return; }
        }
        #endregion

        /*
         * Purchase Product: Processes the purchase and updates the quantity in the corresponding JSON File only if the Product Name matches and the Current Stock is Greater than the Quantity Requested.
         */
        #region purchaseProduct
        public void purchaseProduct(String productName, String StoreName, int Quantity, List<StoreStock> storeStock)
        {
            //Looping through the Stock Class//
            foreach (var product in storeStock)
            {
                if (productName == product.ProductName && product.CurrentStock >= Quantity)
                {
                    jsonCommand.updateQuantityStore(command.getJsonDataDirectory(StoreName.Trim(), "/Stores/") + "_inventory.json", productName, Quantity, "minus");
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

        /*
         * Print Reciept: Prints the reciept based on if the user has booked into a workshop or not. This is determined if the bookedTotal is 0 or not. The discount is added accordingly
         */
        public List<customerPurchase> printReciept(List<customerPurchase> products, List<StoreStock> storeStock, int bookedTotal, string storeName)
        {
            command.displayTitle("Purchased at: " + storeName.ToUpper() + " LTD");  Console.ForegroundColor = ConsoleColor.Green; command.displayMessage("Served By: Lu-Vuong ");
            Console.WriteLine("Date: " + DateTime.Now + "\n");

            if (bookedTotal == 0)
            {
                command.displayMessage("No Discount Added, Book into a Workshop along with your next purchase to get a 10% Discount!!!");
                command.displayMessage("Number of Items: " + itemCart.Count);
                Console.WriteLine("{0,15} {1,15} {2,15} {3,15}", "Item Number", "Product", "Quantity", "Price");
                for (int i = 0; i < products.Count; i++)
                {
                    for (int j = 0; j < storeStock.Count; j++)
                    {
                        if (products[i].ProductName == storeStock[j].ProductName)
                        {
                            Console.WriteLine("{0,15} {1,15} {2,15} {3,15}", products[i].purchaseItemNumber, products[i].ProductName, products[i].Quantity, "$" + storeStock[j].Cost * products[i].Quantity + ".00");
                        }
                    }
                }    
                command.displayMessage("Total Cost: " + "$" + purchaseTotal.ToString("N2"));
                command.displayTitle("THANK YOU FOR SHOPPING WITH US");
                
                return products;
            }
            else
            {
                purchaseTotal = purchaseTotal * 0.9;
                command.displayMessage("Printed Reciept: Booked Into Workshop! Added 10% Discount on Purchase! ");
                command.displayMessage("Number of Items: " + itemCart.Count);
                Console.WriteLine("{0,15} {1,15} {2,15} {3,15}", "Item Number", "Product", "Quantity", "Price");
                for (int i = 0; i < products.Count; i++)
                {
                    for (int j = 0; j < storeStock.Count; j++)
                    {
                        if (products[i].ProductName == storeStock[j].ProductName)
                        {
                            Console.WriteLine("{0,15} {1,15} {2,15} {3,15}", products[i].purchaseItemNumber, products[i].ProductName, products[i].Quantity, "$" + storeStock[j].Cost * products[i].Quantity + ".00");
                        }
                    }
                }
                command.displayMessage("Total Cost: " + "$" + purchaseTotal.ToString("N2"));
                command.displayTitle("THANK YOU FOR SHOPPING WITH US"); Console.ResetColor(); return products;
            }
        }

        /*
         * Add Product: Adds a new product and creates the item in the customer purchase list
         */
        public void addProduct(List<customerPurchase> purchasedProducts, List<StoreStock> storeStock, String productName, String StoreName, int Quantity)
        {
            //Looping through the Stock Class//
            foreach (var product in storeStock)
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
                        purchasedProducts.Add(new customerPurchase(purchaseNumber++, StoreName.Trim(), productName, Quantity));
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

        /*
         * Book Workshop: Prompts the user for the workshop ID and then confirms if the user wants to book. If the session is full however, an error message will be returned 
         * Otherwise the booking calls upon addBooking to add the booking Item.
         */
        public void bookWorkshop(List<WorkshopTimes> workshopTimes, string storeName)
        {
            Console.WriteLine("\nYou have Booked into " + bookedTotal + " Workshops");
            command.displayMessageOneLine("\nEnter the Workshop ID you would like to book into: "); string book = Console.ReadLine(); int workshopID = command.convertInt(book);
            if (command.checkInt(book, workshopID))
            {
                if (workshopID <= workshopTimes.Count())
                {
                    command.displayMessage("Youve chosen: " + workshopID);
                    Console.WriteLine("{0,15} {1,25} {2,25}", "ID", "Session Type", "Session Time");
                    foreach (var chosen in workshopTimes)
                    {
                        if (chosen.ID == workshopID)
                        {
                            Console.WriteLine("{0,15} {1,25} {2,25}", chosen.ID, chosen.type, chosen.sessionTimes);
                            if (chosen.avabililty != 0)
                            {
                                if (command.Continue("Confirm Booking?") == true)
                                {
                                    command.displayMessageOneLine("\nEnter Full Name: "); string name = Console.ReadLine();
                                    Console.WriteLine();
                                    if (bookRef == false) { bookingRef = command.generateBookingReference(7); bookRef = true; addBooking(workshopTimes, jsonCommand.getBookings(storeName), workshopID, storeName, name, bookingRef, chosen.sessionTimes, chosen.type); }

                                    else if (bookRef == true) { command.displayMessageOneLine(name + " Your Booking Reference is: " + bookingRef + "\n"); addBooking(workshopTimes, jsonCommand.getBookings(storeName), workshopID, storeName, name, bookingRef, chosen.sessionTimes, chosen.type); }

                                    break;
                                }else { command.displayMessage("Ok. Returning to Menu"); break; }
                            }else if(chosen.avabililty == 0) { command.displayError("Places for this session are full"); return; }
                        }
                        else if (chosen.ID != workshopID)
                        {
                            continue;
                        }
                    }
                }else { command.displayError("No Such ID"); bookWorkshop(workshopTimes, storeName); ; }
            } else { bookWorkshop(workshopTimes, storeName); }
        }

        /*
         * Add Booking: Checks if the booking is already made through the users and workshop details. If the user is already booked into that session they cannot book again
         * Otherwise the new booking will be displayed to the user.
         */
        public void addBooking(List<WorkshopTimes> workshopTimes, List<Workshop> workshopBookings, int ID, string storename, string name, string bookingRef, string time, string session)
        {
            //Creating a new Local List of type Stock//
            List<Workshop> workshop = new List<Workshop>();

            //Reading through the stockrequests.json file and with that jsonData, converting the json data into Object List<Stock> //
            using (var streamReader = new StreamReader(command.getJsonDataDirectory(storename.Trim(), "/Workshops/") + "_bookings.json"))
            {
                var jsonData = streamReader.ReadToEnd(); workshop = JsonConvert.DeserializeObject<List<Workshop>>(jsonData);
            }

            foreach (var times in workshopTimes)
            {
                if (ID == times.ID)
                {
                    foreach (var booking in workshop)
                    {
                            if (checkBooking(jsonCommand.getBookings(storename),name, bookingRef, storename, session, time) == false) 
                            {
                                workshop.Add(new Workshop(name, session, time, bookingRef));
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine("|Name: " + workshop.LastOrDefault().Name + "\n|Session: " + workshop.LastOrDefault().Session + "\n|Time: " +
                                                    workshop.LastOrDefault().Time + "\n|Booking Reference: " + workshop.LastOrDefault().BookingRef); break;
                            }else if(checkBooking(jsonCommand.getBookings(storename), name, bookingRef, storename, session, time) == true)
                            {
                                command.displayError("You are already booked into this Session"); return;
                            }
                    }
                    Console.ResetColor();
                    updateBookingDetails(workshopTimes, ID, storename.Trim(), name, bookingRef, time, session);
                } 
            }
            var addBooking = JsonConvert.SerializeObject(workshop, Formatting.Indented);
            File.WriteAllText(command.getJsonDataDirectory(storename.Trim(), "/Workshops/") + "_bookings.json", addBooking);
            bookedTotal++;
        }

        /*
         * Check Booking: Checks the booking based on The users name,session and time. 
         * if the booking ref is equal it will return false else true
         */
        public bool checkBooking(List<Workshop> workshopBookings, string name, string bookingref, string storename,string session, string time)
        {
            foreach (var bookings in workshopBookings)
            {
                if (name == bookings.Name && bookings.Session == session && bookings.Time == time)
                {
                    if (bookings.BookingRef == bookingRef)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }else { continue; }
            }
            return false;
        }

        /*
         * Update the JSON file for the corresponding workshop file in the store
         */
        public List<WorkshopTimes> updateBookingDetails(List<WorkshopTimes> workshopTimes, int ID, string storename, string name, string bookingRef, string time, string session)
        {
            foreach (var times in workshopTimes)
            {
                if (ID == times.ID)
                {
                    if (times.avabililty > 0)
                    {
                        times.numBooking = times.numBooking + 1; times.avabililty = times.avabililty - 1;
                        command.colourChange(); command.displayMessage("Your Booking is Confirmed!"); command.colourReset();
                        if (times.avabililty == 0) { times.full = true; }
                        break;
                    }
                }
                else
                {
                    continue;
                }
            }

            var updatedBookingDetails = JsonConvert.SerializeObject(workshopTimes, Formatting.Indented);
            File.WriteAllText(command.getJsonDataDirectory(storename.Trim(), "/Workshops/") + "_workshopTimes.json", updatedBookingDetails);
            return workshopTimes;
        }

        /*
         * Search By Product: Searches the List for a item with the same name, regardless or plural or singular
         * Example: the item Cards will be found if written as 'cards' or 'card'
         */
        public List<StoreStock> searchByProduct(List<StoreStock> storeStock, string ProductName)
        {
            if (storeStock.Any(item => item.ProductName.Trim().Equals(ProductName.Trim(), StringComparison.OrdinalIgnoreCase)) ||
                storeStock.Any(item => item.ProductName.Trim().Remove(item.ProductName.Length - 1).Equals(ProductName.Trim(), StringComparison.OrdinalIgnoreCase)))
            {
                Console.ForegroundColor = ConsoleColor.White; Console.WriteLine("{0,10} {1,25} {2,25} {3,15}", "ID", "Product Name", "Current Stock", "Cost"); command.colourReset();
                foreach (var product in storeStock)
                {
                    if (product.ProductName.Trim().Equals(ProductName.Trim(), StringComparison.OrdinalIgnoreCase) || product.ProductName.Trim().Remove(product.ProductName.Length - 1).Equals(ProductName.Trim(),StringComparison.OrdinalIgnoreCase))
                    {
                        Console.WriteLine("{0,10} {1,25} {2,25} {3,15}", product.ID, product.ProductName, product.CurrentStock, "$" + product.Cost.ToString("N2"));

                        command.displayMessageOneLine("Would you like to purchase this Item? [Yes/No]: "); string purchase = Console.ReadLine();

                        if (purchase.Trim().Equals("Yes".Trim(), StringComparison.OrdinalIgnoreCase))
                        {
                            command.displayMessageOneLine("Enter Quantity: "); string quant = Console.ReadLine(); int Quantity = command.convertInt(quant);
                            if (command.checkInt(quant, Quantity) == true)
                            {
                                addProduct(itemCart, storeStock, product.ProductName, product.Store, Quantity); purchaseProduct(product.ProductName, product.Store, Quantity, storeStock);
                                purchaseTotal += product.Cost; displayItemCart(); displayProduct(product.Store, storeStock);
                            }
                        }
                    }
                    else { continue; }
                }
            }else { command.displayError("No Product found with that name"); }
            return storeStock;
        }
    }
}
