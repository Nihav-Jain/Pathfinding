using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.IO;
using Pathfinding;

namespace PathfindingApp
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private SolidColorBrush NodeColorBrush;
		private SolidColorBrush NodeSelectedColorBrush;

		private Ellipse selectedEllipse;
		private bool bIsDragging;

		private Dictionary<Ellipse, Node> Nodes;
		private List<Node> allNodes;
		private Node firstSelected;
		private Node secondSelected;
		private Edge selectedEdge;

		private List<Edge> allEdges;

		private Stopwatch watch;

		private static string FILE_EXTENTION = ".graph";
		private static uint DEFAULT_EDGE_WEIGHT = 1;

		private GraphCLI graph;

		private static object sLockObject;
		private string loadedFileName;

		private Node SourceNode;
		private Node DestinationNode;

		private Dictionary<string, IPathfinderCLI> PathFindingAlgos;

		private double dragOffsetX;
		private double dragOffsetY;

		public MainWindow()
		{
			InitializeComponent();

			graph = new GraphCLI();

			NodeColorBrush = new SolidColorBrush();
			NodeColorBrush.Color = Color.FromArgb(255, 0, 255, 255);

			NodeSelectedColorBrush = new SolidColorBrush();
			NodeSelectedColorBrush.Color = Color.FromArgb(255, 255, 128, 0);

			Nodes = new Dictionary<Ellipse, Node>();
			allNodes = new List<Node>();
			selectedEllipse = null;
			firstSelected = null;
			secondSelected = null;
			selectedEdge = null;

			allEdges = new List<Edge>();

			watch = new Stopwatch();
			bIsDragging = false;
			dragOffsetX = 0;
			dragOffsetY = 0;

			MouseUp += MainWindow_MouseUp;
			MouseMove += MainWindow_MouseMove;

			sLockObject = new object();
			loadedFileName = string.Empty;

			SourceNode = null;
			DestinationNode = null;

			PathFindingAlgos = new Dictionary<string, IPathfinderCLI>();
			PathFindingAlgos.Add("Breadth First Search", new BreadthFirstPathfinderCLI());
			PathFindingAlgos.Add("Greedy Best First", new GreedyBestFirstPathfinderCLI());
			PathFindingAlgos.Add("Dijkstras", new DijkstrasPathfinderCLI());
			PathFindingAlgos.Add("AStar", new AStarPathfinderCLI());

			foreach(string key in PathFindingAlgos.Keys)
			{
				pathfindingAlgoCombobox.Items.Add(key);
			}
			pathfindingAlgoCombobox.SelectedIndex = 0;
		}

		private void MainWindow_MouseMove(object sender, MouseEventArgs e)
		{
			if ( bIsDragging && selectedEllipse != null)
			{
				Nodes[selectedEllipse].UpdatePosition(e.GetPosition(canvas).X - dragOffsetX, e.GetPosition(canvas).Y - dragOffsetY);
			}
		}

		private void MainWindow_MouseUp(object sender, MouseButtonEventArgs e)
		{
			bIsDragging = false;
			selectedEllipse = null;
			dragOffsetX = 0;
			dragOffsetY = 0;
		}

		private void DrawBtn_Click(object sender, RoutedEventArgs e)
		{
			CreateNewNode(canvas.Width / 2, canvas.Height / 2);
		}

		private void CreateNewNode(double x, double y)
		{
			Node node = new Node(x, y, NodeColorBrush, NodeSelectedColorBrush, canvas);
			Nodes.Add(node.ellipse, node);
			allNodes.Add(node);

			node.MouseClicked += Node_MouseClicked;
			node.ellipse.MouseDown += Ellipse_MouseDown;
		}

		private void Node_MouseClicked(object sender, ClickEventArgs e)
		{
			SelectNode(sender as Node);
		}

		private void Ellipse_MouseDown(object sender, MouseButtonEventArgs e)
		{
			bIsDragging = true;
			selectedEllipse = sender as Ellipse;

			dragOffsetX = e.GetPosition(selectedEllipse).X;
			dragOffsetY = e.GetPosition(selectedEllipse).Y;
		}

		private void SelectNode(Node clickedNode)
		{
			bool selectionStatus = clickedNode.ToggleNodeSelection();

			if(selectionStatus)
			{
				if ( firstSelected == null )
					firstSelected = clickedNode;
				else if ( secondSelected == null )
					secondSelected = clickedNode;
				else
				{
					firstSelected.ToggleNodeSelection();
					secondSelected.ToggleNodeSelection();

					firstSelected = clickedNode;
					secondSelected = null;
				}
			}
			else
			{
				if(clickedNode == firstSelected)
				{
					firstSelected = secondSelected;
					secondSelected = null;
				}
				else if(clickedNode == secondSelected)
				{
					secondSelected = null;
				}
			}
		}

		private void EdgeBtn_Click(object sender, RoutedEventArgs e)
		{
			if(firstSelected == null || secondSelected == null)
			{
				MessageBox.Show("Please select 2 nodes to create an edge between them");
				return;
			}

			Node tailNode = firstSelected;
			Node headNode = secondSelected;
			var existingEdge = from edge in allEdges where (edge.mFromNode == tailNode && edge.mToNode == headNode) select edge;

			if(existingEdge.Count<Edge>() > 0)
			{
				MessageBox.Show("Edge already exists");
				return;
			}
			CreateNewEdge(tailNode, headNode, DEFAULT_EDGE_WEIGHT);
		}

		private void CreateNewEdge(Node tail, Node head, uint edgeWeight)
		{
			Edge newEdge = new Edge(tail, head, edgeWeight, NodeSelectedColorBrush, canvas);
			allEdges.Add(newEdge);
			newEdge.MouseClicked += Edge_MouseClicked;
		}

		private void Edge_MouseClicked(object sender, ClickEventArgs e)
		{
			if ( selectedEdge != null && selectedEdge != sender)
				selectedEdge.ToggleNodeSelection();

			bool selected = (sender as GraphElement).ToggleNodeSelection();
			if ( selected )
			{
				selectedEdge = sender as Edge;
				EdgeWeightText.Text = selectedEdge.EdgeWeight.ToString();
			}
			else
			{
				selectedEdge = null;
				EdgeWeightText.Text = string.Empty;
			}
		}

		private void LoadBtn_Click(object sender, RoutedEventArgs e)
		{
			string filePath = FolderChooser();
			PrintToConsole($"File Loaded: {filePath}");
			if ( filePath.Length > 0 )
			{
				if ( filePath.LastIndexOf(FILE_EXTENTION) > -1 )
				{
					Clear();
					StreamReader reader = new StreamReader(filePath);

					int totalNodes = Convert.ToInt32(reader.ReadLine());
					loadedFileName = filePath;
					uint i, j;
					for(i=0;i<totalNodes;i++ )
					{
						string[] position = reader.ReadLine().Split(' ');
						CreateNewNode(Convert.ToDouble(position[0]), Convert.ToDouble(position[1]));
					}
					while(!reader.EndOfStream)
					{
						string[] edgeData = reader.ReadLine().Split(' ');
						if ( edgeData.Length < 3 )
							continue;

						int fromIndex = Convert.ToInt32(edgeData[0]);
						int toIndex = Convert.ToInt32(edgeData[1]);
						uint edgeWeight = Convert.ToUInt32(edgeData[2]);

						CreateNewEdge(allNodes[fromIndex], allNodes[toIndex], edgeWeight);
					}
					reader.Close();
				}
				else
				{
					MessageBox.Show($"Invalid file. Choose a file with extention {FILE_EXTENTION}");
				}
			}
		}

		private string FolderChooser()
		{
			System.Windows.Forms.OpenFileDialog dialog = new System.Windows.Forms.OpenFileDialog();
			System.Windows.Forms.DialogResult result = dialog.ShowDialog();
			if ( result == System.Windows.Forms.DialogResult.OK )
				return dialog.FileName;
			return string.Empty;
		}

		private void PrintToConsole(string message)
		{
			StringBuilder strBuilder;

			lock ( sLockObject )
			{
				strBuilder = new StringBuilder(consoleTextBlock.Text);
			}

			strBuilder.Append(message);
			strBuilder.Append("\n\n********\n\n");

			lock ( sLockObject )
			{
				consoleTextBlock.Text = strBuilder.ToString();
			}
		}

		private void FindPathBtn_Click(object sender, RoutedEventArgs e)
		{
			if(SourceNode == null)
			{
				MessageBox.Show("Please select a source node");
				return;
			}
			if ( DestinationNode == null )
			{
				MessageBox.Show("Please select a destination node");
				return;
			}
			string selectedAlgoName = pathfindingAlgoCombobox.SelectedValue as string;
			if(!PathFindingAlgos.ContainsKey(selectedAlgoName))
			{
				MessageBox.Show("Invalid Algorithm");
				return;
			}

			GenerateGraph();
			List<uint> visited = new List<uint>();
			List<uint> path = PathFindingAlgos[selectedAlgoName].FindPath(graph, SourceNode.ID, DestinationNode.ID, visited);
			PrintPathAndVisited(path, visited, selectedAlgoName);

			DeselectAll();

			foreach(uint visitedNodeId in visited)
			{
				allNodes[(int)visitedNodeId].IsVisited = true;
			}
			foreach(uint pathNodeId in path)
			{
				allNodes[(int)pathNodeId].IsInPath = true;
				allNodes[(int)pathNodeId].IsVisited = false;
			}

			SourceNode.IsVisited = false;
			SourceNode.IsInPath = false;
			DestinationNode.IsVisited = false;
			DestinationNode.IsInPath = false;

			foreach(Node node in allNodes)
			{
				node.DeselectElement();
			}

			if ( path.Count == 0 )
				MessageBox.Show("No possible path.");
		}
		private void PrintPathAndVisited(List<uint> path, List<uint> visited, string algoName)
		{
			PrintToConsole(algoName);
			StringBuilder pathStr = new StringBuilder();
			foreach ( uint node in path )
			{
				pathStr.Append($"{node} ->");
			}
			pathStr.Append("\n");
			PrintToConsole(pathStr.ToString());

			pathStr = new StringBuilder(string.Empty);
			foreach ( uint node in visited )
			{
				pathStr.Append($"{node}, ");
			}
			pathStr.Append("\n");
			PrintToConsole(pathStr.ToString());
		}

		private void DeleteBtn_Click(object sender, RoutedEventArgs e)
		{
			List<GraphElement> selectedList = new List<GraphElement>();
			if ( firstSelected != null )
				selectedList.Add(firstSelected);
			if ( secondSelected != null )
				selectedList.Add(secondSelected);
			if ( selectedEdge != null )
				selectedList.Add(selectedEdge);

			firstSelected = null;
			secondSelected = null;
			selectedEdge = null;

			if(selectedList.Count > 1)
			{
				MessageBoxButton btnMessageBox = MessageBoxButton.YesNo;
				MessageBoxImage icnMessageBox = MessageBoxImage.Warning;

				MessageBoxResult rsltMessageBox = MessageBox.Show("Multiple items selected. Are you sure you want to delete all?", "Delete multiple?", btnMessageBox, icnMessageBox);
				if(rsltMessageBox == MessageBoxResult.Yes)
					DeleteElements(selectedList);
			}
			else
			{
				DeleteElements(selectedList);
			}
		}

		private void DeleteElements(List<GraphElement> selectedList)
		{
			foreach ( GraphElement element in selectedList )
			{
				element.Delete();
				if ( element is Node )
				{
					Nodes.Remove((element as Node).ellipse);
					allNodes.Remove(element as Node);
				}
				else if ( element is Edge )
					allEdges.Remove(element as Edge);
			}
		}

		private void SetSourceBtn_Click(object sender, RoutedEventArgs e)
		{
			if(firstSelected == null)
			{
				MessageBox.Show("Please select a node to set as source");
				return;
			}

			if (SourceNode != null)
			{
				SourceNode.IsSource = false;
				if ( SourceNode.IsSelected )
					SourceNode.ToggleNodeSelection();
				else
					SourceNode.DeselectElement();
			}

			firstSelected.IsSource = true;
			firstSelected.IsDestination = false;
			firstSelected.ToggleNodeSelection();
			if ( DestinationNode == firstSelected )
				DestinationNode = null;
			SourceNode = firstSelected;

			firstSelected = null;
			if ( secondSelected != null )
				secondSelected.ToggleNodeSelection();
			secondSelected = null;
		}

		private void SetDestBtn_Click(object sender, RoutedEventArgs e)
		{
			if(firstSelected == null)
			{
				MessageBox.Show("Please select a node to set as destination");
				return;
			}

			if (DestinationNode != null)
			{
				DestinationNode.IsDestination = false;
				if ( DestinationNode.IsSelected )
					DestinationNode.ToggleNodeSelection();
				else
					DestinationNode.DeselectElement();
			}

			firstSelected.IsSource = false;
			firstSelected.IsDestination = true;
			firstSelected.ToggleNodeSelection();
			if ( SourceNode == firstSelected )
				SourceNode = null;
			DestinationNode = firstSelected;

			firstSelected = null;
			if ( secondSelected != null )
				secondSelected.ToggleNodeSelection();
			secondSelected = null;
		}

		private void SaveBtn_Click(object sender, RoutedEventArgs e)
		{
			if ( string.IsNullOrEmpty(loadedFileName) )
				SaveAsBtn_Click(sender, e);
			else
			{
				SaveGraphData(loadedFileName);
			}
			MessageBox.Show("Graph Saved");
		}

		private void SaveAsBtn_Click(object sender, RoutedEventArgs e)
		{
			string filePath = SaveFileDialogue();
			PrintToConsole(filePath);

			int extensionIndex = filePath.LastIndexOf('.');
			if(filePath.Substring(extensionIndex) != FILE_EXTENTION)
			{
				filePath = $"{filePath.Substring(0, extensionIndex)}{FILE_EXTENTION}";
			}

			SaveGraphData(filePath);
			loadedFileName = filePath;
			PrintToConsole("Graph Saved");
		}

		private void SaveEdgeWeightBtn_Click(object sender, RoutedEventArgs e)
		{
			if(selectedEdge == null)
			{
				MessageBox.Show("Please select an edge to change its weight");
				return;
			}

			uint edgeWeight;
			bool wasParsed = uint.TryParse(EdgeWeightText.Text, out edgeWeight);

			if ( wasParsed )
			{
				selectedEdge.EdgeWeight = edgeWeight;
				MessageBox.Show("Weight Updated");
			}
			else
			{
				MessageBox.Show("Invalid value. Edge weight must be a positive integer");
			}
		}

		private string SaveFileDialogue()
		{
			Microsoft.Win32.SaveFileDialog dialogue = new Microsoft.Win32.SaveFileDialog();
			dialogue.DefaultExt = FILE_EXTENTION;
			bool? result = dialogue.ShowDialog();

			if ( result == true )
				return dialogue.FileName;
			else
				return string.Empty;
		}

		private void SaveGraphData(string filePath)
		{
			StreamWriter writer = new StreamWriter(filePath);
			writer.WriteLine(Nodes.Count.ToString());

			uint i = 0;
			foreach ( Node node in allNodes )
			{
				node.ID = i;
				i++;

				writer.WriteLine($"{node.X} {node.Y}");
			}
			foreach ( Node node in allNodes )
			{
				foreach ( Edge edge in node.Edges )
				{
					if ( edge.mFromNode == node )
						writer.WriteLine($"{node.ID} {edge.mToNode.ID} {edge.EdgeWeight}");
				}
			}
			writer.Close();
		}

		private void Clear()
		{
			allNodes.Clear();
			allEdges.Clear();
			Nodes.Clear();
			canvas.Children.Clear();
			bIsDragging = false;
			selectedEdge = null;
			selectedEllipse = null;
			firstSelected = null;
			secondSelected = null;
		}

		private void GenerateGraph()
		{
			graph.Clear();

			VertexDataCLI vertexData = new VertexDataCLI();
			foreach(Node node in allNodes)
			{
				vertexData.PosX((int)node.X);
				vertexData.PosY((int)node.Y);
				node.ID = graph.AddVertex(vertexData);
			}

			foreach(Node node in allNodes)
			{
				foreach(Edge edge in node.Edges)
				{
					if(edge.mFromNode == node)
					{
						EdgeDataCLI edgeData = new EdgeDataCLI(edge.EdgeWeight);
						graph.CreateEdge(edge.mToNode.ID, edge.mFromNode.ID, edgeData);
					}
				}
			}
		}

		private void DeselectAll()
		{
			if ( secondSelected != null )
				secondSelected.ToggleNodeSelection();
			if ( firstSelected != null )
				firstSelected.ToggleNodeSelection();
			if ( selectedEdge != null )
				selectedEdge.ToggleNodeSelection();

			firstSelected = null;
			secondSelected = null;
			selectedEdge = null;

			foreach(Node node in allNodes)
			{
				node.IsVisited = false;
				node.IsInPath = false;
				node.DeselectElement();
			}
		}
	}

}

