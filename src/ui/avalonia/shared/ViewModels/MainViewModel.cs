using CommunityToolkit.Mvvm.ComponentModel;

namespace Zongsoft.Discussions.Presentation.ViewModels;

public partial class MainViewModel : ViewModelBase
{
	[ObservableProperty]
	private string _greeting = "Welcome to Avalonia!";
}
