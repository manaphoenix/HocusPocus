using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace HocusPocus.Objects
{
	public class RandomizerTreeItem : TreeViewItem
	{
		public RandomizerItem Item;

		public RandomizerTreeItem()
		{
			Item = new RandomizerItem();
			Item.ItemName = "New Randomizer";
			Item.Nested = false;
			Header = Item.ItemName;
			Foreground = Brushes.White;
		}
	}
}
