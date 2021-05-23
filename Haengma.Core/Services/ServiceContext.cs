using Haengma.Core.Logics;
using Haengma.Core.Persistence;
using Microsoft.Extensions.Logging;

namespace Haengma.Core.Services
{
    public record ServiceContext(ITransactionFactory Transactions, 
        LogicContext Logics, 
        ILogger Logger,
        IIdGenerator<string> IdGenerator);
}
