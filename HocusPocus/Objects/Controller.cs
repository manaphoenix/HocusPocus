using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Xml.Serialization;
using System.Collections.Specialized;
using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HocusPocus.Objects
{
	public class Controller : INotifyPropertyChanged
	{
		private ObservableCollection<RandomizerTreeItem> _Items;
		private List<RandomizerTreeItem> _ItemAccess;
		private List<ComboBoxItem> _Options;
		private readonly StringBuilder builder;
		private readonly Random RNG = new Random();

		public event PropertyChangedEventHandler PropertyChanged;

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

		public List<ComboBoxItem> FunctionOptions
		{
			get { return _Options; }

			set
			{
				if (_Options != value)
				{
					_Options = value;
					PropertyChange();
				}
			}
		}

		public Controller()
		{
			builder = new StringBuilder();
			_Items = new ObservableCollection<RandomizerTreeItem>();
			_ItemAccess = new List<RandomizerTreeItem>();
			_Options = new List<ComboBoxItem>();

			MyItems.CollectionChanged += Items_CollectionChanged;

			var zerothIndex = new ComboBoxItem()
			{
				Content = "None"
			};

			FunctionOptions.Add(zerothIndex);
		}

		public void UpdateCollection()
		{
			for (var i = 0; i < _Options.Count; i++)
			{
				string str = (string)_Options[i].Content;
				if (str == "None") continue;

				_Options[i].Content = MyItems[i - 1].Item.ItemName;
			}
		}

		private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			var newItems = e.NewItems;
			var oldItems = e.OldItems;

			if (e.Action == NotifyCollectionChangedAction.Add)
			{
				foreach (RandomizerTreeItem item in newItems)
				{
					Debug.WriteLine(item.Item.ItemName);
					var newIndex = new ComboBoxItem()
					{
						Content = item.Item.ItemName
					};

					FunctionOptions.Add(newIndex);
				}
				return;
			}

			if (e.Action == NotifyCollectionChangedAction.Remove)
			{
				foreach (RandomizerTreeItem item in oldItems)
				{
					var f = FunctionOptions.First(x => (string)x.Content == item.Item.ItemName);
					FunctionOptions.Remove(f);
				}
				return;
			}

			if (e.Action != NotifyCollectionChangedAction.Add && e.Action != NotifyCollectionChangedAction.Remove)
			{
				Debug.WriteLine("Unhandled: " + e.Action);
			}
		}

		public RandomizerTreeItem FindTopMostParent(RandomizerTreeItem item)
		{
			var par = item;

			while (par.Parent is RandomizerTreeItem it)
			{
				par = it;
			}

			return par;
		}

		public void RollLogic(RandomizerTreeItem item)
		{
			var roll = RNG.Next(0, item.Items.Count);
			if (item.Items.Count < 1) return;
			RandomizerTreeItem result = (RandomizerTreeItem)item.Items[roll];
			builder.Append(result.Item.OutputValue + "\n");

			if (result.Items.Count > 0)
			{
				RollLogic(result);
			}

			if (result.Item.SelectedFunction > 0)
			{
				var top = FindTopMostParent(result);
				var func = MyItems[result.Item.SelectedFunction - 1];

				if (top.Item.ItemName != func.Item.ItemName)
				RollLogic(func);
			}
		}

		public StringBuilder DoRoll(RandomizerTreeItem item)
		{
			builder.Clear();

			if (item.Items.Count < 1) return builder;

			RollLogic(item);

			return builder;
		}

		public RandomizerTreeItem NewItem()
		{
			var item = new RandomizerTreeItem();
			Items.Add(item);

			return item;
		}

		public void PropertyChange([CallerMemberName] string propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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