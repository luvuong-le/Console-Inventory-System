using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WDT_S3546932
{

    interface ICommands
    {
        //Displays Title 
        string displayTitle(String title);

        //Displays a Message to the User
        string displayMessage(String message);

        string displayMessageOneLine(string message);

        //Displays an [ERROR] Message
        string displayError(String error);

        string getCurrentDirectory();

        string getJsonDataDirectory(string filename, string folder);

        String[] getStoreNames();

    }

    interface JCommands
    {
        void updateQuantityStockRequest(string fileName, string ProductName, int Quantity);

        void updateQuantityOwner(string fileName, string ProductName, int Quantity);

        void updateQuantityStore(string fileName, string ProductName, int Quantity);

        //Takes in a file name and reads through the json file //
        string JsonReader(String fileName);

        int lastRequestID(); 
    }

    abstract class OwnerCLI
    {

        //Displays all the products in Owner Inventory 
        abstract public List<OwnerStock> displayAllProductLines();

        // Display All Stock Requests // 
        abstract public List<Stock> displayAllStockRequests(List<Stock> StockList);

        abstract public List<Stock> displayAllStockRequestBool(List<Stock> StockList);

        abstract public int checkCurrentStock(string productName);

        //Updates the quantities During runtime and  saves to file //
       // abstract public void updateQuantity(string fileName, string ProductName, int Quantity);

    }

    abstract class CustomerCLI
    {
        abstract public List<StoreStock> displayProduct();

        abstract public void displayWorkShop();
    }

    abstract class FranchiseCLI
    {
        abstract public List<StoreStock> displayInventory(string StoreName);

        abstract public List<StoreStock> displayInventoryThres(int Threshold);

        abstract public void AddNewInventory(String StoreName);

        abstract public void requestForStock(String productName, String FileName, String StoreName); 
    }
}