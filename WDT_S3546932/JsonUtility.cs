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
            List<StoreStock> storeProducts = JsonConvert.DeserializeObject<List<StoreStock>>(JsonReader(command.getJsonDataDirectory(storeName, "/Stores/") + "_inventory.json"));
            List<OwnerStock> ownerProducts = JsonConvert.DeserializeObject<List<OwnerStock>>(JsonReader(command.getJsonDataDirectory("owners", "/Stock/") + ".json"));

            List<int> storeList = new List<int>();
            foreach(var store in storeProducts)
            {
                storeList.Add(store.ID);
            }
            return storeList;
        }

        public int lastRequestID()
        {
            List<Stock> productList = JsonConvert.DeserializeObject<List<Stock>>(JsonReader(command.getJsonDataDirectory("stockrequests" ,"/Stock/") + ".json"));
            int ID = 0;
            foreach(var reqID in productList)
            {
                //Find the last ID Number 
                ID = reqID.ID;
            }
            return ID;
        }

        public bool matchID(string storename, int ItemID)
        {
            List<StoreStock> productList = JsonConvert.DeserializeObject<List<StoreStock>>(JsonReader(command.getJsonDataDirectory(storename, "/Stores/") + "_inventory.json"));

            foreach (var reqID in productList)
            {
                if (reqID.ID == ItemID && ItemID <= productList.Count)
                {
                    command.displayMessage("Found Match");
                    return true;
                }
                else if(ItemID > productList.Count)
                {
                    command.displayMessage("There is no entry with that ID. Please Enter a Valid ID");
                    return false;
                }
              
            }
            return false;
        }

        public void updateQuantityStockRequest(string fileName, string ProductName, int Quantity)
        {
            List<Stock> productList = JsonConvert.DeserializeObject<List<Stock>>(JsonReader(fileName));

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
        public void updateQuantityOwner(string fileName, string ProductName, int Quantity)
        {
            List<OwnerStock> productList = JsonConvert.DeserializeObject<List<OwnerStock>>(JsonReader(fileName));

            command.displayMessage("Updating: " + fileName);
            foreach (var product in productList)
            {

                if (product.ProductName == ProductName)
                {
                    command.displayMessage("Product Name Found");
                    if (product.CurrentStock >= Quantity)
                    {
                        command.displayMessage("Updating....");
                        Thread.Sleep(2000);
                        command.displayMessage("Current Stock: " + product.CurrentStock);
                        product.CurrentStock = product.CurrentStock - Quantity;
                        command.displayMessage("New Current Stock: " + product.CurrentStock);
                        command.displayMessage("Update Complete");
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

        public void updateQuantityStore(string fileName, string ProductName, int Quantity)
        {
            List<StoreStock> productList = JsonConvert.DeserializeObject<List<StoreStock>>(JsonReader(fileName));

            command.displayMessage("Updating: " + fileName);
            foreach (var product in productList)
            {

                if (product.ProductName == ProductName || product.ProductName == ProductName && product.ReStock != true)
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
                        if (product.ReStock == false) { product.ReStock = true; }
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
    }
}
