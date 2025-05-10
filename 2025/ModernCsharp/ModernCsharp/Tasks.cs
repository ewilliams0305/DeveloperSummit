using System.Runtime.CompilerServices;
using ModernCsharp.ValueObjects;

namespace ModernCsharp;

public sealed class ValueTasks
{
    public async ValueTask ProcessLogicThanMightBeAsync(string id)
    {
        if (id.Contains("NOPE"))
        {
            // The method has a hotpath.  The compiler will actually remove the Task and allocate nothing
            return;
        }

        // The ID is not NOPE so we run some async operation, the compiler will use a Task behind the scenes
        await Task.Delay(5000);
    }

    public async ValueTask<int> ProcessLogicThanMightBeAsync(int value)
    {
        if (value < 5)
        {   
            // Again we have a hotpath and simply return the int required by the <T> param
            return value * 2;
        }

        // Actually we do need to do an async operation.
        await Task.Delay(5000);
        return value * 5;
    }
}

public class TaskExtensions
{
    public async Task<int> AsyncAction(int val)
    {
        await Task.Delay(val);
        return val;
    }

    public async Task RunTillAnyCompletes()
    {
        Task<int>[] tasks =
        [
            AsyncAction(12),
            AsyncAction(32),
            AsyncAction(55),
        ];

        var any = await Task.WhenAny(tasks);

        var all = await Task.WhenAll(tasks);

        await foreach (var val in Task.WhenEach(tasks))
        {
            // Stream the results as they come back
        }
    }
}

public class AsyncEnumerableDemo
{

    public async Task ProcessAllVolumeControls(CancellationToken token)
    {
        await foreach (var volume in LoadAllVolumes(token))
        {
            var status = await ProcessVolume(volume, token);

            if (!status)
            {
                break;
            }
        }
    }

    public async Task Naive(CancellationToken token)
    {
        for (int i = 0; i < 200; i++)
        {
            var volume = await GetVolume( token);
            var status = await ProcessVolume(volume, token);

            if (!status)
            {
                break;
            }
        }
    }

    public async IAsyncEnumerable<VolumeLevel> LoadAllVolumes([EnumeratorCancellation] CancellationToken token)
    {
        for (int i = 0; i < 200; i++)
        {
            yield return await GetVolume(token);
        }
    }

    public async Task<VolumeLevel> GetVolume(CancellationToken token)
    {
        await Task.Delay(5000, token);
        return new VolumeLevel(50);
    }

    public async Task<bool> ProcessVolume(VolumeLevel volume, CancellationToken token)
    {
        await Task.Delay(5000, token);
        return volume > 20;
    }

}