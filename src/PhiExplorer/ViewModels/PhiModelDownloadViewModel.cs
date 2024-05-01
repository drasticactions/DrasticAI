using Drastic.ViewModels;
using DrasticAI.Models;
using DrasticAI.Services;
using Microsoft.Maui.Adapters;
using PhiExplorer.Models;

namespace PhiExplorer.ViewModels;

public class PhiModelDownloadViewModel
    : BaseViewModel
{
    private PhiModelService modelService;
    private List<PhiDownload> downloads;
    private IList<PhiDownloadSection> sections = new List<PhiDownloadSection>();
    public PhiModelDownloadViewModel(IServiceProvider services)
        : base(services)
    {
        this.modelService = services.GetService(typeof(PhiModelService)) as PhiModelService ??
                            throw new NullReferenceException(nameof(PhiModelService));
        this.downloads = this.modelService.AllModels
            .Select(n => new PhiDownload(n, this.modelService, this.Dispatcher)).ToList();
        this.Downloads = new VirtualListViewAdapter<PhiDownload>(this.downloads);
        this.SectionedDownloads = new SectionedPhiDownloadAdapter(this.sections);
        this.SectionedDownloads.AddItems(this.downloads);
    }

    public PhiModelService ModelService => this.modelService;

    public VirtualListViewAdapter<PhiDownload> Downloads { get; }
    
    public SectionedPhiDownloadAdapter SectionedDownloads { get; }
}