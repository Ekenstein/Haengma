using Haengma.Backend.Imperative.Persistance;

namespace Haengma.Backend.Imperative.Services
{
    public record ServiceContext(ITransactionFactory Transactions);
}
