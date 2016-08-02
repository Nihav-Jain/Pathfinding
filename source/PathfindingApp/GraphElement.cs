using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Diagnostics;

namespace PathfindingApp
{
	public class ClickEventArgs : EventArgs
	{
		public UIElement ClickedElement { get; private set; }
		public ClickEventArgs(UIElement element)
		{
			ClickedElement = element;
		}
	}

	public abstract class GraphElement
	{
		protected UIElement displayElement;
		private UIElement mClickableElement;
		private Stopwatch watch;
		private static float MAX_CLICK_DURATION = 300;
		public bool IsSelected { get; protected set; }

		protected Canvas parent;

		public event EventHandler<ClickEventArgs> MouseClicked;

		public GraphElement()
		{
			watch = new Stopwatch();
			mClickableElement = null;
			IsSelected = false;
		}

		protected void SetClickableElement(UIElement element)
		{
			mClickableElement = element;
			mClickableElement.MouseDown += ClickableElement_MouseDown;
		}

		private void ClickableElement_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if(watch.IsRunning)
			{
				watch.Stop();
				watch.Reset();
			}
			else
				mClickableElement.MouseUp += ClickableElement_MouseUp;
			watch.Start();
		}

		private void ClickableElement_MouseUp(object sender, MouseButtonEventArgs e)
		{
			mClickableElement.MouseUp -= ClickableElement_MouseUp;
			watch.Stop();
			if(watch.ElapsedMilliseconds <= MAX_CLICK_DURATION)
			{
				MouseClicked?.Invoke(this, new ClickEventArgs(mClickableElement));
			}
			watch.Reset();
		}

		public bool ToggleNodeSelection()
		{
			IsSelected = !IsSelected;
			if ( IsSelected )
				SelectElement();
			else
				DeselectElement();
			return IsSelected;
		}

		public abstract void SelectElement();
		public abstract void DeselectElement();

		public abstract void Delete();
	}
}
