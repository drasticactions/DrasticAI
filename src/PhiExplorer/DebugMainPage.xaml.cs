using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhiExplorer;

public partial class DebugMainPage : ContentPage
{
    IServiceProvider provider;

    public DebugMainPage(IServiceProvider provider)
    {
        this.InitializeComponent();
        this.provider = provider;
    }
}