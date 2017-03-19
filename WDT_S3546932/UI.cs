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

        //Displays an [ERROR] Message
        string displayError(String error);

        //Takes in a file name and reads through the json file //
        string JsonReader(String fileName);
    }

    abstract class OwnerCLI
    {

        //Displays all the products in Owner Inventory 
        abstract public List<OwnerStock> displayAllProductLines();

        // Display All Stock Requests // 
        abstract public List<Stock> displayAllStockRequests(List<Stock> StockList);

        abstract public List<Stock> displayAllStockRequestBool(List<Stock> StockList);

        //Updates the quantities During runtime and  saves to file //
        abstract public void updateQuantity(string fileName, string ProductName, int Quantity);

    }

    abstract class CustomerCLI
    {

    }

    abstract class FranchiseCLI
    {
        abstract public void displayInventory(string StoreName);

        abstract public void displayInventoryThres(int Threshold);

        abstract public void AddNewInventory();

    }
}

//PolyMorphism this
//Create classes for each menu methods 
 /*
 * For adding new inventory, create the JSON OBJECTs based ont he JsonObjects Class 
 * Json deserialize and serialize
 * 
 */