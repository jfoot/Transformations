using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
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
		private GridLine Grid;                      //Creates a gird object.
        private const int ScaleFactor = 15; //The Spacing between each grid line/ size of a block                    
        private int MaxValue = 3500;            //The maximum size the grid can be in pixels

        //List of all the shape objects.
        private List<Shapes> MyShapes = new List<Shapes>();         //Shape Object List
        private List<Lines> MyLines = new List<Lines>();            //Free-Form Polygon Lines List
        private List<RayLines> MyRayLines = new List<RayLines>();   //Ray-lines List
        private Counter Counter = new Counter();                    //Counter Object - for counting the number of different shapes.

        //Movement of shapes.
        private bool Dragging;                  //Used to record if a shape is moving or not
        private Point ClickV;                   //Used to record where the user has clicked on a selected shape
        private Shape SelectedShape;                //used to record the currently selected shape
        private bool IsDrawing = false;         //Drawing Free-From Shape
        private bool IsDrawingRays = false;     //Drawing Ray Lines

        //Used for moving around the grid
        private bool CtrlDown = false;          //Used to record if the Ctrl Key is held down
        private bool Ctrldragging = false;      //Used to record if the user is panning around the gird
        private Point ClickX;                   //Used to record the start position of the mouse on the grid

        //Ghost Shapes
        private XmlReader XmlReader;                //Used to store a XML file of a shape
        private string SaveFile;                 //Used to store a save file

        //Declares different mouse cursors     
        private readonly Cursor GrabCursor = new Cursor(new System.IO.MemoryStream(Properties.Strings.grab));
        private readonly Cursor GrabbingCursor = new Cursor(new System.IO.MemoryStream(Properties.Strings.grabbing));

        //Animation Times
        private readonly int[] Times = { 0, 1, 3, 5, 10, 15 };
        //Reflection
        private Line ReflLine = new Line();		//Used to create the reflection line on the canvas
	
		public MainWindow()
		{	
			InitializeComponent();

            //If this is the first time loading the program first try and see if you can find the language of the 
            //user and match it with one of the languages the program is translated to. If it can't find a translation
            //then it will set it to EN English by default. 
            if (!Properties.Settings.Default.IsSetUp)
            {
                Properties.Settings.Default.Language = System.Globalization.CultureInfo.InstalledUICulture.TwoLetterISOLanguageName;
                Properties.Settings.Default.IsSetUp = true;
                Properties.Settings.Default.Save();
            }
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

        private void ProgramLoaded(object sender, RoutedEventArgs e)    //When the program loads open a file if the program launched from a file.
        {
            Labels.IsChecked = true;
            if ((((App)Application.Current).file) != null)
            {
                OpenFunction(((App)Application.Current).file_path);
            }
        }
    }
}