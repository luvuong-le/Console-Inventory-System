using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WDT_S3546932
{
    class Stock
    {
            public int ID { get; set; }

            public string StoreName { get; set; }

            public string ProductRequested { get; set; }

            public int Quantity { get; set; }

            public int CurrentStock { get; set; }

            public Boolean Processed { get; set; }

            public Boolean StockAvailability { get; set; }

            public Stock(int ID, string storeName, string productName, int Quantity, int CurrentStock, bool Processed, bool StockAvailability)
            {
                this.ID = ID;
                this.StoreName = storeName;
                this.ProductRequested = productName;
                this.Quantity = Quantity;
                this.CurrentStock = CurrentStock;
                this.Processed = Processed;
                this.StockAvailability = StockAvailability;
            }

    }

    class OwnerStock
    {
            public int ID { get; set; }

            public string ProductName { get; set; }

            public int CurrentStock { get; set; }

            public Boolean StockAvailability { get; set; }
    }

    class StoreStock
    {
            public int ID { get; set; }

            public string Store { get; set; }

            public string ProductName { get; set; }

            public int CurrentStock { get; set; }

            public Boolean ReStock { get; set; }
    }
}
