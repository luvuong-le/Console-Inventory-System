using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WDT_S3546932
{
    abstract public class UI
    {
        //Displays Title 
        abstract public void displayTitle (String title);

        //Displays a Message to the User
        abstract public void displayMessage (String message);

        //Displays an [ERROR] Message
        abstract public void displayError(String error);

        //Displays all the products in Owner Inventory 
        abstract public void displayAllProductLines();
    
        abstract public String JsonReader(String fileName);

        // Display All Stock Requests // 
        abstract public void displayAllStockRequests();

        abstract public void updateQuantity(string fileName, string ProductName, int Quantity);

    }
}
