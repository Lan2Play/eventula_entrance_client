namespace EventulaEntranceClient.Services;

public class BackgroundTrigger
{
    private readonly ILogger<BackgroundTrigger> _Logger;
    CancellationTokenSource _Cts;

    public BackgroundTrigger(ILogger<BackgroundTrigger> logger)
    {
        _Cts = new CancellationTokenSource();

        Task.Run(() => CaptureFrameTimer(_Cts.Token).ConfigureAwait(false));
        _Logger = logger;
    }

    public void Stop()
    {
        _Cts.Cancel();
    }

    public event EventHandler Trigger;

    private async Task CaptureFrameTimer(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                Trigger?.Invoke(this, new EventArgs());

                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, "Exception on capturing frame");
            }
        }
    }
}
