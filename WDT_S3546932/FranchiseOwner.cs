using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WDT_S3546932
{
    class Franchise : FranchiseCLI
    {
        Owner owner = new Owner();

        Utility command = new Utility();

        JsonUtility jsonCommand = new JsonUtility();
        
        /*
         * Add Inventory: Creates a new list of the owner inventory and store stock. 
         * Foreach item in the list these new lists will store the ID and then run a comparison with each of the new lists to see which id is not there
         * Which ever item is different the user is able to choose that item and add it to their store inventory. If the item already exists there will be an error message.
         */

        #region AddInventory
        //Add New Inventory 
        public void AddNewInventory(string storeName, List<OwnerStock> OwnerInventory, List<StoreStock> storeStock)
        {
            //Display List of Products in OwnerInventory that is not Currently in Store File //
            //Display Owner Stock // 
            command.displayMessage("Owner Inventory: ");

            Console.WriteLine("{0,10} {1,25} {2,25}", "ID", "Product Name", "Current Stock");

            foreach (var product in OwnerInventory)
            {
                Console.WriteLine("{0,10} {1,25} {2,25}", product.ID, product.ProductName, product.CurrentStock);
            }

            command.displayMessage("Your Store Inventory: ");
            //Display Store Iventory
            if (storeStock.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine("{0,15}", "[ALERT] Nothing in Store Right Now"); command.colourReset();
            }
            else
            {
                Console.WriteLine("{0,10} {1,25} {2,25}", "ID", "Product Name", "Current Stock");

                foreach (var store in storeStock)
                {
                    Console.WriteLine("{0,10} {1,25} {2,25}", store.ID, store.ProductName, store.CurrentStock);
                }
            }

            int ownerCount = OwnerInventory.Count; int storeCount = storeStock.Count;

            if(storeCount != ownerCount)
            {
                command.displayMessage("You are missing the following Items");

                //Go through each file and return the IDs // 
                List<int> storeList = new List<int>();

                foreach (var store in storeStock)
                {
                    storeList.Add(store.ID);
                }

                List<int> ownerList = new List<int>();

                foreach (var owner in OwnerInventory)
                {
                    ownerList.Add(owner.ID);
                }
                //For each ID Array returned compare the two arrays//

                List<int> difference = ownerList.Except(storeList).ToList(); //Compare the OwnerInventory to the Store and determine which ID they do not have //

                // Go through the Owner Stock and make  REQUEST TO STOCK and update to the store file and add that item in ///

                foreach (var product in OwnerInventory)
                {
                    foreach (var id in difference)
                    {
                        if (product.ID == id)
                        {
                            Console.WriteLine("{0,10} {1,25} {2,25}", product.ID, product.ProductName, product.CurrentStock);
                        }
                    }
                }

                command.displayMessageOneLine("\nEnter Product ID Number to Add to Store: "); string ID = Console.ReadLine(); int itemID = command.convertInt(ID);

                if (command.checkInt(ID, itemID) == true)
                {
                    if (difference.Any(item => item == itemID))
                    {
                        //Prompt User to Choose an ID Number and Request it Based on the List //
                        foreach (var test in difference)
                        {
                            foreach (var product in OwnerInventory)
                            {
                                if (itemID == test && product.ID == itemID)
                                {
                                    command.displayMessageOneLine("\nPlease Enter the Amount you would like to add: "); string quant = Console.ReadLine(); int Quantity = command.convertInt(quant);

                                    if (command.checkInt(quant, Quantity) == true)
                                    {

                                        command.displayMessageOneLine("\nYouve chosen the Product");

                                        Console.WriteLine("\n{0,0} {1,15} {2,15}", product.ID, product.ProductName, "Quantity: " + Quantity);

                                        command.displayMessageOneLine("Would you like to Continue [Yes/No]: "); string choice = Console.ReadLine();
                                        if (choice.Equals("Yes".Trim(), StringComparison.OrdinalIgnoreCase))
                                        {
                                            //Process Add Inventory //
                                            requestForStock(product.ProductName, storeName, Quantity);
                                            command.displayMessage("Successfully Requested Product, Please wait for a response from the Head Office.");
                                            AddProduct(product.ProductName, storeName, Quantity = 0);
                                        }
                                        else if (choice.Equals("No".Trim(), StringComparison.OrdinalIgnoreCase)) { command.displayMessage("Ok. Returning to Menu"); }
                                        return;
                                    }
                                }
                            }
                        }
                    }
                    else if(difference.Any(item => item != itemID)) { command.displayError("You already have that Item in Stock"); return; } 
                    else if(difference.Any(item => itemID > item)) { command.displayError("No Such ID"); return; }
                }
                }else { command.displayError("You already Have All Stock In Your Inventory"); return; }
            }

        #endregion

        #region DisplayInventory
        //Display Inventory and Request For Stock in StockRequest.json //
        public List<StoreStock> displayInventory(string StoreName, List<StoreStock> storeStock)
        {
            command.displayMessageOneLine("Enter a Threshold for Re-Stocking: "); string Thres = Console.ReadLine(); int thres = command.convertInt(Thres);

            if (command.checkInt(Thres, thres) == true)
            {
                Console.ForegroundColor = ConsoleColor.White;  Console.WriteLine("\n{0,10} {1,25} {2,25} {3,35}", "ID", "Product Name", "Current Stock", "Re-Stock"); command.colourReset();

                foreach (var product in storeStock)
                {
                    if (product.CurrentStock <= thres)
                    {
                        command.colourChange();
                        Console.WriteLine("{0,10} {1,25} {2,25} {3,35}", product.ID, product.ProductName, product.CurrentStock, product.ReStock = true); command.colourReset();
                    }else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("{0,10} {1,25} {2,25} {3,35}", product.ID, product.ProductName, product.CurrentStock, product.ReStock = false); command.colourReset();
                    }
                }
                command.colourChange(); command.displayMessage("Green: [Threshold that needs Restocking]"); command.colourReset(); Console.ForegroundColor = ConsoleColor.Red; command.displayMessage("Red: [Below Threshold, Does not need Restocking]"); command.colourReset();
            }
            else { displayInventory(StoreName, storeStock); }

            command.displayMessageOneLine("\nEnter Request To Process[ID]: "); string requestid = Console.ReadLine(); int requestID = command.convertInt(requestid); 
    
            if (command.checkInt(requestid, requestID) == true) 
            {
                command.displayMessageOneLine("Please Enter the Quantity you would like to Purchase: "); string quant = Console.ReadLine(); int Quantity = command.convertInt(quant);
                if (command.checkInt(quant, Quantity) == true)
                {
                    foreach (var product in storeStock)
                    {
                        if (product.ID == requestID)
                        {
                            if (product.ReStock == true)
                            {
                                requestForStock(product.ProductName, StoreName, Quantity);
                                break;
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Red; command.displayMessageOneLine("[ERROR] You have enough stock, Would you like to Continue [Yes/No]: "); string choice = Console.ReadLine(); command.colourReset();
                                if (choice.Equals("Yes".Trim(), StringComparison.OrdinalIgnoreCase))
                                {
                                    requestForStock(product.ProductName, StoreName, Quantity);
                                }
                                else if (choice.Equals("No".Trim(), StringComparison.OrdinalIgnoreCase)) { command.displayMessage("Ok. Returning to Menu"); break; }
                                break;
                            }
                        }
                    }
                }
            }else { Menu.FranchiseOwner franchiseMenu = new Menu.FranchiseOwner(); franchiseMenu.displayMenu(); }
            return storeStock;
        }
        #endregion

        #region DisplayInventoryThres
        // Display Inventory Based on Threshold
        public List<StoreStock> displayInventoryThres(string StoreName, List<StoreStock> storeStock)
        {
            int productRestock = 0;
            command.displayMessageOneLine("Enter a Threshold for Re-Stocking: "); string Thres = Console.ReadLine(); int thres = command.convertInt(Thres);

            if (command.checkInt(Thres, thres) == true)
            {
                Console.ForegroundColor = ConsoleColor.White;  Console.WriteLine("\n{0,10} {1,25} {2,25} {3,35}", "ID", "Product Name", "Current Stock", "Re-Stock"); command.colourReset();

                foreach (var product in storeStock)
                {
                    if (product.CurrentStock <= thres)
                    {
                        productRestock += 1;
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine("{0,10} {1,25} {2,25} {3,35}", product.ID, product.ProductName, product.CurrentStock, product.ReStock = true); command.colourReset();
                    }else if(productRestock == 0) { command.displayError("No Items to be Restocked!"); Menu.FranchiseOwner Fmenu = new Menu.FranchiseOwner(); Fmenu.displayMenu(); }
                }
                Console.ForegroundColor = ConsoleColor.Cyan; command.displayMessage("Blue: [All Items based on threshold that need Restocking]"); command.colourReset();
            }
            else { displayInventoryThres(StoreName, storeStock); }


            command.displayMessageOneLine("\nEnter Request To Process[ID]: "); string requestid = Console.ReadLine(); int requestID = command.convertInt(requestid);
            if (command.checkInt(requestid, requestID) == true)
            {
                command.displayMessageOneLine("Please Enter the Quantity you would like to Purchase: "); string quant = Console.ReadLine(); int Quantity = command.convertInt(quant);
                if (command.checkInt(quant, Quantity))
                {
                    foreach (var product in storeStock)
                    {
                        if (product.ReStock == true && product.CurrentStock <= thres)
                        {
                            if (product.ID == requestID)
                            {
                                requestForStock(product.ProductName, StoreName, Quantity);
                                break;
                            }
                        }
                        else
                        {
                            command.displayError("Not in the List");
                            break;
                        }
                    }
                }
            } else { Menu.FranchiseOwner franchiseMenu = new Menu.FranchiseOwner(); franchiseMenu.displayMenu(); }
            return storeStock;
        }
        #endregion

        /*
         * Request Stock: Makes a request for stock based on the product name and quantity. IF the product name is valid the request is made and left until the owner processes the request.
         */

        #region requestStock
        //Request For Stock, Appends to the StockRequest.json File//
        public List<Stock> requestForStock(string productName, string StoreName, int Quantity)
        {
            //Access Owner Inventory //
            List<OwnerStock> ownerInventory = jsonCommand.getOwnerFile();

            command.displayMessage("Requesting");

            //Creating a new Local List of type Stock//
            List<Stock> stock = new List<Stock>();

            //Reading through the stockrequests.json file and with that jsonData, converting the json data into Object List<Stock> //
            using (var streamReader = new StreamReader(command.getJsonDataDirectory("stockrequests".Trim(), "/Stock/") + ".json"))
            {
                var jsonData = streamReader.ReadToEnd(); stock = JsonConvert.DeserializeObject<List<Stock>>(jsonData);
            }
            
            //Looping through the Owner Inventory To find a Match, If a match is found, Add new Stock Request//
            foreach (var product in ownerInventory)
            {
                    if (productName == product.ProductName)
                    {
                        if (Quantity > owner.checkCurrentStock(product.ProductName)) { product.StockAvailability = false; } else { product.StockAvailability = true; }

                        //Adding a new Object using the already available stock object //
                        stock.Add(new Stock(jsonCommand.lastRequestID() + 1, StoreName.ToUpper(), product.ProductName, Quantity, owner.checkCurrentStock(productName), false, product.StockAvailability));
                    }
            }

            //Append the Results and convert the Object back into a JSON String //
            var appendRequest = JsonConvert.SerializeObject(stock, Formatting.Indented);
            File.WriteAllText(command.getJsonDataDirectory("stockrequests".Trim(), "/Stock/") + ".json", appendRequest);
            return stock; //Returns New List of Stock Requests
        }
        #endregion

        /*
         * Add Product: Addes the product to the current stores stock
         */
        #region addProduct
        public void AddProduct(String productName, String StoreName, int Quantity)
        {
            //Creating a new Local List of type Stock//
            List<StoreStock> stock = new List<StoreStock>();

            //Reading through the stockrequests.json file and with that jsonData, converting the json data into Object List<Stock> //
            using (var streamReader = new StreamReader(command.getJsonDataDirectory(StoreName.Trim(), "/Stores/") + "_inventory.json"))
            {
                var jsonData = streamReader.ReadToEnd(); stock = JsonConvert.DeserializeObject<List<StoreStock>>(jsonData);
            }

            List<OwnerStock> ownerInventory = jsonCommand.getOwnerFile();

            //Looping through the Stock Class//
            foreach (var product in ownerInventory)
            {
                if (productName == product.ProductName && product.CurrentStock > Quantity)
                {
                    //Adding a new Object using the already available stock object //
                    stock.Add(new StoreStock(product.ID, StoreName.ToUpper(), productName, product.CurrentStock = Quantity, false, Math.Round(product.Cost,2)));
                }else if(productName == product.ProductName && product.CurrentStock < Quantity)
                {
                    command.displayError("Sorry We have: " + product.CurrentStock + " In Stock at the moment");
                    return;
                }else if(productName == product.ProductName && product.CurrentStock == 0)
                {
                    command.displayError("We do not have any in stock at the moment");
                    return;
                }
            }

            //Append the Results and convert the Object back into a JSON String //
            var appendRequest = JsonConvert.SerializeObject(stock, Formatting.Indented);
            File.WriteAllText(command.getJsonDataDirectory(StoreName.Trim(), "/Stores/") + "_inventory.json", appendRequest);
            jsonCommand.updateQuantityOwner(command.getJsonDataDirectory("owners".Trim(), "/Stock/") + "_inventory.json", productName, Quantity);
        }
    }
    #endregion
}
