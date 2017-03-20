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
            command.displayMessage(StoreName);
            command.displayMessageOneLine("Enter a number for Re-Stocking: "); string Thres = Console.ReadLine(); int thres;  Int32.TryParse(Thres, out thres); 
            String storeNameFile = "JsonData/" + StoreName + "_inventory.json";

            List<StoreStock> productList = JsonConvert.DeserializeObject<List<StoreStock>>(jsonCommand.JsonReader(storeNameFile));

            Console.WriteLine("{0,5} {1,15} {2,15} {3,15}", "ID", "Product Name", "Current Stock", "Re-Stock");

            foreach (var product in productList)
            {
                Console.WriteLine("{0,5} {1,15} {2,15} {3,15}", product.ID, product.ProductName, product.CurrentStock, product.ReStock);
            }

            command.displayMessage("Enter Request To Process[ID]: ");
            String requestProcess = Console.ReadLine();
            int requestID;
            Int32.TryParse(requestProcess, out requestID);
            command.displayMessage("Request number: " + requestID);

            foreach (var product in productList)
            {
                if(product.ReStock == true && product.CurrentStock >= thres)
                {
                    requestForStock(product.ProductName, "stockrequest.json", storeNameFile);
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
            List<StoreStock> storeProducts = JsonConvert.DeserializeObject<List<StoreStock>>(jsonCommand.JsonReader(StoreName));

            List<Stock> stockRequests = JsonConvert.DeserializeObject<List<Stock>>(jsonCommand.JsonReader(StoreName));

            foreach (var storeProduct in storeProducts)
            {
               if (productName == storeProduct.ProductName && storeProduct.ReStock == true)
               {
                    //Append to StockRequest File 
                    stockRequests.Add(new Stock(3, StoreName, storeProduct.ProductName, storeProduct.CurrentStock, storeProduct.CurrentStock, false, true));
                }
                
            }
            var appendRequest = JsonConvert.SerializeObject(stockRequests, Formatting.Indented);
            File.WriteAllText("stockrequests,json", appendRequest);
            Console.WriteLine(appendRequest);
        }
    }
}
