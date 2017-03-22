using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WDT_S3546932
{

    class Owner : OwnerCLI
    {
        Utility command = new Utility();

        JsonUtility jsonCommand = new JsonUtility();

        public override List<OwnerStock> displayAllProductLines()
        {
            List<OwnerStock> productList = null;
            try
            {
                Console.WriteLine("{0,10} {1,25} {2,25}", "ID", "Product Name", "Current Stock");

                productList = JsonConvert.DeserializeObject<List<OwnerStock>>(jsonCommand.JsonReader(command.getJsonDataDirectory("owners", "/Stock/") + "_inventory.json"));
                foreach (var product in productList)
                {
                    Console.WriteLine("{0,10} {1,25} {2,25}", product.ID, product.ProductName, product.CurrentStock);
                }
            } catch(Exception){
                command.displayError("");
            }
            return productList;
        }

        public override List<Stock> displayAllStockRequests(List<Stock> productList)
        {
            stockRequest(productList);
            return productList;
        }

        public override List<Stock> displayAllStockRequestBool(List<Stock> productList)
        {
            command.displayMessage("Enter [True/False]: "); String choice = Console.ReadLine();

            productList = JsonConvert.DeserializeObject<List<Stock>>(jsonCommand.JsonReader(command.getJsonDataDirectory("stockrequests", "/Stock/") + ".json"));

            foreach (var request in productList)
            {
                if (choice == "True" || choice == "true" || choice == "TRUE")
                {
                    if (request.StockAvailability == true)
                    {
                        stockRequest(productList);
                        break;
                    }
                }
                else if (choice == "False" || choice == "false" || choice == "FALSE")
                {
                    if (request.StockAvailability == false)
                    {
                        stockRequest(productList);
                        break;
                    }
                }

            }
            return productList;
        }

        // Calls and Updates Details in StockRequest.json, Storename.json, OwnersInventory.json //
        public List<Stock> stockRequest(List<Stock> productList)
        {
                Console.WriteLine("{0,5} {1,10} {2,15} {3,10} {4,10} {5,15} {6,15}", "ID", "Store", "Product", "Quantity", "Current Stock", "Stock Availability", "Processed");
                productList = JsonConvert.DeserializeObject<List<Stock>>(jsonCommand.JsonReader(command.getJsonDataDirectory("stockrequests", "/Stock/") + ".json"));
                foreach (var request in productList)
                {
                        Console.WriteLine("{0,5} {1,10} {2,15} {3,10} {4,10} {5,15} {6,20}",
                        request.ID, request.StoreName, request.ProductRequested, request.Quantity, request.CurrentStock, request.StockAvailability, request.Processed);
                }

                command.displayMessage("Enter Request To Process[ID]: ");
                String requestProcess = Console.ReadLine();
                int requestID;
                Int32.TryParse(requestProcess, out requestID);
                command.displayMessage("Request number: " + requestID);

                foreach (var request in productList)
                {

                    if (request.ID == requestID)
                    {
                        command.displayMessage("Request Found");
                        if (request.StockAvailability == false || request.Quantity > request.CurrentStock || request.Processed == true)
                        {
                            command.displayMessage("Unavailable Stock to complete this request");
                            break;
                        }
                        else
                        {
                            command.displayMessage("Processing Request....");
                            String ProductName = request.ProductRequested;
                            int Quantity = request.Quantity;
                            String StoreName = request.StoreName;
                            if (request.Processed == false)
                            {
                            jsonCommand.updateQuantityOwner(command.getJsonDataDirectory("owners", "/Stock/") + "_inventory.json", ProductName, Quantity);
                            jsonCommand.updateQuantityStockRequest(command.getJsonDataDirectory("stockrequests", "/Stock/") + ".json", ProductName, Quantity);
                                if (request.StoreName == StoreName && request.Processed == false)
                                {
                                jsonCommand.updateQuantityStore(command.getJsonDataDirectory(StoreName, "/Stores/") + "_inventory.json", ProductName, Quantity);
                                }
                            }
                            break;
                        }
                    }
                    else if (requestID > productList.Count) { command.displayError("No such ID"); break; }
                    else { if (request.ID != requestID) { continue; } }
                }
            return productList;
        }

        public override int checkCurrentStock(string productName)
        {
            List<OwnerStock> ownerStock = JsonConvert.DeserializeObject<List<OwnerStock>>(jsonCommand.JsonReader(command.getJsonDataDirectory("owners", "/Stock/") + "_inventory.json"));

            int CurrentStock = 0;

            foreach (var stock in ownerStock)
            {
                if(productName == stock.ProductName)
                {
                    CurrentStock = stock.CurrentStock;
                }
            }

            return CurrentStock;
        }
    }
}
