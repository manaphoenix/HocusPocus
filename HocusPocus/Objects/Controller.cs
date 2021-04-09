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
		private ObservableCollection<RandomizerItem> _Items;
		private StringBuilder builder;
		private readonly Random RNG = new Random(Guid.NewGuid().GetHashCode());

		public ObservableCollection<RandomizerItem> MyItems
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
			MyItems = new ObservableCollection<RandomizerItem>();
		}

		public void NestedRoll(RandomizerItem item)
		{
			var roll = RNG.Next(0, item.Items.Count);
			RandomizerItem result = (RandomizerItem)item.Items[roll];
			builder.Append(result.OutputValue + "\n");

			if (result.Nested)
			{
				NestedRoll(result);
			}
		}

		public StringBuilder DoRoll(RandomizerItem item)
		{
			builder.Clear();

			if (item.Items.Count < 1) return builder;

			var roll = RNG.Next(0, item.Items.Count);
			RandomizerItem result = (RandomizerItem)item.Items[roll];
			builder.Append(result.OutputValue + "\n");

			if (result.Nested)
			{
				NestedRoll(result);
			}

			return builder;
		}

		public void NewItem()
		{
			var item = new RandomizerItem();
			MyItems.Add(item);
		}

		
		public void Save()
		{
			/*
			var items = _Items.ToArray();

			var bf = new BinaryFormatter();
			var stream = File.Open("Randomizer.dat", FileMode.Create, FileAccess.Write);

			bf.Serialize(stream, items);
			stream.Close();
			*/
		}

		public void Load()
		{
			/*
			if (File.Exists("Randomizer.dat"))
			{
				var bf = new BinaryFormatter();
				var stream = File.Open("Randomizer.dat", FileMode.Open, FileAccess.Read);

				var items = (RandomizerItem[])bf.Deserialize(stream);
				stream.Close();

				foreach (var item in items)
				{
					item.Header = item.ItemName;
					_Items.Add(item);
				}
			}
			*/
		}
		
	}
}
