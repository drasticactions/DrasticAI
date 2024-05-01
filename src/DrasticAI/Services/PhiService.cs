using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using Drastic.Services;
using Drastic.Tools;
using LlamaCppLib;
using Microsoft.Extensions.DependencyInjection;

namespace DrasticAI.Services;

public class PhiService : INotifyPropertyChanged, IDisposable
{
    private IAppDispatcher dispatcher;
    private IErrorHandlerService error;

    private LlmEngine llmEngine;
    private PhiModelService modelService;
    private SamplingOptions samplingOption;

    public PhiService(IServiceProvider provider)
    {
        this.dispatcher = provider.GetRequiredService<IAppDispatcher>();
        this.modelService = provider.GetRequiredService<PhiModelService>();
        this.error = provider.GetRequiredService<IErrorHandlerService>();
        this.samplingOption = provider.GetService<SamplingOptions>() ?? new SamplingOptions
        {
            ExtraStopTokens = ["<|EOT|>", "<|end_of_turn|>", "<|endoftext|>", "<|im_end|>", "<|eot_id|>", "support:", "<|assistant|>", "<|end|>"],
        };
        this.llmEngine = new LlmEngine(new LlmEngineOptions { MaxParallel = 8 });
    }

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;

    public void Dispose()
    {
        this.llmEngine.UnloadModel();
        this.llmEngine.Dispose();
    }

    /// <summary>
    /// On Property Changed.
    /// </summary>
    /// <param name="propertyName">Name of the property.</param>
    protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        this.dispatcher?.Dispatch(() =>
        {
            var changed = this.PropertyChanged;
            if (changed == null)
            {
                return;
            }

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        });
    }

#pragma warning disable SA1600 // Elements should be documented
    protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = "",
        Action? onChanged = null)
#pragma warning restore SA1600 // Elements should be documented
    {
        if (EqualityComparer<T>.Default.Equals(backingStore, value))
        {
            return false;
        }

        backingStore = value;
        onChanged?.Invoke();
        this.OnPropertyChanged(propertyName);
        return true;
    }

    public async Task<string> PromptAsync(string message, CancellationToken? cancellationToken = default)
    {
        var prompt = this.llmEngine.Prompt(
            message,
            this.samplingOption);

        var response = new List<byte>();
        var ct = cancellationToken ?? CancellationToken.None;

        await foreach (var token in prompt.NextToken(ct))
        {
            response.AddRange(token);
        }

        return Encoding.UTF8.GetString(response.ToArray());
    }

    public async Task PromptAsync(string message, Action<string> tokenCallback, CancellationToken? cancellationToken = default)
    {
        var prompt = this.llmEngine.Prompt(
            message,
            this.samplingOption);

        await foreach (var token in new TokenEnumerator(prompt, cancellationToken))
        {
            tokenCallback(token);
        }
    }

    public Task LoadModelAsync(LlmModelOptions? modelOptions = default)
    {
        var model = this.modelService.SelectedModel;
        if (model is null)
        {
            throw new InvalidOperationException("No model selected.");
        }

        modelOptions ??= new LlmModelOptions
        {
            ContextLength = 0,
            GpuLayers = 0,
            ThreadCount = 8,
            BatchThreadCount = 8,
            UseFlashAttention = false,
        };

        return this.LoadModelAsync(model.FileLocation, modelOptions);
    }

    private async Task LoadModelAsync(string modelPath, LlmModelOptions? modelOptions = default)
    {
        // Unload existing model if it's loaded.
        this.llmEngine.UnloadModel();

        var tcs = new TaskCompletionSource<bool>();

        void ProgressCallback(float progress)
        {
            if (progress >= 100)
            {
                tcs.SetResult(true);
            }
        }

        try
        {
            this.llmEngine.LoadModel(modelPath, modelOptions, ProgressCallback);
        }
        catch (Exception ex)
        {
            this.error.HandleError(ex);
            tcs.SetException(ex);
        }

        await tcs.Task;
    }
}