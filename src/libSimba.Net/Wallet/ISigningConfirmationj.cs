using System.Threading.Tasks;
using libSimba.Net.Models.Transaction;

namespace libSimba.Net.Wallet
{
    public interface ISigningConfirmation
    {
        Task<bool> RequestSigningConfirmation(RawPayload payload);
    }
}