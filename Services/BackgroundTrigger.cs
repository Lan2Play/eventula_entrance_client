namespace EventulaEntranceClient.Services;

public class BackgroundTrigger
{
    private readonly ILogger<BackgroundTrigger> _Logger;
    readonly CancellationTokenSource _Cts;
    private Func<Task> _Trigger;

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

    public void SubscribeTrigger(Func<Task> trigger)
    {
        _Trigger = trigger;
    }

    public void Unsubscribe(Func<Task> trigger)
    {
        if (_Trigger == trigger)
        {
            _Trigger = null;
        }
    }

    private async Task CaptureFrameTimer(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                if (_Trigger != null)
                {
                    await _Trigger();
                }

                await Task.Delay(TimeSpan.FromMilliseconds(500), cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, "Exception on capturing frame");
            }
        }
    }
}
