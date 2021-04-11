using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

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
			if (item.Items.Count < 1) return;
			RandomizerTreeItem result = (RandomizerTreeItem)item.Items[roll];

			if (result.Items.Count > 0)
			{
				builder.Append(result.Item.OutputValue + "\n");
				NestedRoll(result);
			}

			if (result.Item.Function && MyItems.Any(x => x.Item.ItemName == result.Item.OutputValue))
			{
				var func = MyItems.First(x => x.Item.ItemName == result.Item.OutputValue);
				NestedRoll(func);
			}
			else
			{
				if (result.Items.Count <= 0)
					builder.Append(result.Item.OutputValue + "\n");
			}
		}

		public StringBuilder DoRoll(RandomizerTreeItem item)
		{
			builder.Clear();

			if (item.Items.Count < 1) return builder;

			var roll = RNG.Next(0, item.Items.Count);
			RandomizerTreeItem result = (RandomizerTreeItem)item.Items[roll];

			if (result.Items.Count > 0)
			{
				builder.Append(result.Item.OutputValue + "\n");
				NestedRoll(result);
			}

			if (result.Item.Function && MyItems.Any(x => x.Item.ItemName == result.Item.OutputValue))
			{
				var func = MyItems.First(x => x.Item.ItemName == result.Item.OutputValue);
				NestedRoll(func);
			}
			else
			{
				if (result.Items.Count <= 0)
					builder.Append(result.Item.OutputValue + "\n");
			}

			return builder;
		}

		public RandomizerTreeItem NewItem()
		{
			var item = new RandomizerTreeItem();
			Items.Add(item);

			return item;
		}

		public void Save()
		{
			var items = new List<RandomizerItem>();
			_ItemAccess.ForEach(x =>
			{
				if (Items.Any(y => y.Item.ChildID == x.Item.ParentID) || x.Item.ParentID == Guid.Empty)
					items.Add(x.Item);
			});

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
					var it = NewItem();
					it.Item = item;
					it.Header = item.ItemName;

					if (item.ParentID != Guid.Empty)
					{
						bool FoundParent = false;
						// add back to child TODO
						foreach (var itema in _ItemAccess)
						{
							if (itema.Item.ChildID == item.ParentID)
							{
								itema.Items.Add(it);
								FoundParent = true;
								break;
							}
						}

						if (!FoundParent)
						{
							Items.Remove(it);
						}
					}
					else
					{
						MyItems.Add(it);
					}
				}
			}
		}
	}
}