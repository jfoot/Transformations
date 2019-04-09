using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Xml;
using WPFLocalizeExtension.Engine;
using Point = System.Windows.Point;

namespace Transformations
{
    public partial class MainWindow : Window
	{
		//Grid Variables
		GridLine Grid;						//Creates a gird object.
		public const int ScaleFactor = 15;	//The Spacing between each grid line/ size of a block                    
		public int MaxValue = 3500;			//The maximum size the grid can be in pixels

		//List of all the shape objects.
		List<Shapes> MyShapes = new List<Shapes>();			//Shape Object List
		List<Lines> MyLines = new List<Lines>();			//Free-Form Polygon Lines List
		List<RayLines> MyRayLines = new List<RayLines>();	//Ray-lines List
		Counter Counter = new Counter();					//Counter Object - for counting the number of different shapes.

		//Movement of shapes.
		public bool Dragging;					//Used to record if a shape is moving or not
		public Point ClickV;					//Used to record where the user has clicked on a selected shape
		public Shape SelectedShape;				//used to record the currently selected shape
		public bool IsDrawing = false;			//Drawing Free-From Shape
		public bool IsDrawingRays = false;		//Drawing Ray Lines
		
		//Used for moving around the grid
		public bool CtrlDown = false;			//Used to record if the Ctrl Key is held down
		public bool Ctrldragging = false;		//Used to record if the user is panning around the gird
		public Point ClickX;					//Used to record the start position of the mouse on the grid

		//Ghost Shapes
		public XmlReader XmlReader;				//Used to store a XML file of a shape
		public string SaveFile;					//Used to store a save file
		//Declares different mouse cursors 
	    
        //public readonly Cursor GrabCursor = new Cursor(new System.IO.MemoryStream(LocalizationProvider.GetLocalizedValue<int>("grab")));
		//public readonly Cursor GrabbingCursor = new Cursor(new System.IO.MemoryStream(LocalizationProvider.GetLocalizedValue<int>("grabbing")));

		//Animation Times
		public int[] Times = { 0, 1, 3, 5, 10, 15 };
		//Reflection
		public Line ReflLine = new Line();		//Used to create the reflection line on the canvas
	
		public MainWindow()
		{	
			InitializeComponent();
            LocalizeDictionary.Instance.Culture = new System.Globalization.CultureInfo(Properties.Settings.Default.Language);

            SplashScreen splash = new SplashScreen("splash_screen.png");	//Creates a start up splash screen
			splash.Show(true, true);
			//Checks the database connection on startup.
			System.Data.OleDb.OleDbConnection conn = new System.Data.OleDb.OleDbConnection();
			try
			{
				conn.ConnectionString = DataBase.ConnectionString();
				conn.Open();
			}
			catch (Exception)
			{
				DatabaseError.Visibility = Visibility.Visible;	//If no connection can be established display an error
			}
			finally
			{
				conn.Close();
			}

			//Change the width and height of the application based upon the monitors resolution
			this.Width = Properties.Settings.Default.DefaultResolution == false ? 850 : 1100;   
			this.Height = Properties.Settings.Default.DefaultResolution == false ? 600 : 900;

			//Set the animation frame rate depending upon the graphics setting
			try { if (!Properties.Settings.Default.DefaultPerformance)
					Timeline.DesiredFrameRateProperty.OverrideMetadata(typeof(Timeline), new FrameworkPropertyMetadata { DefaultValue = 15 }); } catch (Exception){}

           // Updates the UI upon start up.

            accountName.Content = Properties.Settings.Default.AliasName;
            version.Content = String.Format(Properties.Strings.CreatedBy + " Jonathan Foot 2019©    " + Properties.Strings.ver + " " + Assembly.GetExecutingAssembly().GetName().Version.ToString());

            if (Properties.Settings.Default.CurrentUser == Properties.Strings.Guest)
            {
                Login.Header = Properties.Strings.StudentLoginDrop;
                Teacher.Header = Properties.Strings.TeacherLoginDrop;
            }
            else if (Properties.Settings.Default.CurrentUser != Properties.Strings.Guest && Properties.Settings.Default.IsTeacher == true)
            {
                Login.Header = Properties.Strings.StudentLoginDrop;
                Teacher.Header = Properties.Strings.TeacherLogOutDrop;
            }
            else if (Properties.Settings.Default.CurrentUser != Properties.Strings.Guest && Properties.Settings.Default.IsTeacher == false)
            {
                Login.Header = Properties.Strings.StudentLogOutDrop;
                Teacher.Header = Properties.Strings.TeacherLoginDrop;
            }
			if (Properties.Settings.Default.DarkMode)	//Sets the background colour
				border.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 31, 31, 31));
		}
		private void CanvasLoaded(object sender, RoutedEventArgs e) //Called upon when the canvas is loaded
		{
			if (Properties.Settings.Default.DefaultPerformance) //Change the size of the grid based upon performance settings
			{	//Then ensures the grid will definitely fill the whole screen.
				MaxValue = border.ActualWidth > 750 ? Convert.ToInt32(border.ActualWidth) : 750;  
			}
			else
			{
				MaxValue = border.ActualWidth/2 > 750 ? Convert.ToInt32(border.ActualWidth/2) : 750;
			}

			//Sets the scaling of the application
			XSlider.Maximum = MaxValue - ((border.ActualWidth / 2) / sliderSf.Value);
			XSlider.Minimum = -MaxValue + ((border.ActualWidth / 2) / sliderSf.Value);
			YSlider.Maximum = MaxValue - ((border.ActualHeight / 2) / sliderSf.Value);
			YSlider.Minimum = -MaxValue + ((border.ActualHeight / 2) / sliderSf.Value);
			Scaling.Main(TranslationTransformCanvas, scaleTransformCanvas, XSlider, YSlider, sliderSf, border);
			
			//Draws the grid and labels 
			Grid = new GridLine().DrawGrid(MaxValue, ScaleFactor, MyCanvas);
			LabelsChecked(sender, e);
		}

    }
}