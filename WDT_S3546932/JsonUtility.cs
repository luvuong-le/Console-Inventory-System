using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WDT_S3546932
{
    class JsonUtility : JCommands
    {
        Utility command = new Utility();

        public List<StoreStock> getStoreData(string storeName) { List<StoreStock> stores = JsonConvert.DeserializeObject<List<StoreStock>>(JsonReader(command.getJsonDataDirectory(storeName.Trim(), "/Stores/") + "_inventory.json")); return stores; }

        public List<OwnerStock> getOwnerFile() { List<OwnerStock> owner =  JsonConvert.DeserializeObject<List<OwnerStock>>(JsonReader(command.getJsonDataDirectory("owners".Trim(), "/Stock/") + "_inventory.json")); return owner; }

        public List<Stock> getStockRequestData() { List<Stock> stockRequest = JsonConvert.DeserializeObject<List<Stock>>(JsonReader(command.getJsonDataDirectory("stockrequests".Trim(), "/Stock/") + ".json")); return stockRequest; }

        public List<Workshop> getBookings(string storeName) { List<Workshop> bookings = JsonConvert.DeserializeObject<List<Workshop>>(JsonReader(command.getJsonDataDirectory(storeName.Trim(), "/Workshops/") + "_bookings.json")); return bookings; }

        public List<WorkshopTimes> getWorkShopTimes(string storeName) { List<WorkshopTimes> workshopTimes = JsonConvert.DeserializeObject<List<WorkshopTimes>>(JsonReader(command.getJsonDataDirectory(storeName.Trim(), "/Workshops/") + "_workshopTimes.json")); return workshopTimes; }

        public string JsonReader(string fileName)
        {
            String json = " ";

            try
            {
                StreamReader r = new StreamReader(fileName);

                json = r.ReadToEnd();

                r.Close();
            }
            catch (FileNotFoundException e)
            {
                command.displayMessage(e.Message);
                //If File is not Found return to the Main Menu
                Menu.OwnerMenu Omenu = new Menu.OwnerMenu(); Omenu.displayMenu();
            }
            return json;
        }

        public List<int> returnAllIDs(String storeName)
        {
            List<StoreStock> storeProducts = JsonConvert.DeserializeObject<List<StoreStock>>(JsonReader(command.getJsonDataDirectory(storeName.Trim(), "/Stores/") + "_inventory.json"));
            List<OwnerStock> ownerProducts = JsonConvert.DeserializeObject<List<OwnerStock>>(JsonReader(command.getJsonDataDirectory("owners".Trim(), "/Stock/") + ".json"));

            List<int> storeList = new List<int>();
            foreach (var store in storeProducts)
            {
                storeList.Add(store.ID);
            }
            return storeList;
        }

        public int lastRequestID()
        {
            List<Stock> productList = JsonConvert.DeserializeObject<List<Stock>>(JsonReader(command.getJsonDataDirectory("stockrequests".Trim(), "/Stock/") + ".json"));
            int ID = 0;
            foreach (var reqID in productList)
            {
                //Find the last ID Number 
                ID = reqID.ID;
            }
            return ID;
        }

        public bool matchID(string storename, int ItemID)
        {
            List<StoreStock> productList = JsonConvert.DeserializeObject<List<StoreStock>>(JsonReader(command.getJsonDataDirectory(storename.Trim(), "/Stores/") + "_inventory.json"));

            if (productList.Any(item => item.ID == ItemID))
            {
                foreach (var reqID in productList)
                {
                    if (reqID.ID == ItemID)
                    {
                        command.displayMessageOneLine("\n[SUCCESS] Found Match: " + reqID.ProductName + "\n");
                        return true;
                    }
                    else if (reqID.ID != ItemID)
                    {
                        continue;
                    }
                }
            }
            else
            {
                command.displayError("No Such ID");
            }
            return false;
        }

      
        public void updateQuantityOwner(string fileName, string ProductName, int Quantity)
        {
            List<OwnerStock> productList = JsonConvert.DeserializeObject<List<OwnerStock>>(JsonReader(fileName));
            foreach (var product in productList)
            {
                if (product.ProductName == ProductName)
                {
                    if (product.CurrentStock >= Quantity)
                    {
                        Thread.Sleep(2000);
                        product.CurrentStock = product.CurrentStock - Quantity;
                        if (product.CurrentStock == 0) { product.StockAvailability = false; }
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
        }

        public void updateQuantityStoreStockRequest(int requestID, string fileName, string ProductName, int Quantity, string addSubtract)
        {
            List<StoreStock> productList = JsonConvert.DeserializeObject<List<StoreStock>>(JsonReader(fileName));
            List<Stock> stockrequests = JsonConvert.DeserializeObject<List<Stock>>(JsonReader(command.getJsonDataDirectory("stockrequests".Trim(), "/Stock/") + ".json"));

            if (productList.Any(item => item.ProductName == ProductName))
            {
                foreach (var product in productList)
                {
                    if (product.ProductName == ProductName)
                    {
                        Thread.Sleep(2000);
                        if (addSubtract == "minus") { product.CurrentStock = product.CurrentStock - Quantity; } else if (addSubtract == "add") { product.CurrentStock = product.CurrentStock + Quantity; }
                        foreach (var request in stockrequests) { if (request.ID == requestID && request.Processed == false) { request.Processed = true; } else { continue; } }
                    }
                }
            }else { command.displayError("No Product With that Name"); }



            var updatedList = JsonConvert.SerializeObject(productList, Formatting.Indented);
            File.WriteAllText(fileName, updatedList);
            var updatedStockRequests = JsonConvert.SerializeObject(stockrequests, Formatting.Indented);
            File.WriteAllText(command.getJsonDataDirectory("stockrequests".Trim(), "/Stock/") + ".json", updatedStockRequests);
      }

        public List<StoreStock> updateQuantityStore(string fileName, string ProductName, int Quantity, string addSubtract)
        {
            List<StoreStock> productList = JsonConvert.DeserializeObject<List<StoreStock>>(JsonReader(fileName));
            foreach (var product in productList)
            {

                if (product.ProductName == ProductName || product.ProductName == ProductName && product.ReStock != true)
                {
                    if (product.CurrentStock >= Quantity)
                    {
                        Thread.Sleep(2000);
                        if (addSubtract == "minus") { product.CurrentStock = product.CurrentStock - Quantity; } else if (addSubtract == "add") { product.CurrentStock = product.CurrentStock + Quantity; }
                        if (product.ReStock == false && product.CurrentStock == 0) { product.ReStock = true; }
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
            return productList;
        }
    }
}
