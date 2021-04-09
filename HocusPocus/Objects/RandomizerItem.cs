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
	[Serializable]
	public class RandomizerItem : TreeViewItem, INotifyPropertyChanged, ISerializable
	{
		private string _Name;
		private string _OutputValue;
		private bool _Nested;

		public event PropertyChangedEventHandler PropertyChanged;

		public string ItemName
		{
			get { return _Name; }

			set
			{
				if (_Name != value)
				{
					_Name = value;
					PropertyChange();
				}
			}
		}

		public string OutputValue 
		{ 
			get { return _OutputValue ?? _Name; }

			set
			{
				if (_OutputValue != value)
				{
					_OutputValue = value;
					PropertyChange();
				}
			}
		}

		public bool Nested
		{
			get { return _Nested; }

			set
			{
				if (_Nested != value)
				{
					_Nested = value;
					PropertyChange();
				}
			}
		}

		public RandomizerItem()
		{
			ItemName = "New Randomizer";
			Nested = false;
			Header = ItemName;
			Foreground = Brushes.White;
		}

		public RandomizerItem(SerializationInfo info, StreamingContext context)
		{
			_Name = (string)info.GetValue("ItemName", typeof(string));
			_OutputValue = (string)info.GetValue("OutputValue", typeof(string));
			_Nested = (bool)info.GetValue("Nested", typeof(bool));
		}

		public void PropertyChange([CallerMemberName] string propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("ItemName", _Name);
			info.AddValue("OutputValue", _OutputValue);
			info.AddValue("Nested", _Nested);
		}
	}
}
