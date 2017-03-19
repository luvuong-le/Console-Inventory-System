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

    }

    class OwnerStock
    {
        public int ID { get; set; }

        public string ProductName { get; set; }

        public int CurrentStock { get; set; }

        public Boolean StockAvailability { get; set; }
    }
}
