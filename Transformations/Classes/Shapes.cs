using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.IO;
using System.Windows.Markup;
using System.Xml;
using System.Windows;

namespace Transformations
{
    abstract class Shapes    //Declares a shape object
	{
		private TransformGroup myTransformGroup = new TransformGroup();     //Creates a transform group to group the transformations
        private ScaleTransform myScalingTransform = new ScaleTransform();   //Applies a scaling transform
        private RotateTransform myRotateTransform = new RotateTransform();  //Applies a rotation transform
        private TranslateTransform myTranslateTransform = new TranslateTransform(); //Applies a translation rotation
        private Shape myShape;          //Declares the actual shape
        private string partnerShapes;   //Declares the partner shape - used for reflection
        

        public TransformGroup MyTransformGroup
        {
            get { return myTransformGroup; }
            set { myTransformGroup = value; }
        }
        public ScaleTransform MyScalingTransform
        {
            get { return myScalingTransform; }
            set { myScalingTransform = value; }
        }
        public RotateTransform MyRotateTransform
        {
            get { return myRotateTransform; }
            set { myRotateTransform = value; }
        }
        public TranslateTransform MyTranslateTransform
        {
            get { return myTranslateTransform; }
            set { myTranslateTransform = value; }
        }
        public Shape MyShape
        {
            get { return myShape; }
            set { myShape = value; }
        }
        public string PartnerShape
        {
            get { return partnerShapes; }
            set { partnerShapes = value; }
        }
        
		public void SetDefualts(Canvas canvas)
		{
            MyTransformGroup.Children.Add(MyScalingTransform);
            MyTransformGroup.Children.Add(MyRotateTransform);
            MyTransformGroup.Children.Add(MyTranslateTransform);
			MyShape.RenderTransform = MyTransformGroup;

			canvas.Children.Add(MyShape);
		}
    }

    class Circle : Shapes	//A class specifically for the creation of circles  
    {
        public string Name;
		public Circle()		//Creates a circle without a name (default constructor)
		{
            MyShape = new Ellipse
            {
                Fill = new SolidColorBrush(Color.FromArgb(Properties.Settings.Default.DefaultColour.A, Properties.Settings.Default.DefaultColour.R, Properties.Settings.Default.DefaultColour.G, Properties.Settings.Default.DefaultColour.B)),
                Stroke = Brushes.Black,
                StrokeThickness = 2,
                Width = Properties.Settings.Default.DefaultHeight,
                Height = Properties.Settings.Default.DefaultHeight
            };
        }
		public Circle(string name)      //Creates a circle with a known name (default constructor)
		{
			MyShape = new Ellipse
			{
				Fill = new SolidColorBrush(Color.FromArgb(Properties.Settings.Default.DefaultColour.A, Properties.Settings.Default.DefaultColour.R, Properties.Settings.Default.DefaultColour.G, Properties.Settings.Default.DefaultColour.B)),
				Stroke = Brushes.Black,
				StrokeThickness = 2,
				Width = Properties.Settings.Default.DefaultHeight,
				Height = Properties.Settings.Default.DefaultHeight,
			};
            Name = name;   
        }
              
        public Circle MakerSpawn(double position_x, double position_y, Canvas canvas)	//Creates a marker point
        {
            MyShape.Fill = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            MyShape.Width = 10;
            MyShape.Height = 10;
            MyShape.StrokeThickness = 1;
			MyShape.Name = Name;

            SetDefualts(canvas);
            Canvas.SetLeft(MyShape, position_x - 5);
            Canvas.SetTop(MyShape, position_y - 5);
            return this;
        }
        public Shapes SpawnCircle(Canvas canvas)	//Spawns the circle created onto the actual canvas
        {
			MyShape.Name = Name;
			SetDefualts(canvas);
            Canvas.SetTop(MyShape, -Properties.Settings.Default.DefaultHeight);
            Canvas.SetLeft(MyShape, 0);

            return this;
        }
    }

    class FreeForm : Shapes //A class specifically for the creation of polygons (free-forms)  
	{
        public string Name;
		public FreeForm()   //Creates a new polygon shape without a name (default constructor)
		{
            MyShape = new Polygon
            {
                Fill = new SolidColorBrush(Color.FromArgb(Properties.Settings.Default.DefaultColour.A, Properties.Settings.Default.DefaultColour.R, Properties.Settings.Default.DefaultColour.G, Properties.Settings.Default.DefaultColour.B)),
                Stroke = Brushes.Black,
                StrokeThickness = 2
            };
        }
		public FreeForm(string name)    //Creates a new polygon with a known name (default constructor)
		{
			MyShape = new Polygon
			{
				Fill = new SolidColorBrush(Color.FromArgb(Properties.Settings.Default.DefaultColour.A, Properties.Settings.Default.DefaultColour.R, Properties.Settings.Default.DefaultColour.G, Properties.Settings.Default.DefaultColour.B)),
				Stroke = Brushes.Black,
				StrokeThickness = 2,
				Name = name
            };
            Name = name;
        }
      
        //Creates a ghost shape, of a known original shape
        public Shapes WrongGhost(byte R, byte G, byte B, Polygon ownerPolygon, Canvas canvas)
        {
			ownerPolygon.Name = "WrongGhost";
			XmlReader xmlReader = XmlReader.Create(new StringReader(XamlWriter.Save(ownerPolygon)));
            MyShape = (Polygon)XamlReader.Load(xmlReader);    //Create a new polygon with the properties of the original
			MyShape.Fill = Properties.Settings.Default.DefaultPerformance == true ? new SolidColorBrush(Color.FromArgb(100, R, G, B)) : new SolidColorBrush(Color.FromArgb(255, R, G, B));   //If the grid would be smaller then size available then force the grid to be bigger.
            SetDefualts(canvas);
            return this;
        }
        //Creates a reflection ghost shape, this will generate a ghost from a selection of points.
        public Shapes ReflectionGhost(PointCollection points, byte R, byte G, byte B, Canvas canvas, string Partner) //Reusable Function for drawing of polygon shapes.
        {
            (MyShape as Polygon).Points = points;
            MyShape.Fill = Properties.Settings.Default.DefaultPerformance == true ? new SolidColorBrush(Color.FromArgb(100, R, G, B)) : new SolidColorBrush(Color.FromArgb(255, R, G, B));   //If the grid would be smaller then size available then force the grid to be bigger.
            PartnerShape = Partner;
			MyShape.Name = Name;
			SetDefualts(canvas);
            return this;
        }
        //Creates a new custom shape from a selection of known points
        public Shapes SpawnCustomShape(PointCollection points, Canvas canvas) //Reusable Function for drawing of polygon shapes.
        {
            ((MyShape) as Polygon).Points = points;
			MyShape.Name = Name;
			SetDefualts(canvas);
            Canvas.SetTop(MyShape, -Properties.Settings.Default.DefaultHeight);
            Canvas.SetLeft(MyShape, 0);
            return this;
        }
    }

    class Ghost : Shapes //A class specifically for the creation of ghost shapes  
	{
        public string Name;
        public Ghost(string name) { Name = name;}
        public Ghost() { }

        public Shapes SpawnGhostShape(byte R, byte G, byte B, Shape selectedShape, Canvas canvas, bool ghostVisibality) //A reusable function for spawning Ellipse ghost shapes (transformed shapes)
        {
            if (selectedShape.GetType().ToString() == "System.Windows.Shapes.Polygon")    //If shape is a polygon
            {
                XmlReader xmlReader = XmlReader.Create(new StringReader(XamlWriter.Save(selectedShape)));
                MyShape = (Polygon)XamlReader.Load(xmlReader);    //Create a new polygon with the properties of the original
            }
            else
            {
                XmlReader xmlReader = XmlReader.Create(new StringReader(XamlWriter.Save(selectedShape)));
                MyShape = (Ellipse)XamlReader.Load(xmlReader);    //Create a new polygon with the properties of the original
            }


            MyShape.Fill = Properties.Settings.Default.DefaultPerformance == true ? new SolidColorBrush(Color.FromArgb(100, R, G, B)) : new SolidColorBrush(Color.FromArgb(255, R, G, B));   //If the grid would be smaller then size available then force the grid to be bigger.
            MyShape.Effect = null;
            MyShape.StrokeThickness = 2;
            MyShape.Name = Name;

            if (ghostVisibality == false)
                MyShape.Visibility = Visibility.Hidden;

            SetDefualts(canvas);
            return this;
        }
    }


	class Lines // Declares a line object, this will be used for free-form drawing.
	{
		private List<Line> linesList = new List<Line>();    //Declares a list of lines
		private PointCollection myPoints = new PointCollection();   //Declares a point collection, to retrieve the points from the lines.

		public List<Line> LinesList
		{
			get { return linesList; }
			set { linesList = value; }
		}
        public PointCollection MyPoints
		{
			get { return myPoints; }
			set { myPoints = value; }
		}
	}
	class GridLine  //Declares a grid-line object
	{   //Declares a variety of different line lists, used for different parts of the grid.
		private List<Line> xGridline = new List<Line>();
		private List<Line> yGridline = new List<Line>();
		private List<Line> axis = new List<Line>();
		private List<Line> axisLines = new List<Line>();
		private List<Label> labels = new List<Label>();

		
		public List<Line> XGridline
		{
			get { return xGridline; }
			set { xGridline = value; }
		}
		public List<Line> YGridline
		{
			get { return yGridline; }
			set { yGridline = value; }
		}
		public List<Line> Axis
		{
			get { return axis; }
			set { axis = value; }
		}
		public List<Line> AxisLines
		{
			get { return axisLines; }
			set { axisLines = value; }
		}
		public List<Label> Labels
		{
			get { return labels; }
			set { labels = value; }
		}

		//Used to draw the grid.
		public GridLine DrawGrid(int max_value, int scale_factor, Canvas canvas) //Draws the grid
		{
			//Declares the variables which control the colours of the grids
			SolidColorBrush TextColour = Properties.Settings.Default.DarkMode == true ? new SolidColorBrush(Color.FromArgb(255, 255, 255, 255)) : new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
			SolidColorBrush AxisColour = Properties.Settings.Default.DarkMode == true ? new SolidColorBrush(Color.FromArgb(255, 255, 255, 255)) : new SolidColorBrush(Color.FromArgb(255, 128, 128, 128));
			//Declares variables used for counting and indexing.
			int x = 0;
			int y = 0;
     
            while (x < max_value) // Used to draw lines from the center out to the far right.
			{
				YGridline.Add(new Line() {
					Stroke = new SolidColorBrush(Color.FromArgb(Properties.Settings.Default.DefaultGridColour.A, Properties.Settings.Default.DefaultGridColour.R,
					Properties.Settings.Default.DefaultGridColour.G, Properties.Settings.Default.DefaultGridColour.B)),
					StrokeThickness = 2,
					X1 = x,
					Y1 = -max_value,
					X2 = x,
					Y2 = max_value
				});
                canvas.Children.Add(YGridline[YGridline.Count - 1]);
                if (x / scale_factor % 5 == 0)
				{
					YGridline[YGridline.Count -1].StrokeThickness = 4;
                    labels.Add(new Label() { Content = (x / scale_factor), Foreground = TextColour });
					Canvas.SetLeft(Labels[Labels.Count - 1], x);
					Canvas.SetTop(Labels[Labels.Count - 1], 0);

                    AxisLines.Add(new Line() { X1 = x, X2 = x, Y1 = 0, Y2 = (scale_factor/2), Stroke = AxisColour, StrokeThickness = 2 });
                    canvas.Children.Add(AxisLines[AxisLines.Count - 1]);
                }
				x += scale_factor;
			}
			x = 0;
            y = 0;
            while (x > -max_value) //used to draw lines from the center to the far left.
			{
				YGridline.Add(new Line() {StrokeThickness = 2,
					Stroke = new SolidColorBrush(Color.FromArgb(Properties.Settings.Default.DefaultGridColour.A, Properties.Settings.Default.DefaultGridColour.R,
					Properties.Settings.Default.DefaultGridColour.G, Properties.Settings.Default.DefaultGridColour.B)) ,
					X1 = x ,
					Y1 = -max_value,
					X2 = x,
					Y2 = max_value
				});
                canvas.Children.Add(YGridline[YGridline.Count - 1]);
                if (x / scale_factor % 5 == 0)
				{
					YGridline[YGridline.Count - 1].StrokeThickness = 4;
                    Labels.Add(new Label() { Content = (x / scale_factor), Foreground = TextColour });
					Canvas.SetLeft(Labels[Labels.Count - 1], x);
					Canvas.SetTop(Labels[Labels.Count - 1], 0);
                     
                    AxisLines.Add(new Line() { X1 = x, X2 = x, Y1 = 0, Y2 = (scale_factor / 2), Stroke = AxisColour, StrokeThickness = 2  });
	                canvas.Children.Add(AxisLines[AxisLines.Count - 1]);
				}
				x -= scale_factor;
			}
            y = 0;
            x = 0;
            while (y < max_value)   //used to draw lines from the center to the bottom
			{
				XGridline.Add(new Line() {
					Stroke = new SolidColorBrush(Color.FromArgb(Properties.Settings.Default.DefaultGridColour.A, Properties.Settings.Default.DefaultGridColour.R,
					Properties.Settings.Default.DefaultGridColour.G, Properties.Settings.Default.DefaultGridColour.B)),
					StrokeThickness = 2,
					X1 = -max_value,
					Y1 = y,
					X2 = max_value,
					Y2 = y
				});
                canvas.Children.Add(XGridline[XGridline.Count - 1]);
                if (y / scale_factor % 5 == 0)
				{
					XGridline[XGridline.Count - 1].StrokeThickness = 4;
					Labels.Add(new Label() { Content = -(y / scale_factor), Foreground = TextColour });
					Canvas.SetLeft(Labels[Labels.Count - 1], x);
					Canvas.SetTop(Labels[Labels.Count - 1], y);
             
                    AxisLines.Add(new Line() { X1 = 0, X2 = (scale_factor/2), Y1 = y, Y2 = y, Stroke = AxisColour, StrokeThickness = 2 });
                    canvas.Children.Add(AxisLines[AxisLines.Count - 1]);
                }
				y += scale_factor;
			}
			y = 0;
            x = 0;
			while (y > -max_value)  //Used to draw lines from the center to the top.
			{
				XGridline.Add(new Line() {
					Stroke = new SolidColorBrush(Color.FromArgb(Properties.Settings.Default.DefaultGridColour.A, Properties.Settings.Default.DefaultGridColour.R,
					Properties.Settings.Default.DefaultGridColour.G, Properties.Settings.Default.DefaultGridColour.B)),
					StrokeThickness = 2,
					X1 = -max_value,
					Y1 = y,
					X2 = max_value,
					Y2 = y
				});
                canvas.Children.Add(XGridline[XGridline.Count - 1]);
                if (y / scale_factor % 5 == 0)
				{
					XGridline[XGridline.Count - 1].StrokeThickness = 4;
                    Labels.Add(new Label() { Content = -(y / scale_factor), Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0)) });
					Canvas.SetLeft(Labels[Labels.Count - 1], 0);
					Canvas.SetTop(Labels[Labels.Count - 1], y);

                    AxisLines.Add(new Line() { X1 = 0, X2 = (scale_factor/2), Y1 = y, Y2 = y, Stroke = AxisColour, StrokeThickness = 2});
                    canvas.Children.Add(AxisLines[AxisLines.Count -1]);
                }
                y -= scale_factor;
			}
            //VERTICLE LINE- Y Axis
            Axis.Add(new Line() { Stroke = AxisColour, StrokeThickness = 3, X1 = 0, Y1 = -max_value, X2 = 0, Y2 = max_value });
            canvas.Children.Add(Axis[0]);
            
            //Horizontal line - X Axis
            Axis.Add(new Line() { Stroke = AxisColour, StrokeThickness = 3, X1 = -max_value, Y1 = 0, X2 = max_value, Y2 = 0 });
            canvas.Children.Add(Axis[1]);
            return this;
        }
	}
	class RayLines  //Declares a ray-line object
	{   //Ray-lines are objectised so the ray-lines object can be encapsulated inside a list.
		private List<Line> raylinesList = new List<Line>(); //a list containing lines
		public List<Line> RayLinesList
		{
			get { return raylinesList; }
			set { raylinesList = value; }
		}
	}
}

