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
        string displayTitle(string title);

        //Displays a Message to the User
        string displayMessage(string message);

        string displayMessageOneLine(string message);

        //Displays an [ERROR] Message
        string displayError(string error);

        string getCurrentDirectory();

        string getJsonDataDirectory(string filename, string folder);

        string[] getFileNames(string folderName);

        bool checkStoreName(string storeName, string[] filenames);

        void printAllStoreNames(string[] filenames);

        bool checkInt(string request, int Quantity);

        string generateBookingReference(int size);

    }

    interface JCommands
    {
        List<Stock> getStockRequestData();

        List<OwnerStock> getOwnerFile();

        List<StoreStock> getStoreData(string storeName);

        List<Workshop> getBookings(string storeName);

        List<WorkshopTimes> getWorkShopTimes(string storeName);

        void updateQuantityOwner(string fileName, string ProductName, int Quantity);

        void updateQuantityStore(string fileName, string ProductName, int Quantity, string addSubtract);

        void updateQuantityStoreStockRequest(int requestID, string fileName, string ProductName, int Quantity, string addSubtract);

        //Takes in a file name and reads through the json file //
        string JsonReader(string fileName);

        bool matchID(string storename, int ItemID);

        int lastRequestID();

        List<int> returnAllIDs(string storeName);
    }

    interface OwnerCLI
    {

        //Displays all the products in Owner Inventory 
        List<OwnerStock> displayAllProductLines(List<OwnerStock> OwnerInventory);

        // Display All Stock Requests // 
        List<Stock> displayAllStockRequests(List<Stock> StockList);

        List<Stock> displayAllStockRequestBool(List<Stock> StockList);

        int checkCurrentStock(string productName);
    }

    interface CustomerCLI
    {
        List<StoreStock> displayProduct(string StoreName, List<StoreStock> storeStock);

        void displayWorkShop(string storeName);

        void purchaseProduct(string productName, string StoreName, int Quantity, List<StoreStock> storeStock);

        void addProduct(List<customerPurchase> purchasedProducts, List<StoreStock> storeStock, String productName, String StoreName, int Quantity);

        void bookWorkshop(List<WorkshopTimes> workshopTimes, string storeName);

        List<WorkshopTimes> updateBookingDetails(List<WorkshopTimes> workshopTimes, int ID, string storename, string name, string bookingRef, string time, string session);

        List<StoreStock> searchByProduct(List<StoreStock> storeStock, string ProductName);

        List<customerPurchase> printReciept(List<customerPurchase> products, List<StoreStock> storeStock, int bookedTotal, string storeName);


    }

    interface FranchiseCLI
    {
        List<StoreStock> displayInventory(string StoreName, List<StoreStock> storeStock);

        List<StoreStock> displayInventoryThres(string StoreName, List<StoreStock> storeStock);

        void AddNewInventory(string StoreName, List<OwnerStock> OwnerInventory, List<StoreStock> storeStock);

        List<Stock> requestForStock(string productName, string StoreName, int Quantity);

        void AddProduct(string productName, string StoreName, int Quantity);

    }
}