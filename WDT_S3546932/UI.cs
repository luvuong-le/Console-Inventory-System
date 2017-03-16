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
        void displayTitle(String title);

        //Displays a Message to the User
        void displayMessage(String message);

        //Displays an [ERROR] Message
        void displayError(String error);

        //Takes in a file name and reads through the json file //
        String JsonReader(String fileName);
    }

    abstract class OwnerCLI
    {

        //Displays all the products in Owner Inventory 
        abstract public void displayAllProductLines();

        // Display All Stock Requests // 
        abstract public void displayAllStockRequests();

        abstract public void displayAllStockRequestBool();

        //Updates the quantities During runtime and  saves to file //
        abstract public void updateQuantity(string fileName, string ProductName, int Quantity);

    }

    abstract class CustomerCLI
    {

    }

    abstract class FranchiseCLI
    {

    }
}

//PolyMorphism this
//Create classes for each menu methods 
 /*
 * For adding new inventory, create the JSON OBJECTs based ont he JsonObjects Class 
 * Json deserialize and serialize
 * 
 */