using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhiExplorer.ViewModels;

namespace PhiExplorer;

public partial class DownloadModelPage : ContentPage
{
    private PhiModelDownloadViewModel vm;

    public DownloadModelPage(IServiceProvider provider)
    {
        InitializeComponent();
        this.BindingContext = this.vm = provider.GetRequiredService<PhiModelDownloadViewModel>();
    }

    private async void CloseModal(object sender, EventArgs e)
    {
        await this.Navigation.PopModalAsync();
    }
}