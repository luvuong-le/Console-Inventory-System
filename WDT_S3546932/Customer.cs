using Newtonsoft.Json;
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

        int productListCount; int purchaseNumber = 0; double purchaseTotal = 0; int bookedTotal = 0; string bookingRef = null;

        bool purchaseComplete = false; bool bookRef = false;

        List<customerPurchase> itemCart = new List<customerPurchase>();

        #region displayProduct
        public override List<StoreStock> displayProduct(string store)
        {
            productListCount = 0;
            List<StoreStock> products = JsonConvert.DeserializeObject<List<StoreStock>>(jsonCommand.JsonReader(command.getJsonDataDirectory(store, "/Stores/") + "_inventory.json"));

            Console.ForegroundColor = ConsoleColor.White;  Console.WriteLine("{0,10} {1,25} {2,25} {3,15}", "ID", "Product Name", "Current Stock", "Cost"); command.colourReset();
            foreach (var product in products)
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

            customerOptions(store, products);

            return products;
        }
        #endregion

        #region CustomerOptions
        public void customerOptions(string storeName, List<StoreStock> store)
        {
            List<WorkshopTimes> workshopTimes = JsonConvert.DeserializeObject<List<WorkshopTimes>>(jsonCommand.JsonReader(command.getJsonDataDirectory(storeName, "/Workshops/") + "_workshopTimes.json"));
            command.colourChange();  command.displayMessage("Green: [Stock Available]"); command.colourReset(); Console.ForegroundColor = ConsoleColor.Red; command.displayMessage("Red: [Out of Stock]"); command.colourReset();
            Console.ForegroundColor = ConsoleColor.White;  command.displayMessage("[Legend: 'P' Next Page | 'R' Return to Menu  | 'B' Previous Page | 'C' Complete Transaction | 'W' Book Workshop | ID Number Based on Item | 'S' Search by Name]"); command.colourReset();

            command.displayMessageOneLine("Enter Item ID Number to Purchase or Function: "); string user_inp = Console.ReadLine(); int item_ID; Int32.TryParse(user_inp, out item_ID);

            if (user_inp.Equals("P", StringComparison.OrdinalIgnoreCase))
            {
                command.displayMessage("Going to Next Page");

                foreach (var product in store)
                {
                    if (product.ID > productListCount && productListCount <= store.Count && product.CurrentStock == 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("{0,10} {1,25} {2,25} {3,15}", product.ID, product.ProductName, product.CurrentStock, "$" + product.Cost.ToString("N2"));
                        productListCount++;
                        command.colourReset();
                    } else if (product.ID > productListCount && productListCount <= store.Count && product.CurrentStock > 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("{0,10} {1,25} {2,25} {3,15}", product.ID, product.ProductName, product.CurrentStock, "$" + product.Cost.ToString("N2"));
                        productListCount++;
                        command.colourReset();
                    }
                    else if (product.ID < productListCount && productListCount >= store.Count) { command.displayError("End Of Items...Returning to first page"); productListCount = 0; displayProduct(storeName); break; }

                }
                customerOptions(storeName, store);
            }
            else if (user_inp.Equals("B", StringComparison.OrdinalIgnoreCase))
            {
                command.displayMessage("Going to Previous Page");
                productListCount = 0;
                Console.ForegroundColor = ConsoleColor.White; Console.WriteLine("{0,10} {1,25} {2,25}", "ID", "Product Name", "Current Stock"); command.colourReset();
                foreach (var product in store)
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
                customerOptions(storeName, store);
            }
            else if (user_inp.Equals("R", StringComparison.OrdinalIgnoreCase))
            {
                Menu.CustomerMenu Cmenu = new Menu.CustomerMenu(); Cmenu.displayMenu();
            }
            else if (user_inp.Equals("C", StringComparison.OrdinalIgnoreCase))
            {
                command.displayMessage("Completing Transaction");
                if (purchaseComplete == true && itemCart.Count != 0) { printReciept(itemCart, store, bookedTotal, storeName); }
                else if (purchaseComplete == false && itemCart.Count != 0) { purchaseComplete = true; printReciept(itemCart, store, bookedTotal, storeName); }
                else if (itemCart.Count == 0) { purchaseComplete = false; printReciept(itemCart, store, bookedTotal, storeName); }
            }
            else if (user_inp.Equals("W", StringComparison.OrdinalIgnoreCase)) { bookWorkshop(workshopTimes, storeName); }
            else if (user_inp.Equals("S", StringComparison.OrdinalIgnoreCase)) { command.displayMessageOneLine("Enter the Name of the Product: "); string productSearch = Console.ReadLine();  searchByProduct(store,productSearch); }
            else if (jsonCommand.matchID(storeName, item_ID) == true) //Checks if the ID input was valid
            {
                command.displayMessageOneLine("Please Enter the Amount you would like to Purchase: "); string quant = Console.ReadLine(); int Quantity = command.convertInt(quant);
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
                                    command.displayMessageOneLine("|Product: " + product.ProductName + " \n|Quantity: " + Quantity + "\n");

                                    command.displayMessageOneLine("Would you like to Continue [Yes/No]: "); string choice = Console.ReadLine();

                                    if (choice.Equals("Yes", StringComparison.OrdinalIgnoreCase))
                                    {
                                        //Process purchase product  //
                                        addProduct(itemCart, store, product.ProductName, product.Store, Quantity);
                                        purchaseProduct(product.ProductName, storeName, Quantity);
                                        purchaseTotal += product.Cost; displayItemCart();
                                        command.displayMessageOneLine("Keep Purchasing [Yes/No]: "); string more = Console.ReadLine();
                                        if (more.Equals("Yes", StringComparison.OrdinalIgnoreCase))
                                        {
                                            productListCount = 0;
                                            displayProduct(storeName);
                                            continue;
                                        }
                                        else if (more.Equals("No", StringComparison.OrdinalIgnoreCase))
                                        {
                                            //purchaseComplete = true;
                                            command.displayMessageOneLine("Would you like to book into a workshop? [Yes/No]: "); string workshop = Console.ReadLine();
                                            //Compare workshops entered to purchasecOMPLETE to see if discount is added // //Workshopbooked = true/false //
                                            if (workshop == "Yes" || workshop == "yes") { displayWorkShop(storeName); bookWorkshop(workshopTimes, storeName); return; } else { command.displayMessage("Ok. Returning to Menu"); return; }
                                        }
                                    }
                                    else if (choice.Equals("No", StringComparison.OrdinalIgnoreCase))
                                    {
                                        //purchaseComplete = false;
                                        command.displayMessageOneLine("Would you like to book into a workshop? [Yes/No]: "); string workshop = Console.ReadLine();
                                        //Compare workshops entered to purchasecOMPLETE to see if discount is added // //Workshopbooked = true/false //
                                        if (workshop.Equals("Yes", StringComparison.OrdinalIgnoreCase)) { displayWorkShop(storeName); bookWorkshop(workshopTimes, storeName); return; } else { command.displayMessage("Ok. Returning to Menu"); return; }
                                    }
                                    else
                                    {
                                        command.displayError("Invalid Input! Must be Yes/No");
                                    }
                                }
                                else if (product.CurrentStock < Quantity) { command.displayError("Not enough stock"); displayProduct(storeName); return; }
                                else if (product.CurrentStock == 0) { command.displayError(product.CurrentStock + " in Stock "); displayProduct(storeName); return; }
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

        public List<WorkshopTimes> showWorkShopTimes(string storeName)
        {
            List<WorkshopTimes> workshopsTimes = JsonConvert.DeserializeObject<List<WorkshopTimes>>(jsonCommand.JsonReader(command.getJsonDataDirectory(storeName, "/Workshops/") + "_workshopTimes.json"));

            command.displayTitle("Session Times");
            Console.WriteLine("{0,5} {1,25} {2,25} {3,15} {4,15} {5,25}", "ID", "Type", "Session Times", "Number of People Booked", "Places left", "Workshop Availability");
            foreach (var session in workshopsTimes)
            {
                if (session.full == true)
                {
                    command.colourChange();  Console.WriteLine("{0,5} {1,25} {2,25} {3,15} {4,15} {5,25}", session.ID, session.type, session.sessionTimes, session.numBooking, session.avabililty, session.full); command.colourReset();
                }else if(session.full == false)
                {
                    Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine("{0,5} {1,25} {2,25} {3,15} {4,15} {5,25}", session.ID, session.type, session.sessionTimes, session.numBooking, session.avabililty, session.full); command.colourReset();
                }
            }

            command.colourChange(); command.displayMessage("Green: [Session Not Full]"); command.colourReset(); Console.ForegroundColor = ConsoleColor.Red; command.displayMessage("Red: [Session Full]"); command.colourReset();

            Console.WriteLine();

            return workshopsTimes;
        }

        public void displayItemCart()
        {
            Console.WriteLine("{0,15} {1,15}", "Product Name", "Quantity");
            foreach (var item in itemCart) { Console.WriteLine("{0,15} {1,15}", item.ProductName, item.Quantity); }
        }
        public List<Workshop> showWorkShopBookings(string storeName)
        {
            List<Workshop> workshopBookings = JsonConvert.DeserializeObject<List<Workshop>>(jsonCommand.JsonReader(command.getJsonDataDirectory(storeName, "/Workshops/") + "_bookings.json"));

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
        public override void displayWorkShop(string storeName)
        {
            showWorkShopTimes(storeName);

            showWorkShopBookings(storeName);

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

        public List<customerPurchase> printReciept(List<customerPurchase> products, List<StoreStock> store, int bookedTotal, string storeName)
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
                    for (int j = 0; j < store.Count; j++)
                    {
                        if (products[i].ProductName == store[j].ProductName)
                        {
                            Console.WriteLine("{0,15} {1,15} {2,15} {3,15}", products[i].purchaseItemNumber, products[i].ProductName, products[i].Quantity, "$" + store[j].Cost.ToString("N2"));
                        }
                    }
                }    
                command.displayMessage("Total Cost: " + "$" +purchaseTotal.ToString("N2"));
                command.displayTitle("THANK YOU FOR SHOPPING WITH US");
                
                return products;
            }
            else
            {
                command.displayMessage("Printed Reciept: Booked Into Workshop! Added 10% Discount on Purchase! ");
                command.displayMessage("Number of Items: " + itemCart.Count);
                for (int i = 0; i < products.Count; i++)
                {
                    for (int j = 0; j < store.Count; j++)
                    {
                        if (products[i].ProductName == store[j].ProductName)
                        {
                            Console.WriteLine("{0,15} {1,15} {2,15} {3,15}", products[i].purchaseItemNumber, products[i].ProductName, products[i].Quantity, "$" + store[j].Cost.ToString("N2"));
                        }
                    }
                }
                command.displayMessage("Total Cost: " + "$" + purchaseTotal.ToString("N2"));
                command.displayTitle("THANK YOU FOR SHOPPING WITH US"); Console.ResetColor(); return products;
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
            showWorkShopTimes(storeName);
            Console.WriteLine("You have Booked into " + bookedTotal + " Workshops");
            command.displayMessageOneLine("\n\nEnter the Workshop ID you would like to book into: "); string book = Console.ReadLine(); int workshopID = command.convertInt(book);
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
                            if (chosen.full == true)
                            {
                                if (command.Continue("Confirm Booking?") == true)
                                {
                                    command.displayMessageOneLine("\nEnter Name: "); string name = Console.ReadLine();
                                    Console.WriteLine();
                                    if (bookRef == false) { bookingRef = command.generateBookingReference(7); bookRef = true; addBooking(workshopTimes, workshopID, storeName, name, bookingRef, chosen.sessionTimes, chosen.type); }

                                    else if (bookRef == true) { command.displayMessageOneLine(name + " Your Booking Reference is: " + bookingRef + "\n"); addBooking(workshopTimes, workshopID, storeName, name, bookingRef, chosen.sessionTimes, chosen.type); }

                                    break;
                                }
                            }else if(chosen.full == false) { command.displayError("Places for this session are full"); return; }
                        }
                        else if (chosen.ID != workshopID)
                        {
                            continue;
                        }
                    }
                }else { command.displayError("No Such ID"); bookWorkshop(workshopTimes, storeName); ; }
            } else { bookWorkshop(workshopTimes, storeName); }
        }

        public void addBooking(List<WorkshopTimes> workshopTimes, int ID, string storename, string name, string bookingRef, string time, string session)
        {
            List<Workshop> workshopBookings = JsonConvert.DeserializeObject<List<Workshop>>(jsonCommand.JsonReader(command.getJsonDataDirectory(storename, "/Workshops/") + "_bookings.json"));

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
                    foreach (var booking in workshop)
                    {
                            if (checkBooking(name, bookingRef, storename, session, time) == false) 
                            {
                                workshop.Add(new Workshop(name, session, time, bookingRef));
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine("|Name: " + workshop.LastOrDefault().Name + "\n|Session: " + workshop.LastOrDefault().Session + "\n|Time: " +
                                                    workshop.LastOrDefault().Time + "\n|Booking Reference: " + workshop.LastOrDefault().BookingRef); break;
                            }else if(checkBooking(name, bookingRef, storename, session, time) == true)
                            {
                                command.displayError("You are already booked into this Session"); return;
                            }
                    }
                    Console.ResetColor();
                    updateBookingDetails(workshopTimes, ID, storename, name, bookingRef, time, session);
                } 
            }
            var addBooking = JsonConvert.SerializeObject(workshop, Formatting.Indented);
            File.WriteAllText(command.getJsonDataDirectory(storename, "/Workshops/") + "_bookings.json", addBooking);
            bookedTotal++;
        }

        public bool checkBooking(string name, string bookingref, string storename,string session, string time)
        {
            List<Workshop> workshopBookings = JsonConvert.DeserializeObject<List<Workshop>>(jsonCommand.JsonReader(command.getJsonDataDirectory(storename, "/Workshops/") + "_bookings.json"));

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
            File.WriteAllText(command.getJsonDataDirectory(storename, "/Workshops/") + "_workshopTimes.json", updatedBookingDetails);
            return workshopTimes;
        }

        public List<StoreStock> searchByProduct(List<StoreStock> store, string ProductName)
        {
            Console.ForegroundColor = ConsoleColor.White; Console.WriteLine("{0,10} {1,25} {2,25} {3,15}", "ID", "Product Name", "Current Stock", "Cost"); command.colourReset();
            foreach (var product in store)
            {
                if(ProductName == product.ProductName)
                {
                    Console.WriteLine("{0,10} {1,25} {2,25} {3,15}", product.ID, product.ProductName, product.CurrentStock, "$" + product.Cost.ToString("N2"));

                    command.displayMessageOneLine("Would you like to purchase this Item? [Yes/No]: "); string purchase = Console.ReadLine();

                    if (purchase.Equals("Yes", StringComparison.OrdinalIgnoreCase))
                    {
                        command.displayMessageOneLine("Enter Quantity: "); string quant = Console.ReadLine(); int Quantity = command.convertInt(quant);
                        if (command.checkInt(quant, Quantity) == true)
                        {
                            addProduct(itemCart, store, product.ProductName, product.Store, Quantity); purchaseProduct(product.ProductName, product.Store, Quantity);
                            purchaseTotal += product.Cost; displayItemCart(); displayProduct(product.Store);
                        }
                    }
                }else { continue; }
            }
            return store;
        }
    }
}
