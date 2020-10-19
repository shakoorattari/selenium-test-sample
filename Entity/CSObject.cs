using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    public class CSObject
    {
        public CSObject()
        {
            SellerDetails = new List<CSSellersObject>();
        }
        public string ASIN { get; set; }
        public string TotalStockCount { get; set; }
        public string NoOfSellers { get; set; }
        public string totalFBAStock { get; set; }
        public string totalFBMStock { get; set; }
        public string totalAMZStock { get; set; }
        public List<CSSellersObject> SellerDetails { get; set; }
    }

    public class CSSellersObject
    {
        public string SellerName { get; set; }
        public string Quantity { get; set; }
        public string Price { get; set; }
        public string Type { get; set; }
        public string Rating { get; set; }
        public string Condition { get; set; }
        public string Reviews { get; set; }
    }
}
