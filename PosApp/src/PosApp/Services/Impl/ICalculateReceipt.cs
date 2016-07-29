using PosApp.Domain;

namespace PosApp.Services.Impl
{
    public interface ICalculateReceipt
    {
        Receipt GetPromotedReceipt(Receipt receipt); 
    }
}