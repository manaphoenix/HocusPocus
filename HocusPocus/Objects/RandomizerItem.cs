using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace HocusPocus.Objects
{
	[Serializable]
	public class RandomizerItem : INotifyPropertyChanged, ISerializable
	{
		private string _Name;
		private string _OutputValue;
		private bool _Nested;
		private bool _Function;
		private Guid _ParentID;
		private Guid _ChildID;

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

		public bool Function
		{
			get { return _Function; }

			set
			{
				if (_Function != value)
				{
					_Function = value;
					PropertyChange();
				}
			}
		}

		public Guid ParentID
		{
			get { return _ParentID; }

			set
			{
				if (_ParentID != value)
				{
					_ParentID = value;
					PropertyChange();
				}
			}
		}

		public Guid ChildID
		{
			get { return _ChildID; }

			set
			{
				if (_ChildID != value)
				{
					_ChildID = value;
					PropertyChange();
				}
			}
		}

		public RandomizerItem()
		{
			ItemName = "New Randomizer";
			Nested = false;
			Function = false;
			ChildID = Guid.NewGuid();
		}

		public RandomizerItem(SerializationInfo info, StreamingContext context)
		{
			_Name = (string)info.GetValue("ItemName", typeof(string));
			_OutputValue = (string)info.GetValue("OutputValue", typeof(string));
			_Nested = (bool)info.GetValue("Nested", typeof(bool));
			_Function = (bool)info.GetValue("Function", typeof(bool));
			_ParentID = (Guid)info.GetValue("ParentID", typeof(Guid));
			_ChildID = (Guid)info.GetValue("ChildID", typeof(Guid));
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
			info.AddValue("Function", _Function);
			info.AddValue("ParentID", _ParentID);
			info.AddValue("ChildID", _ChildID);
		}
	}
}