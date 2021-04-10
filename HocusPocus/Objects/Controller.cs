using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Controls;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using System.IO;
using System.Linq;
using System.Diagnostics;

namespace HocusPocus.Objects
{
	public class Controller
	{
		private ObservableCollection<RandomizerTreeItem> _Items;
		private List<RandomizerTreeItem> _ItemAccess;
		private readonly StringBuilder builder;
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

		public List<RandomizerTreeItem> Items
		{
			get { return _ItemAccess; }

			set
			{
				if (_ItemAccess != value)
				{
					_ItemAccess = value;
				}
			}
		}

		public Controller()
		{
			builder = new StringBuilder();
			_Items = new ObservableCollection<RandomizerTreeItem>();
			_ItemAccess = new List<RandomizerTreeItem>();
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

		public RandomizerTreeItem NewItem()
		{
			var item = new RandomizerTreeItem();
			Items.Add(item);

			return item;
		}
		/*
		private void NewItem(RandomizerItem internalitem)
		{
			var item = new RandomizerTreeItem
			{
				Item = internalitem,
				Header = internalitem.ItemName
			};

			if (item.Item.UUID == Guid.Empty || !MyItems.Any(x => x.Item.UUID == item.Item.UUID))
			{
				MyItems.Add(item);
			} else
			{
				if (MyItems.Any(x => x.Item.UUID == item.Item.UUID))
				{
					var testItem = MyItems.First(x => x.Item.UUID == item.Item.UUID);
					testItem.Items.Add(item);
				} else
				{
					//Crawl TODO
				}
			}
		}

		public void AddItems(List<RandomizerTreeItem> BaseItems, List<RandomizerItem> SerializableItems)
		{
			foreach (var item in BaseItems)
			{
				SerializableItems.Add(item.Item);
				if (item.Items.Count > 0)
					AddItems(item.Items.OfType<RandomizerTreeItem>().ToList(), SerializableItems);
			}
		}

		public void Save()
		{
			var items = new List<RandomizerItem>();
			AddItems(_Items.ToList(), items);
			
			var xml = new XmlSerializer(typeof(List<RandomizerItem>));
			var stream = File.Open("Randomizer.dat", FileMode.Create, FileAccess.Write);

			using (TextWriter tw = new StreamWriter(stream))
			{
				xml.Serialize(tw, items);
			}

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
		*/
		
	}
}
