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

namespace PathfindingApp
{
	public class Node : GraphElement
	{
		public double X { get; set; }
		public double Y { get; set; }

		public uint ID { get; set; }

		public static int NodeRadius = 20;
		public Ellipse ellipse { get; private set; }
		private SolidColorBrush mSelectedBrush;
		private SolidColorBrush mColorBrush;

		public bool IsSource {get; set; }
		public bool IsDestination { get; set; }
		public bool IsVisited { get; set; }
		public bool IsInPath { get; set; }

		private static SolidColorBrush SourceNodeColor = null;
		private static SolidColorBrush DestinationNodeColor = null;
		private static SolidColorBrush VisitedNodeColor = null;
		private static SolidColorBrush PathNodeColor = null;

		public List<Edge> Edges { get; private set; }
		public Node(double x, double y, SolidColorBrush NodeColorBrush, SolidColorBrush selectedColorbrush, Canvas parentCanvas)
		{
			if(SourceNodeColor == null)
			{
				SourceNodeColor = new SolidColorBrush();
				SourceNodeColor.Color = Color.FromRgb(0, 0, 255);

				DestinationNodeColor = new SolidColorBrush();
				DestinationNodeColor.Color = Color.FromRgb(255, 0, 0);

				VisitedNodeColor = new SolidColorBrush();
				VisitedNodeColor.Color = Color.FromRgb(255, 255, 0);

				PathNodeColor = new SolidColorBrush();
				PathNodeColor.Color = Color.FromRgb(0, 255, 0);
			}

			parent = parentCanvas;
			ellipse = new Ellipse();
			ellipse.Fill = NodeColorBrush;
			ellipse.Width = NodeRadius * 2;
			ellipse.Height = ellipse.Width;
			ellipse.StrokeThickness = 1;
			ellipse.Stroke = Brushes.Black;
			Canvas.SetZIndex(ellipse, 2);

			parentCanvas.Children.Add(ellipse);

			displayElement = ellipse;

			mColorBrush = NodeColorBrush;
			mSelectedBrush = selectedColorbrush;

			Edges = new List<Edge>();
			UpdatePosition(x, y);

			SetClickableElement(ellipse);

			IsSource = false;
			IsDestination = false;
			IsVisited = false;
			IsInPath = false;
		}

		public void AddEdge(Edge edge)
		{
			Edges.Add(edge);
		}

		public void UpdatePosition(double x, double y)
		{
			X = x + NodeRadius;
			Y = y + NodeRadius;
			Canvas.SetLeft(ellipse, x);
			Canvas.SetTop(ellipse, y);
			UpdateEdges();
		}

		private void UpdateEdges()
		{
			foreach(Edge edge in Edges)
			{
				edge.UpdateEdge();
			}
		}

		public override void SelectElement()
		{
			ellipse.Fill = mSelectedBrush;
		}

		public override void DeselectElement()
		{
			if ( IsSource )
				ellipse.Fill = SourceNodeColor;
			else if ( IsDestination )
				ellipse.Fill = DestinationNodeColor;
			else if ( IsInPath )
				ellipse.Fill = PathNodeColor;
			else if ( IsVisited )
				ellipse.Fill = VisitedNodeColor;
			else
				ellipse.Fill = mColorBrush;
		}

		public override void Delete()
		{
			while(Edges.Count != 0)
			{
				Edges[0].Delete();
			}
			parent.Children.Remove(ellipse);
		}
	}
}
