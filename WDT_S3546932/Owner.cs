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
        

        public override void updateQuantity(string fileName, string ProductName, int Quantity)
        {
            List<Stock> productList = JsonConvert.DeserializeObject<List<Stock>>(command.JsonReader(fileName));

            command.displayMessage("Updating: " + fileName);
            foreach (var product in productList)
            {

                if (product.ProductRequested == ProductName || product.ProductRequested == ProductName && product.Processed != true)
                {
                    command.displayMessage("Product Name Found");
                    if (product.CurrentStock >= Quantity)
                    {
                        command.displayMessage("Updating....");
                        Thread.Sleep(2000);
                        command.displayMessage("Current Stock: {0} " + product.CurrentStock);
                        product.CurrentStock = product.CurrentStock - Quantity;
                        command.displayMessage("New Current Stock: {0} " + product.CurrentStock);
                        command.displayMessage("Update Complete");
                        if (product.Processed == false) { product.Processed = true; }
                        break;
                    }
                    else
                    {
                        command.displayMessage("Cannot Update.");
                    }
                }
            }

            var updatedList = JsonConvert.SerializeObject(productList, Formatting.Indented);
            File.WriteAllText(fileName, updatedList);
            //Console.WriteLine(updatedList);
            
        }

        public override List<OwnerStock> displayAllProductLines()
        {
            List<OwnerStock> productList = null;
            try
            {
                Console.WriteLine("{0,5} {1,15} {2,15}", "ID", "Product Name", "Current Stock");

                productList = JsonConvert.DeserializeObject<List<OwnerStock>>(command.JsonReader("JsonData/owners_inventory.json"));
                foreach (var product in productList)
                {
                    Console.WriteLine("{0,5} {1,15} {2,15}", product.ID, product.ProductName, product.CurrentStock);
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

            productList = JsonConvert.DeserializeObject<List<Stock>>(command.JsonReader("JsonData/stockrequests.json"));

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

        public List<Stock> stockRequest(List<Stock> productList)
        {
                Console.WriteLine("{0,5} {1,10} {2,15} {3,10} {4,10} {5,15} {6,15}", "ID", "Store", "Product", "Quantity", "Current Stock", "Stock Availability", "Processed");
                productList = JsonConvert.DeserializeObject<List<Stock>>(command.JsonReader("JsonData/stockrequests.json"));
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
                                updateQuantity("JsonData/owners_inventory.json", ProductName, Quantity);
                                updateQuantity("JsonData/stockrequests.json", ProductName, Quantity);
                                if (request.StoreName == StoreName && request.Processed == false)
                                {
                                    updateQuantity("JsonData/" + StoreName + "_inventory.json", ProductName, Quantity);
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
    }
}