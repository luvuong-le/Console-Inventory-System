using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WDT_S3546932
{
    class Customer : CustomerCLI
    {
        Utility command = new Utility();

        JsonUtility jsonCommand = new JsonUtility();

        public override List<StoreStock> displayProduct()
        {
            throw new NotImplementedException();
        }

        public override void displayWorkShop()
        {
            throw new NotImplementedException();
        }
    }
}
