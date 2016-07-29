using System.Collections.Generic;
using System.Linq;

namespace PosApp.Domain
{
    public class Receipt
    {
        public Receipt(IList<ReceiptItem> receiptItems)
        {
            ReceiptItems = receiptItems;
            Total = receiptItems.Sum(r => r.Total);
        }

        public IList<ReceiptItem> ReceiptItems { get;}
        public decimal Total { get; set; }
        public decimal Promoted { get; set; }
    }
}