namespace PhiExplorer;

public partial class MainPage : ContentPage
{
    int count = 0;
    private IServiceProvider provider;
    private DownloadModelPage downloadModelPage;
    
    public MainPage(IServiceProvider provider)
    {
        InitializeComponent();
        this.provider = provider;
        this.downloadModelPage = new DownloadModelPage(provider);
    }

    private async void OnCounterClicked(object sender, EventArgs e)
    {
        await this.Navigation.PushModalAsync(this.downloadModelPage);
    }
}