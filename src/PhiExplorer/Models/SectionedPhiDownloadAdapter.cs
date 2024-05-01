using DrasticAI.Models;
using Microsoft.Maui.Adapters;

namespace PhiExplorer.Models;

public class SectionedPhiDownloadAdapter : VirtualListViewAdapterBase<PhiDownloadSection, PhiDownload>
{
    public readonly IList<PhiDownloadSection> Items;
    
    public SectionedPhiDownloadAdapter(IList<PhiDownloadSection> items)
        : base()
    {
        this.Items = items;
    }
    
    public override int GetNumberOfSections()
        => Items.Count;

    public override int GetNumberOfItemsInSection(int sectionIndex)
        => Items[sectionIndex].Count;

    public override PhiDownload GetItem(int sectionIndex, int itemIndex)
        => Items[sectionIndex][itemIndex];
    
    public override PhiDownloadSection GetSection(int sectionIndex)
        => Items[sectionIndex];

    public void AddItems(List<PhiDownload> downloads)
    {
        var groupedItems = downloads.GroupBy(n => n.Model.Parameters);

        foreach (var groupedItemList in groupedItems)
        {
            var section = Items.FirstOrDefault(s => s.Type == groupedItemList.Key);

            if (section is null)
            {
                section = new PhiDownloadSection() { Type = groupedItemList.Key };
                Items.Add(section);
            }

            section.AddRange(groupedItemList);
        }
    }
    
    public void AddItem(PhiDownload itemName)
    {
        var section = Items.FirstOrDefault(s => s.Type == itemName.Model.Parameters);

        if (section is null)
        {
            section = new PhiDownloadSection() { Type = itemName.Model.Parameters };
            Items.Add(section);
        }

        section.Add(itemName);
        InvalidateData();
    }

    public void RemoveItem(int sectionIndex, int itemIndex)
    {
        var section = Items.ElementAtOrDefault(sectionIndex);

        if (section is null)
            return;

        section.RemoveAt(itemIndex);

        if (section.Count <= 0)
            Items.RemoveAt(sectionIndex);

        InvalidateData();
    }
}

public class PhiDownloadSection : List<PhiDownload>
{
    public ParameterType Type { get; set; }
}