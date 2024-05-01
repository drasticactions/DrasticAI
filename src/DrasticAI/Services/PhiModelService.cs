using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Drastic.Services;
using DrasticAI.Models;
using Microsoft.Extensions.DependencyInjection;

namespace DrasticAI.Services;

public class PhiModelService : INotifyPropertyChanged
{
    private IAppDispatcher dispatcher;
    private PhiModel? selectedModel;

    public PhiModelService(IServiceProvider provider)
    {
        this.dispatcher = provider.GetRequiredService<IAppDispatcher>();
        foreach (var item in Enum.GetValues<ParameterType>())
        {
            foreach (var qType in Enum.GetValues<QuantizationType>())
            {
                this.AllModels.Add(new PhiModel((ParameterType)item, (QuantizationType)qType));
            }
        }

        this.UpdateAvailableModels();
    }

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;

    public event EventHandler? OnUpdatedSelectedModel;

    public event EventHandler? OnAvailableModelsUpdate;

    public ObservableCollection<PhiModel> AllModels { get; } = new ObservableCollection<PhiModel>();

    public ObservableCollection<PhiModel> AvailableModels { get; } = new ObservableCollection<PhiModel>();

    public PhiModel? SelectedModel
    {
        get
        {
            return this.selectedModel;
        }

        set
        {
            this.SetProperty(ref this.selectedModel, value);
            this.OnUpdatedSelectedModel?.Invoke(this, EventArgs.Empty);
        }
    }

    public void UpdateAvailableModels()
    {
        lock (this)
        {
            this.dispatcher.Dispatch(() =>
            {
                this.AvailableModels.Clear();
                var models = this.AllModels.Where(n => n.Exists);
                foreach (var model in models)
                {
                    this.AvailableModels.Add(model);
                }

                if (this.SelectedModel is not null && !this.AvailableModels.Contains(this.SelectedModel))
                {
                    this.SelectedModel = null;
                }

                this.SelectedModel ??= this.AvailableModels.FirstOrDefault();
            });
        }

        this.OnAvailableModelsUpdate?.Invoke(this, EventArgs.Empty);
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
    protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = "", Action? onChanged = null)
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
}