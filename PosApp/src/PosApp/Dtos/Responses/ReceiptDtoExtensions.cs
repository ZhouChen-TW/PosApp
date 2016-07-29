using System.Linq;
using System.Text;
using NHibernate.Util;
using PosApp.Domain;

namespace PosApp.Dtos.Responses
{
    static class ReceiptDtoExtensions
    {
        public static string ToReceiptDto(this Receipt receipt)
        {
            StringBuilder receiptBuilder = new StringBuilder(256)
                .AppendLine("Receipt:")
                .AppendLine("--------------------------------------------------");

            receipt.ReceiptItems.OrderBy(ri => ri.Product.Name)
                .Select(ri =>
                {
                    string price = ri.Total.ToString("F2");
                    string Promoted =
                        receipt.PromotionItems.Where(
                            (p => p.Product.Barcode.Equals(ri.Product.Barcode)))
                            .Select(g => g.Promoted).Single().ToString("F2");           
                    return $"Product: {ri.Product.Name}, Amount: {ri.Amount}, Price: {price}, Promoted: {Promoted}";
                })
                .ForEach(ri => receiptBuilder.AppendLine(ri));

            return receiptBuilder
                .AppendLine("--------------------------------------------------")
                .AppendLine($"Promoted: {receipt.Promoted.ToString("F2")}")
                .Append($"Total: {receipt.Total.ToString("F2")}")
                .ToString();
        }
    }
}