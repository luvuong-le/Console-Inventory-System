using Newtonsoft.Json;
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

        // Add New Inventory 
        public override void AddNewInventory()
        {
            throw new NotImplementedException();
        }

        //Display Inventory and Request For Stock in StockRequest.json //
        public override List<StoreStock> displayInventory(string StoreName)
        {
            command.displayMessageOneLine("Enter a number for Re-Stocking: "); string Thres = Console.ReadLine(); int thres;  Int32.TryParse(Thres, out thres); 
            String storeNameFile = command.getJsonDataDirectory(StoreName) + "_inventory.json";

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
                        requestForStock(product.ProductName, command.getJsonDataDirectory("stockrequests") + ".json", StoreName);
                        break;
                    }
                    else
                    {
                        command.displayError("Cannot make a request");
                        command.displayMessage("You have enough stock, Would you like to Continue [Yes/No]"); string choice = Console.ReadLine();
                        if (choice == "yes" || choice == "Yes")
                        {
                            requestForStock(product.ProductName, command.getJsonDataDirectory("stockrequests") + ".json", StoreName);
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
        public override void requestForStock(string productName, string FileName, string StoreName)
        { 
            command.displayMessage("Requesting Stock");

            //Creating a new Local List of type Stock//
            List<Stock> stock = new List<Stock>();

            //Reading through the stockrequests.json file and with that jsonData, converting the json data into Object List<Stock> //
            using (var streamReader = new StreamReader(command.getJsonDataDirectory("stockrequests") + ".json"))
            {
                var jsonData = streamReader.ReadToEnd(); stock = JsonConvert.DeserializeObject<List<Stock>>(jsonData);
                //Console.WriteLine(jsonData);
            }
            
            //Creating a local list of stockRequest to allow access to the Stock Class Variables //
            List<Stock> stockRequests = JsonConvert.DeserializeObject<List<Stock>>(jsonCommand.JsonReader(command.getJsonDataDirectory(StoreName) + "_inventory.json"));

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
            File.WriteAllText(command.getJsonDataDirectory("stockrequests") + ".json", appendRequest);
            Console.WriteLine(appendRequest);
        }
    }
}
