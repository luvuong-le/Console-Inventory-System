using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WDT_S3546932
{
    class JsonObjects
    {
        public class ProductLineObjects
        {
            public string ID { get; set; }
            public string ProductName { get; set; }
            public string CurrentStock { get; set; }
        }

        public class StockRequest
        {
            public string ID { get; set; }

            public string StoreName { get; set; }
    
            public string ProductRequested { get; set; }

            public string Quantity { get; set; }

            public string CurrentStock { get; set; }

            public Boolean StockAvailability { get; set; }


        }
    }
}
