using System.Windows.Controls;
using System.Windows.Media;

namespace HocusPocus.Objects
{
	public class RandomizerTreeItem : TreeViewItem
	{
		public RandomizerItem Item;

		public RandomizerTreeItem()
		{
			Item = new RandomizerItem();
			Header = Item.ItemName;
			Foreground = Brushes.White;
		}
	}
}