using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Controls;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using System.IO;
using System.Linq;

namespace HocusPocus.Objects
{
	public class Controller
	{
		private ObservableCollection<RandomizerTreeItem> _Items;
		private StringBuilder builder;
		private readonly Random RNG = new Random();

		public ObservableCollection<RandomizerTreeItem> MyItems
		{
			get { return _Items; }

			set
			{
				if (_Items != value)
				{
					_Items = value;
				}
			}
		}

		public Controller()
		{
			builder = new StringBuilder();
			MyItems = new ObservableCollection<RandomizerTreeItem>();
		}

		public void NestedRoll(RandomizerTreeItem item)
		{
			var roll = RNG.Next(0, item.Items.Count);
			RandomizerTreeItem result = (RandomizerTreeItem)item.Items[roll];
			builder.Append(result.Item.OutputValue + "\n");

			if (result.Item.Nested)
			{
				NestedRoll(result);
			}
		}

		public StringBuilder DoRoll(RandomizerTreeItem item)
		{
			builder.Clear();

			if (item.Items.Count < 1) return builder;

			var roll = RNG.Next(0, item.Items.Count);
			RandomizerTreeItem result = (RandomizerTreeItem)item.Items[roll];
			builder.Append(result.Item.OutputValue + "\n");

			if (result.Item.Nested)
			{
				NestedRoll(result);
			}

			return builder;
		}

		public void NewItem()
		{
			var item = new RandomizerTreeItem();
			MyItems.Add(item);
		}

		private void NewItem(RandomizerItem internalitem)
		{
			var item = new RandomizerTreeItem();
			item.Item = internalitem;
			item.Header = internalitem.ItemName;
			MyItems.Add(item);
		}

		public void Save()
		{
			var items = new List<RandomizerItem>();

			foreach (var item in _Items)
			{
				items.Add(item.Item);
			}

			var xml = new XmlSerializer(typeof(List<RandomizerItem>));
			//var bf = new BinaryFormatter();
			var stream = File.Open("Randomizer.dat", FileMode.Create, FileAccess.Write);

			using (TextWriter tw = new StreamWriter(stream))
			{
				xml.Serialize(tw, items);
			}

			//bf.Serialize(stream, items);
			stream.Close();
			
		}

		public void Load()
		{
			if (File.Exists("Randomizer.dat"))
			{
				var items = new List<RandomizerItem>();
				var xml = new XmlSerializer(typeof(List<RandomizerItem>));
				var stream = File.Open("Randomizer.dat", FileMode.Open, FileAccess.Read);

				using (TextReader tr = new StreamReader(stream))
				{
					items = (List<RandomizerItem>)xml.Deserialize(tr);
				}

				stream.Close();

				foreach (var item in items)
				{
					NewItem(item);
				}
			}
		}
		
	}
}
