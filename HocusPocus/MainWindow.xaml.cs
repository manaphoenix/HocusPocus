using HocusPocus.Objects;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace HocusPocus
{
	public partial class MainWindow : Window
	{
		private static IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
		{
			switch (msg)
			{
				case 0x0024:
					WmGetMinMaxInfo(hwnd, lParam);
					handled = true;
					break;
			}
			return (IntPtr)0;
		}

		private static void WmGetMinMaxInfo(IntPtr hwnd, IntPtr lParam)
		{
			MINMAXINFO mmi = (MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(MINMAXINFO));
			int MONITOR_DEFAULTTONEAREST = 0x00000002;
			IntPtr monitor = MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST);
			if (monitor != IntPtr.Zero)
			{
				MONITORINFO monitorInfo = new MONITORINFO();
				GetMonitorInfo(monitor, monitorInfo);
				RECT rcWorkArea = monitorInfo.rcWork;
				RECT rcMonitorArea = monitorInfo.rcMonitor;
				mmi.ptMaxPosition.x = Math.Abs(rcWorkArea.left - rcMonitorArea.left);
				mmi.ptMaxPosition.y = Math.Abs(rcWorkArea.top - rcMonitorArea.top);
				mmi.ptMaxSize.x = Math.Abs(rcWorkArea.right - rcWorkArea.left);
				mmi.ptMaxSize.y = Math.Abs(rcWorkArea.bottom - rcWorkArea.top);
			}
			Marshal.StructureToPtr(mmi, lParam, true);
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct POINT
		{
			/// <summary>x coordinate of point.</summary>
			public int x;

			/// <summary>y coordinate of point.</summary>
			public int y;

			/// <summary>Construct a point of coordinates (x,y).</summary>
			public POINT(int x, int y)
			{
				this.x = x;
				this.y = y;
			}
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct MINMAXINFO
		{
			public POINT ptReserved;
			public POINT ptMaxSize;
			public POINT ptMaxPosition;
			public POINT ptMinTrackSize;
			public POINT ptMaxTrackSize;
		};

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		public class MONITORINFO
		{
			public int cbSize = Marshal.SizeOf(typeof(MONITORINFO));
			public RECT rcMonitor = new RECT();
			public RECT rcWork = new RECT();
			public int dwFlags = 0;
		}

		[StructLayout(LayoutKind.Sequential, Pack = 0)]
		public struct RECT
		{
			public int left;
			public int top;
			public int right;
			public int bottom;
			public static readonly RECT Empty = new RECT();
			public int Width { get { return Math.Abs(right - left); } }
			public int Height { get { return bottom - top; } }

			public RECT(int left, int top, int right, int bottom)
			{
				this.left = left;
				this.top = top;
				this.right = right;
				this.bottom = bottom;
			}

			public RECT(RECT rcSrc)
			{
				left = rcSrc.left;
				top = rcSrc.top;
				right = rcSrc.right;
				bottom = rcSrc.bottom;
			}

			public bool IsEmpty { get { return left >= right || top >= bottom; } }

			public override string ToString()
			{
				if (this == Empty) { return "RECT {Empty}"; }
				return "RECT { left : " + left + " / top : " + top + " / right : " + right + " / bottom : " + bottom + " }";
			}

			public override bool Equals(object obj)
			{
				if (!(obj is Rect)) { return false; }
				return (this == (RECT)obj);
			}

			/// <summary>Return the HashCode for this struct (not garanteed to be unique)</summary>
			public override int GetHashCode() => left.GetHashCode() + top.GetHashCode() + right.GetHashCode() + bottom.GetHashCode();

			/// <summary> Determine if 2 RECT are equal (deep compare)</summary>
			public static bool operator ==(RECT rect1, RECT rect2) { return (rect1.left == rect2.left && rect1.top == rect2.top && rect1.right == rect2.right && rect1.bottom == rect2.bottom); }

			/// <summary> Determine if 2 RECT are different(deep compare)</summary>
			public static bool operator !=(RECT rect1, RECT rect2) { return !(rect1 == rect2); }
		}

		[DllImport("user32")]
		internal static extern bool GetMonitorInfo(IntPtr hMonitor, MONITORINFO lpmi);

		[DllImport("User32")]
		internal static extern IntPtr MonitorFromWindow(IntPtr handle, int flags);

		// INIT
		public Controller Base;

		public MainWindow()
		{
			Base = new Controller();
			DataContext = Base;

			InitializeComponent();

			SourceInitialized += (s, e) =>
			{
				IntPtr handle = (new WindowInteropHelper(this)).Handle;
				HwndSource.FromHwnd(handle).AddHook(new HwndSourceHook(WindowProc));
			};

			MinimizeButton.Click += (s, e) => WindowState = WindowState.Minimized;
			MaximizeButton.Click += (s, e) =>
			{
				WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
				var RMaxImage = new BitmapImage();
				RMaxImage.BeginInit();
				RMaxImage.UriSource = new Uri("pack://application:,,,/data/image/RevertMaximizeIcon.png");
				RMaxImage.EndInit();
				var MaxImage = new BitmapImage();
				MaxImage.BeginInit();
				MaxImage.UriSource = new Uri("pack://application:,,,/data/image/MaximizeIcon.png");
				MaxImage.EndInit();

				MaxImageContent.Source = WindowState == WindowState.Maximized ? RMaxImage : MaxImage;
			};
			ExitButton.Click += (s, e) => Close();

			Closed += MainWindow_Closed;
			Loaded += MainWindow_Loaded;
		}

		private void MainWindow_Loaded(object sender, RoutedEventArgs e)
		{
			Base.Load();
		}

		private void MainWindow_Closed(object sender, EventArgs e)
		{
			Base.Save();
		}

		//Events
		private void AddButton_Clicked(object sender, RoutedEventArgs e)
		{
			var newItem = Base.NewItem();

			if (MyTreeView.SelectedItem != null)
			{
				var parent = MyTreeView.SelectedItem as RandomizerTreeItem;
				newItem.Item.ParentID = parent.Item.ChildID;

				parent.Items.Add(newItem);
			}
			else
			{
				Base.MyItems.Add(newItem);
			}
		}

		private void MyTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			if (MyTreeView.SelectedItem != null) // stupid check to get it to correctly unselect V.V
			{
				OptionBox.Visibility = Visibility.Visible;
				RandomizerSettings.Visibility = Visibility.Visible;
				AppSettings.Visibility = Visibility.Collapsed;
			}

			var item = (RandomizerTreeItem)e.NewValue;
			BindingOperations.ClearBinding(TextName, TextBox.TextProperty);
			BindingOperations.ClearBinding(ValueBox, TextBox.TextProperty);
			BindingOperations.ClearBinding(FunctionCombo, Selector.SelectedIndexProperty);
			BindingOperations.ClearBinding(FunctionCombo, ItemsControl.ItemsSourceProperty);

			if (item == null) return;

			var myBinding = new Binding()
			{
				Source = item.Item,
				Path = new PropertyPath("ItemName"),
				Mode = BindingMode.TwoWay,
				UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
			};
			BindingOperations.SetBinding(TextName, TextBox.TextProperty, myBinding);
			BindingOperations.SetBinding(item, HeaderedItemsControl.HeaderProperty, myBinding);

			myBinding = new Binding()
			{
				Source = item.Item,
				Path = new PropertyPath("OutputValue"),
				Mode = BindingMode.TwoWay,
				UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
			};
			BindingOperations.SetBinding(ValueBox, TextBox.TextProperty, myBinding);

			myBinding = new Binding()
			{
				Source = item.Item,
				Path = new PropertyPath("SelectedFunction"),
				Mode = BindingMode.TwoWay,
				UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
			};
			BindingOperations.SetBinding(FunctionCombo, Selector.SelectedIndexProperty, myBinding);

			myBinding = new Binding()
			{
				Source = Base,
				Path = new PropertyPath("FunctionOptions"),
				Mode = BindingMode.OneWay,
				UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
			};
			BindingOperations.SetBinding(FunctionCombo, ItemsControl.ItemsSourceProperty, myBinding);

			Base.UpdateCollection();
			if (item.Item.SelectedFunction > FunctionCombo.Items.Count)
			{
				item.Item.SelectedFunction = 0;
			}

			if (item.Parent is RandomizerTreeItem)
			{
				OptionsPanel.Visibility = Visibility.Visible;
			} else
			{
				OptionsPanel.Visibility = Visibility.Collapsed;
			}
		}

		private void DeleteButton_Clicked(object sender, RoutedEventArgs e)
		{
			if (MyTreeView.SelectedItem is RandomizerTreeItem item)
			{
				if (item.Parent is RandomizerTreeItem parent)
				{
					Debug.WriteLine("Removing!");
					parent.Items.Remove(item);
				}
				else
				{
					Base.MyItems.Remove(item);
				}

				Base.Items.Remove(item);
			}
		}

		private void RollButton_Clicked(object sender, RoutedEventArgs e)
		{
			if (MyTreeView.SelectedItem is RandomizerTreeItem item)
			{
				var result = Base.DoRoll(item);
				Output.Text = result.ToString();
			}
		}

		//Settings button
		private void Settings_Clicked(object sender, RoutedEventArgs e)
		{
			OptionBox.Visibility = Visibility.Visible;
			RandomizerSettings.Visibility = Visibility.Collapsed;
			AppSettings.Visibility = Visibility.Visible;
			Output.Text = "TEST\ntest\n12345";
		}

		// Non-Important events
		private void MyTreeView_MouseDown(object sender, MouseButtonEventArgs e)
		{
			OptionBox.Visibility = Visibility.Hidden;

			if (OptionBox.Visibility == Visibility.Hidden && MyTreeView.SelectedItem != null)
			{
				RandomizerTreeItem item = (RandomizerTreeItem)MyTreeView.SelectedItem;
				item.IsSelected = false;
			}
		}

		private void ToolBar_Loaded(object sender, RoutedEventArgs e)
		{
			var toolBar = sender as ToolBar;
			if (toolBar.Template.FindName("OverflowGrid", toolBar) is FrameworkElement overflowGrid)
			{
				overflowGrid.Visibility = Visibility.Collapsed;
			}
			if (toolBar.Template.FindName("MainPanelBorder", toolBar) is FrameworkElement mainPanelBorder)
			{
				mainPanelBorder.Margin = new Thickness();
			}
		}
	}
}