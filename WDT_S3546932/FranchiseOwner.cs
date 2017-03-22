﻿using Newtonsoft.Json;
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

        //Add New Inventory 
        public override void AddNewInventory(string storeName)
        {
            //Display List of Products in OwnerInventory that is not Currently in Store File //

            List<OwnerStock> OwnerProducts = JsonConvert.DeserializeObject<List<OwnerStock>>(jsonCommand.JsonReader(command.getJsonDataDirectory("owners", "/Stock/") + "_inventory.json"));
            List<StoreStock> storeStock = JsonConvert.DeserializeObject<List<StoreStock>>(jsonCommand.JsonReader(command.getJsonDataDirectory(storeName, "/Stores/") + "_inventory.json"));

            //Display Owner Stock // 
            command.displayMessage("Owner Inventory: ");

            Console.WriteLine("{0,5} {1,15} {2,15}", "ID", "Product Name", "Current Stock");

            foreach (var product in OwnerProducts)
            {
                Console.WriteLine("{0,5} {1,15} {2,15}", product.ID, product.ProductName, product.CurrentStock);
            }

            command.displayMessage("Your Store Inventory: ");
            //Display Store Iventory
            Console.WriteLine("{0,5} {1,15} {2,15}", "ID", "Product Name", "Current Stock");

            foreach (var store in storeStock)
            {
                Console.WriteLine("{0,5} {1,15} {2,15}", store.ID, store.ProductName, store.CurrentStock);
            }

            int ownerCount=  OwnerProducts.Count; int storeCount = storeStock.Count;

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

                foreach (var owner in OwnerProducts)
                {
                    ownerList.Add(owner.ID);
                }
                //For each ID Array returned compare the two arrays//

                List<int> difference = ownerList.Except(storeList).ToList(); //Compare the OwnerInventory to the Store and determine which ID they do not have //

                // Go through the Owner Stock and make  REQUEST TO STOCK and update to the store file and add that item in ///

                foreach (var product in OwnerProducts)
                {
                    foreach (var id in difference)
                    {
                        if (product.ID == id)
                        {
                            Console.WriteLine("{0,5} {1,15} {2,15}", product.ID, product.ProductName, product.CurrentStock);
                        }
                    }
                }

                command.displayMessageOneLine("\nEnter Item ID Number to Purchase or Function: "); string ID = Console.ReadLine(); int itemID; Int32.TryParse(ID, out itemID);

                //Prompt User to Choose an ID Number and Request it Based on the List //
                foreach (var test in difference) {
                    foreach (var product in OwnerProducts)
                    {
                        if (itemID != test)
                        {
                            command.displayError("Please choose an ID from the List"); return;
                        }
                        else if (itemID == test && product.ID == itemID) {
                            command.displayMessageOneLine("\nPlease Enter the Amount you would like to Purchase Also: "); string quant = Console.ReadLine(); int Quantity; Int32.TryParse(quant, out Quantity);

                            command.displayMessageOneLine("Youve chosen the Product");

                            Console.WriteLine("\n{0,0} {1,15} {2,15}", product.ID, product.ProductName, "Quantity: " + Quantity);

                            command.displayMessage("Would you like to Continue [Yes/No]"); string choice = Console.ReadLine();
                            if (choice == "yes" || choice == "Yes")
                            {
                                //Process Add Inventory //
                                AddProduct(product.ProductName, storeName, Quantity);

                            }
                            else if (choice == "No" || choice == "no") { command.displayMessage("Ok. Returning to Menu"); }
                            return;
                        }
                    }
                }
            }
        }

        //Display Inventory and Request For Stock in StockRequest.json //
        public override List<StoreStock> displayInventory(string StoreName)
        {
            command.displayMessageOneLine("Enter a Threshold for Re-Stocking: "); string Thres = Console.ReadLine(); int thres;  Int32.TryParse(Thres, out thres); 
            String storeNameFile = command.getJsonDataDirectory(StoreName, "/Stores/") + "_inventory.json";

            List<StoreStock> productList = JsonConvert.DeserializeObject<List<StoreStock>>(jsonCommand.JsonReader(storeNameFile));
           
            Console.WriteLine("{0,5} {1,15} {2,15} {3,15}", "ID", "Product Name", "Current Stock", "Re-Stock");

            foreach (var product in productList)
            {
                Console.WriteLine("{0,5} {1,15} {2,15} {3,15}", product.ID, product.ProductName, product.CurrentStock, product.ReStock);
            }

            command.displayMessageOneLine("\nEnter Request To Process[ID]: "); string requestProcess = Console.ReadLine();
            int requestID; Int32.TryParse(requestProcess, out requestID);  command.displayMessage("[Request number]: " + requestID);

            foreach (var product in productList)
            {
                if (product.ID == requestID)
                {
                    if (product.ReStock == true && product.CurrentStock >= thres)
                    {
                        requestForStock(product.ProductName, StoreName);
                        break;
                    }
                    else
                    {
                        command.displayError("Cannot make a request");
                        command.displayMessage("You have enough stock, Would you like to Continue [Yes/No]"); string choice = Console.ReadLine();
                        if (choice == "yes" || choice == "Yes")
                        {
                            requestForStock(product.ProductName, StoreName);
                        } else if (choice == "No" || choice == "no") { command.displayMessage("Ok. Returning to Menu"); }
                        break;
                    }
                }
            }

            return productList;
        }

        // Display Inventory Based on Threshold
        public override List<StoreStock> displayInventoryThres(int Threshold)
        {
            throw new NotImplementedException();
        }

        //Request For Stock, Appends to the StockRequest.json File//
        public override void requestForStock(string productName, string StoreName)
        { 
            command.displayMessage("Requesting Stock");

            //Creating a new Local List of type Stock//
            List<Stock> stock = new List<Stock>();

            //Reading through the stockrequests.json file and with that jsonData, converting the json data into Object List<Stock> //
            using (var streamReader = new StreamReader(command.getJsonDataDirectory("stockrequests", "/Stock/") + ".json"))
            {
                var jsonData = streamReader.ReadToEnd(); stock = JsonConvert.DeserializeObject<List<Stock>>(jsonData);
                //Console.WriteLine(jsonData);
            }
            
            //Creating a local list of stockRequest to allow access to the Stock Class Variables //
            List<Stock> stockRequests = JsonConvert.DeserializeObject<List<Stock>>(jsonCommand.JsonReader(command.getJsonDataDirectory(StoreName, "/Stores/") + "_inventory.json"));

            //Looping through the Stock Class//
            foreach (var storeProduct in stockRequests)
            {
                if (productName == storeProduct.ProductRequested)
                {
                    if (storeProduct.CurrentStock > owner.checkCurrentStock(storeProduct.ProductRequested)) { storeProduct.StockAvailability = false; } else { storeProduct.StockAvailability = true; }

                    //Adding a new Object using the already available stock object //
                    stock.Add(new Stock(jsonCommand.lastRequestID() + 1, StoreName, storeProduct.ProductRequested, storeProduct.CurrentStock, owner.checkCurrentStock(productName), false, storeProduct.StockAvailability));
                }
            }

            //Append the Results and convert the Object back into a JSON String //
            var appendRequest = JsonConvert.SerializeObject(stock, Formatting.Indented);
            File.WriteAllText(command.getJsonDataDirectory("stockrequests", "/Stock/") + ".json", appendRequest);
            Console.WriteLine(appendRequest);
        }

        public override void AddProduct(String productName, String StoreName, int Quantity)
        {
            //Creating a new Local List of type Stock//
            List<StoreStock> stock = new List<StoreStock>();

            //Reading through the stockrequests.json file and with that jsonData, converting the json data into Object List<Stock> //
            using (var streamReader = new StreamReader(command.getJsonDataDirectory(StoreName, "/Stores/") + "_inventory.json"))
            {
                var jsonData = streamReader.ReadToEnd(); stock = JsonConvert.DeserializeObject<List<StoreStock>>(jsonData);
                //Console.WriteLine(jsonData);
            }

            List<OwnerStock> ownerInventory = JsonConvert.DeserializeObject<List<OwnerStock>>(jsonCommand.JsonReader(command.getJsonDataDirectory("owners", "/Stock/") + "_inventory.json"));

            //Looping through the Stock Class//
            foreach (var product in ownerInventory)
            {
                if (productName == product.ProductName)
                {
                    //Adding a new Object using the already available stock object //
                    stock.Add(new StoreStock(product.ID, StoreName, productName, product.CurrentStock = Quantity, false));
                }
            }

            //Append the Results and convert the Object back into a JSON String //
            var appendRequest = JsonConvert.SerializeObject(stock, Formatting.Indented);
            File.WriteAllText(command.getJsonDataDirectory(StoreName, "/Stores/") + "_inventory.json", appendRequest);
            Console.WriteLine(appendRequest);
        }
    }
}
