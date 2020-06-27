using System;
using System.Threading;
using System.Threading.Tasks;

namespace GTP
{
    public interface IGtpConnection : IDisposable
    {
        Task<GtpResponse> SendAsync(GtpCommand command, CancellationToken cancellationToken);
    }
}
