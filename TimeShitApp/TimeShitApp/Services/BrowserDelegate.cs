
namespace TimeShit.Services;

public class BrowserDelegate : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }
}