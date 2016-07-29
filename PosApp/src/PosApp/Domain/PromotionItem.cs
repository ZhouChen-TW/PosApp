namespace PosApp.Domain
{
    public class PromotionItem
    {
        public PromotionItem(Product product, int amount)
        {
            Product = product;
            Amount = amount;
            Promoted = amount*product.Price;
        }

        public Product Product { get; }
        public int Amount { get; }
        public decimal Promoted { get; }
    }
}