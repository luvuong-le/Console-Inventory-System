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

    class Owner : OwnerCLI
    {
        Utility command = new Utility();

        JsonUtility jsonCommand = new JsonUtility();

        #region displayAllProductLines
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
        #endregion

        #region displayStockRequests
        public override List<Stock> displayAllStockRequests(List<Stock> productList)
        {
            displayAllStock(productList);
            stockRequest(productList);
            return productList;
        }
        #endregion

        #region displayStockRequestsBool
        public override List<Stock> displayAllStockRequestBool(List<Stock> productList)
        {
            command.displayMessageOneLine("Enter [True/False]: "); String choice = Console.ReadLine();

            productList = JsonConvert.DeserializeObject<List<Stock>>(jsonCommand.JsonReader(command.getJsonDataDirectory("stockrequests", "/Stock/") + ".json"));

            foreach (var request in productList)
            {
                if (choice.Equals("True", StringComparison.OrdinalIgnoreCase))
                {
                    displayOptionStock(productList, true);
                    stockRequest(productList);
                    break;
                }
                else if (choice.Equals("False", StringComparison.OrdinalIgnoreCase))
                {
                    displayOptionStock(productList, false);
                    stockRequest(productList);
                    break;
                }else
                {
                    command.displayError("Must Enter True or False"); break;
                }

            }
            return productList;
        }
        #endregion

        public List<Stock> displayAllStock(List<Stock> stockRequests)
        {
            if (stockRequests.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine("{0,15}", "[ALERT] No Stock Requests"); command.colourReset();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.White; Console.WriteLine("\n{0,15} {1,15} {2,25} {3,15} {4,15} {5,15} {6,15}", "ID", "Store", "Product Requested", "Quantity", "Current Stock", "Processed", "Stock Availability"); command.colourReset();
                foreach (var stock in stockRequests)
                {
                    Console.WriteLine("{0,15} {1,15} {2,25} {3,15} {4,15} {5,15} {6,15}", stock.ID, stock.StoreName, stock.ProductRequested, stock.Quantity, stock.CurrentStock, stock.Processed, stock.StockAvailability);
                }
            }
            return stockRequests;
        }

        public List<Stock> displayOptionStock(List<Stock> stockRequests, bool option)
        {
            if (stockRequests.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine("{0,15}", "[ALERT] No Stock Requests"); command.colourReset();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.White; Console.WriteLine("\n{0,15} {1,15} {2,25} {3,15} {4,15} {5,15} {6,15}", "ID", "Store", "Product Requested", "Quantity", "Current Stock", "Processed", "Stock Availability"); command.colourReset();
                foreach (var stock in stockRequests)
                {
                    if (option == true && stock.StockAvailability == true)
                    {
                        Console.WriteLine("{0,15} {1,15} {2,25} {3,15} {4,15} {5,15} {6,15}", stock.ID, stock.StoreName, stock.ProductRequested, stock.Quantity, stock.CurrentStock, stock.Processed, stock.StockAvailability);
                    }
                    else if (option == false && stock.StockAvailability == false)
                    {
                        Console.WriteLine("{0,15} {1,15} {2,25} {3,15} {4,15} {5,15} {6,15}", stock.ID, stock.StoreName, stock.ProductRequested, stock.Quantity, stock.CurrentStock, stock.Processed, stock.StockAvailability);
                    }
                }
            }
            return stockRequests;
        }
        #region StockRequest
        // Calls and Updates Details in StockRequest.json, Storename.json, OwnersInventory.json //
        public List<Stock> stockRequest(List<Stock> productList)
        {
                command.displayMessageOneLine("Enter Request To Process[ID]: "); String requestProcess = Console.ReadLine(); int requestID = command.convertInt(requestProcess);
            if (command.checkInt(requestProcess, requestID) == true)
            {
                foreach (var request in productList)
                {

                    if (request.ID == requestID)
                    {
                        command.displayMessage("Request Found");
                        if (request.StockAvailability == false && request.Quantity > request.CurrentStock && request.Processed == false)
                        {
                            command.displayError("Unavailable Stock to complete this request");
                            break;
                        }
                        else if (request.Processed == true) { command.displayError("Has already been processed"); }
                        else
                        {
                            command.displayMessage("Processing Request....");
                            String ProductName = request.ProductRequested;
                            int Quantity = request.Quantity;
                            String StoreName = request.StoreName;
                            if (request.Processed == false)
                            {
                                jsonCommand.updateQuantityOwner(command.getJsonDataDirectory("owners", "/Stock/") + "_inventory.json", ProductName, Quantity);
                                if (request.StoreName == StoreName && request.Processed == false)
                                {
                                    jsonCommand.updateQuantityStoreStockRequest(requestID , command.getJsonDataDirectory(StoreName, "/Stores/") + "_inventory.json", ProductName, Quantity, "add");
                                    request.Processed = true;
                                }
                            }
                            break;
                        }
                    }
                    else if (requestID > productList.Count) { command.displayError("No such ID"); break; }
                    else { if (request.ID != requestID) { continue; } }
                }
            }
            return productList;
        }
        #endregion


        #region checkCurrentStock
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
        #endregion
    }
}
