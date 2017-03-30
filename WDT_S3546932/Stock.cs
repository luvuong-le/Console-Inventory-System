using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WDT_S3546932
{
    #region Stock
    class Stock
    {
            public int ID { get; set; }

            public string StoreName { get; set; }

            public string ProductRequested { get; set; }

            public int Quantity { get; set; }

            public int CurrentStock { get; set; }

            public Boolean Processed { get; set; }

            public Boolean StockAvailability { get; set; }

            public Stock(int ID, string StoreName, string productName, int Quantity, int CurrentStock, bool Processed, bool StockAvailability)
            {
                this.ID = ID;
                this.StoreName = StoreName;
                this.ProductRequested = productName;
                this.Quantity = Quantity;
                this.CurrentStock = CurrentStock;
                this.Processed = Processed;
                this.StockAvailability = StockAvailability;
            }
      
    }
    #endregion

    #region OwnerStock
    class OwnerStock
    {
            public int ID { get; set; }

            public string ProductName { get; set; }

            public int CurrentStock { get; set; }

            public Boolean StockAvailability { get; set; }

            public double Cost { get; set; }
    }
    #endregion

    #region StoreStock
    class StoreStock
    {
            public int ID { get; set; }

            public string Store { get; set; }

            public string ProductName { get; set; }

            public int CurrentStock { get; set; }

            public Boolean ReStock { get; set; }

            public double Cost { get; set; }

            public StoreStock(int ID, string Store, string ProductName, int CurrentStock, Boolean ReStock, double Cost)
            {
                this.ID = ID;
                this.Store = Store;
                this.ProductName = ProductName;
                this.CurrentStock = CurrentStock;
                this.ReStock = ReStock;
                this.Cost = Cost;
            }
    }
    #endregion

    class customerPurchase
    {
        public int purchaseItemNumber { get; set; }

        public string Store { get; set; }

        public string ProductName { get; set; }

        public int Quantity { get; set; }

        public customerPurchase(int purchaseItemNumber, string Store, string ProductName, int Quantity)
        {
            this.purchaseItemNumber = purchaseItemNumber;
            this.Store = Store;
            this.ProductName = ProductName;
            this.Quantity = Quantity;
        }

    }
}
