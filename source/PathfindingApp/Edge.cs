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
	public class Edge : GraphElement
	{

		public Node mFromNode { get; private set; }
		public Node mToNode { get; private set; }

		public Path mArrowPath { get; private set; }
		private PathGeometry mArrowPathGeometry;
		public Line edgeLine { get; private set; }

		public uint EdgeWeight { get; set; }

		private SolidColorBrush mSelectedBrush;

		public Edge(Node from, Node to, uint edgeWeight, SolidColorBrush selectedColorbrush, Canvas parentCanvas)
		{
			parent = parentCanvas;
			mFromNode = from;
			mToNode = to;

			mSelectedBrush = selectedColorbrush;

			mFromNode.AddEdge(this);
			mToNode.AddEdge(this);

			edgeLine = new Line();
			edgeLine.Stroke = Brushes.Black;
			edgeLine.StrokeThickness = 4;

			Canvas.SetZIndex(edgeLine, 1);

			displayElement = edgeLine;

			mArrowPathGeometry = new PathGeometry();
			mArrowPath = new Path();
			mArrowPath.StrokeThickness = 4;
			mArrowPath.Stroke = mArrowPath.Fill = Brushes.Black;

			UpdateEdge();
			SetClickableElement(edgeLine);

			parent.Children.Add(edgeLine);
			parent.Children.Add(mArrowPath);

			EdgeWeight = edgeWeight;
		}

		public void UpdateEdge()
		{
			edgeLine.X1 = mFromNode.X;
			edgeLine.Y1 = mFromNode.Y;

			edgeLine.X2 = mToNode.X;
			edgeLine.Y2 = mToNode.Y;

			UpdateArrow(new Point(edgeLine.X1, edgeLine.Y1), new Point(edgeLine.X2, edgeLine.Y2));
		}

		private void UpdateArrow(Point p1, Point p2)
		{
			double theta = Math.Atan2((p2.Y - p1.Y), (p2.X - p1.X)) * 180 / Math.PI;

			PathGeometry pathGeometry = mArrowPathGeometry;
			pathGeometry.Figures.Clear();
			PathFigure pathFigure = new PathFigure();
			Point p = new Point(p1.X + ((p2.X - p1.X) / 1.35), p1.Y + ((p2.Y - p1.Y) / 1.35));
			pathFigure.StartPoint = p;

			Point lpoint = new Point(p.X + 6, p.Y + 15);
			Point rpoint = new Point(p.X - 6, p.Y + 15);
			LineSegment seg1 = new LineSegment();
			seg1.Point = lpoint;
			pathFigure.Segments.Add(seg1);

			LineSegment seg2 = new LineSegment();
			seg2.Point = rpoint;
			pathFigure.Segments.Add(seg2);

			LineSegment seg3 = new LineSegment();
			seg3.Point = p;
			pathFigure.Segments.Add(seg3);

			pathGeometry.Figures.Add(pathFigure);
			RotateTransform transform = new RotateTransform();
			transform.Angle = theta + 90;
			transform.CenterX = p.X;
			transform.CenterY = p.Y;
			pathGeometry.Transform = transform;

			mArrowPath.Data = pathGeometry;
		}

		public override void SelectElement()
		{
			edgeLine.Stroke = mSelectedBrush;
		}

		public override void DeselectElement()
		{
			edgeLine.Stroke = Brushes.Black;
		}

		public override void Delete()
		{
			mFromNode.Edges.Remove(this);
			mToNode.Edges.Remove(this);

			parent.Children.Remove(mArrowPath);
			parent.Children.Remove(edgeLine);
		}
	}
}
