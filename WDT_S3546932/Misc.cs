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
    class Misc : UI
    {
        public override void displayTitle(String title) { Console.WriteLine(title); Console.WriteLine("---------------------------"); }

        public override void displayMessage(String message) { Console.WriteLine("\n" + message + "\n"); }

        public override void displayError(String error) { Console.WriteLine("\n [ERRPR] " + error + "\n"); }

        public override String JsonReader(String fileName)
        {
            StreamReader r = new StreamReader(fileName);    

            string json = r.ReadToEnd();

            r.Close();

            return json;
        }

        public override void updateQuantity(string fileName, string ProductName, int Quantity)
        {
            dynamic productList = JsonConvert.DeserializeObject(JsonReader(fileName));

            displayMessage("Updating: " + fileName);
            foreach (var product in productList)
            {

                if (product.ProductName == ProductName || product.ProductRequested == ProductName && product.Processed != true)
                {
                    displayMessage("Product Name Found");
                    if (product.CurrentStock >= Quantity)
                    {
                        displayMessage("Current Stock: " + product.CurrentStock);
                        product.CurrentStock = product.CurrentStock - Quantity;
                        String productStock = product.CurrentStock;
                        Console.WriteLine("New Current Stock: " + product.CurrentStock);
                        Console.WriteLine("Update Complete");
                        if (product.Processed == false) { product.Processed = true; }
                        break;
                    }else
                    {
                        displayMessage("Cannot Update.");
                    }
                }
         
              
            }

            var updatedList = JsonConvert.SerializeObject(productList, Formatting.Indented);
            System.IO.File.WriteAllText(fileName, updatedList);
            //Console.WriteLine(updatedList);
    
        }

        public override void displayAllProductLines()
        {

            Console.WriteLine("{0,5} {1,10} {2,15}", "ID", "Name", "StockLevel");
            displayMessage("--------------------------------------------------");

            dynamic productList = JsonConvert.DeserializeObject(JsonReader("owners_inventory.json"));

            foreach (var product in productList)
            {
                Console.WriteLine("{0,5} {1,10} {2,15}", product.ID, product.ProductName, product.CurrentStock);
            }
        }

        public override void displayAllStockRequests()
        {
            Console.WriteLine("{0,5} {1,10} {2,15} {3,10} {4,10} {5,15}", "ID", "Store", "Product", "Quantity", "Current Stock", "Stock Availability");

            dynamic requests = JsonConvert.DeserializeObject(JsonReader("stockrequests.json"));

            foreach (var request in requests)
            {
                Console.WriteLine("{0,5} {1,10} {2,15} {3,10} {4,10} {5,15}",
                    request.ID, request.StoreName, request.ProductRequested, request.Quantity, request.CurrentStock, request.StockAvailability);
            }

            displayMessage("Enter Request To Process[ID]: ");
            String requestProcess = Console.ReadLine();
            int requestID;
            Int32.TryParse(requestProcess, out requestID);
            displayMessage("Request number: " + requestID);

            foreach (var request in requests)
            {
                if (request.ID == requestID)
                {
                    displayMessage("Request Found");
                    if (request.stockAvailabilty == false || request.Quantity > request.CurrentStock)
                    {
                        displayMessage("Unavailable Stock to complete this request");
                    }
                    else
                    {
                        displayMessage("Processing Request....");
                        String ProductName = request.ProductRequested;
                        int Quantity = request.Quantity;
                        String StoreName = request.StoreName;
                        if(request.Processed == false)
                        {
                            updateQuantity("owners_inventory.json", ProductName, Quantity);
                            updateQuantity("stockrequests.json", ProductName, Quantity);
                            if (request.StoreName == StoreName)
                            {
                                updateQuantity(StoreName + "_inventory.json", ProductName, Quantity);
                            }
                        }
                        // Go into file of Owner Inventory and Corresponding Store File and Update the quantity of the request
                    
                       
                        break;
                    }
                }else { if (request.ID != requestID) { continue; } }  
            }
            
        }
    }
}
