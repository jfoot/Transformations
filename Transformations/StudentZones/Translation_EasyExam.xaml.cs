using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using System.Linq;


namespace Transformations
{
	/// <summary>
	/// Contains all the code for an easy translation exam, which randomly generates 2 shapes, the user then has a multiple choice selection to find the translation vector.
	/// </summary>
	public partial class Translation_EasyExam : Window
	{
        Exam Exams; //Creates a new exam object
		List<Shapes> MyShapes = new List<Shapes>();   //Creates a list of shape object

		
		public int CorrectAnswerValue;          //Used to record the corr_answer
		GridLine GridLine;					    //Creates a grid object
		public const int ScaleFactor = 30;      //Sets the scale factor to be 30 pixels.
		
		public Translation_EasyExam()
		{
            InitializeComponent();
            Exams = new Exam(0, -2, "Translation Easy Exam", 3,timer);                                           //Start the timer upon launching the exam window.
            border.MouseWheel += new MouseWheelEventHandler((sender, e) => Transformations.Scaling.MouesWheel(sender, e, sliderSf));      //Sets the mouse wheel to scalling.mousewheel method
																																		   //Allows the user to move and pan around the grid.
			border.MouseUp += new MouseButtonEventHandler(Transformations.Scaling.BorderMouseUp);
			border.MouseMove += new MouseEventHandler((sender, e) => Transformations.Scaling.BorderMouseMove(sender, e, xSlider, ySlider, MyCanvas, Cursor));
			border.MouseDown += new MouseButtonEventHandler((sender, e) => Transformations.Scaling.BorderMouseDown(sender, e, MyCanvas));
			if (Properties.Settings.Default.DarkMode)
				border.Background = new SolidColorBrush(Color.FromArgb(255, 31, 31, 31));
		}
        private void Randomise()    //Randomly generates a new exam upon starting the exam.
		{
			try
			{
                for (int x = 0; x < 6; x++)
                {
                    switch (Rnd.RandomNumber(1, 8)) //Picks a random number between 1 to 7
				    {
						case 1:
							MyShapes.Add((new FreeForm().SpawnCustomShape(ShapePoints.Rectangle(ScaleFactor * 5), MyCanvas)));
							break;
						case 2:
							MyShapes.Add((new FreeForm().SpawnCustomShape(ShapePoints.Triangle(ScaleFactor * 5), MyCanvas)));
							break;
						case 3:
							MyShapes.Add((new FreeForm().SpawnCustomShape(ShapePoints.Parallelogram(ScaleFactor * 5), MyCanvas)));
							break;
						case 4:
							MyShapes.Add((new FreeForm().SpawnCustomShape(ShapePoints.Pentogan(ScaleFactor * 5), MyCanvas)));
							break;
						case 5:
							MyShapes.Add((new FreeForm().SpawnCustomShape(ShapePoints.Arrow(ScaleFactor * 5), MyCanvas)));
							break;
						case 6:
							MyShapes.Add((new FreeForm().SpawnCustomShape(ShapePoints.LShape(ScaleFactor * 5), MyCanvas)));
							break;
						case 7:
							MyShapes.Add((new FreeForm().SpawnCustomShape(ShapePoints.Star(ScaleFactor * 5), MyCanvas)));
							break;
					}
                    //Randomly places the shape onto the grid.
				    Canvas.SetLeft(MyShapes[MyShapes.Count - 1].MyShape, Rnd.RandomX(border, ScaleFactor));
				    Canvas.SetTop(MyShapes[MyShapes.Count - 1].MyShape, Rnd.RandomY(border, ScaleFactor));
					//Spawns a ghost of the original shape and then randomly places it on the grid.        			
					MyShapes.Add(((new FreeForm().WrongGhost(142, 0, 217, (Polygon)MyShapes[MyShapes.Count - 1].MyShape, MyCanvas))));

					Canvas.SetLeft(MyShapes[MyShapes.Count - 1].MyShape, Rnd.RandomX(border, ScaleFactor));
					Canvas.SetTop(MyShapes[MyShapes.Count - 1].MyShape, Rnd.RandomY(border, ScaleFactor));
                }
		    }
			catch (Exception)
			{
				//MessageBox.Show(
				//"Failed to randomly generate an 'Translation Easy' exam. " + Properties.Strings.CriticalFailuer,
				//Properties.Strings.EM_CriticalFailure + "400 L", System.Windows.MessageBoxButton.OK, MessageBoxImage.Error);
			}
}
		private int XAnswer()  //Gives the X answer value.
		{
			int x_answer;
			double original_x = Canvas.GetLeft(MyShapes[Exams.ArrayPos].MyShape);
			double ghost_x = Canvas.GetLeft(MyShapes[Exams.ArrayPos + 1].MyShape);
	
			double answer_pixel = ghost_x - original_x;
			x_answer = Convert.ToInt32(answer_pixel / ScaleFactor);
			return x_answer;
		}
		private int YAnswer()  //Gives the Y answer value.
		{
			int y_answer;
			double original_y = Canvas.GetTop(MyShapes[Exams.ArrayPos].MyShape);
			double ghost_y = Canvas.GetTop(MyShapes[Exams.ArrayPos + 1].MyShape);

			double answer_pixel = original_y - ghost_y;
			y_answer = Convert.ToInt32(answer_pixel / ScaleFactor);
			return y_answer;
		}
        private void NextQuestion()    //Used to change to the next question
        {
            try
            {
				Exams.AddQuesPos();   //Adds a value onto the question position
                Exams.ArrayPos += 2;   //Adds a value onto the array count.
               
                if (Exams.QuestionPos > 6) //if the exam questions have reached the end then display the exam results dialog.
                {
                    Exams.Timer.Stop();
                    BlurEffect myBlurEffect = new BlurEffect { Radius = 10 };
                    window.Effect = myBlurEffect;
                    this.Topmost = false;

                    ExamResults exam = new ExamResults(Exams);
                    exam.Show();
                    this.Owner = exam;
                    this.IsEnabled = false;
                }
                else //Change to the next question.
                {
                    foreach (var item in MyShapes)
                    {
                      item.MyShape.Visibility = Visibility.Hidden;  //makes all shapes invisible 
                    }
                    //Makes the shape for the current question visible
                    MyShapes[Exams.ArrayPos].MyShape.Visibility = Visibility.Visible;      
                    MyShapes[Exams.ArrayPos + 1].MyShape.Visibility = Visibility.Visible;

					Exams.ResetAttempts();
                    RefreshText();

                    xSlider.Value = 0;
                    ySlider.Value = 0;
                    sliderSf.Value = 0;
					Transformations.Scaling.Main(TranslationTransformCanvas, scaleTransformCanvas, xSlider, ySlider, sliderSf, border);
                    
                    //Creates a random number in each and every button.
                    b1_x.Text = ((Rnd.RandomX(border, ScaleFactor))/ScaleFactor).ToString();
                    b1_y.Text = ((Rnd.RandomY(border, ScaleFactor)) / ScaleFactor).ToString();
                    b2_x.Text = ((Rnd.RandomX(border, ScaleFactor)) / ScaleFactor).ToString();
                    b2_y.Text = ((Rnd.RandomY(border, ScaleFactor)) / ScaleFactor).ToString();
                    b3_x.Text = ((Rnd.RandomX(border, ScaleFactor)) / ScaleFactor).ToString();
                    b3_y.Text = ((Rnd.RandomY(border, ScaleFactor)) / ScaleFactor).ToString();
                    b4_x.Text = ((Rnd.RandomX(border, ScaleFactor)) / ScaleFactor).ToString();
                    b4_y.Text = ((Rnd.RandomY(border, ScaleFactor)) / ScaleFactor).ToString();
                    b5_x.Text = ((Rnd.RandomX(border, ScaleFactor)) / ScaleFactor).ToString();
                    b5_y.Text = ((Rnd.RandomY(border, ScaleFactor)) / ScaleFactor).ToString();
                    b6_x.Text = ((Rnd.RandomX(border, ScaleFactor)) / ScaleFactor).ToString();
                    b6_y.Text = ((Rnd.RandomY(border, ScaleFactor)) / ScaleFactor).ToString();


                    CorrectAnswerValue = Rnd.RandomNumber(1, 7);
                    switch (CorrectAnswerValue) //Randomly chooses the correct button and gives it the correct answer.
                    {
                        case 1:
                            b1_x.Text = XAnswer().ToString();
                            b1_y.Text = YAnswer().ToString();
                            break;
                        case 2:
                            b2_x.Text = XAnswer().ToString();
                            b2_y.Text = YAnswer().ToString();
                            break;
                        case 3:
                            b3_x.Text = XAnswer().ToString();
                            b3_y.Text = YAnswer().ToString();
                            break;
                        case 4:
                            b4_x.Text = XAnswer().ToString();
                            b4_y.Text = YAnswer().ToString();
                            break;
                        case 5:
                            b5_x.Text = XAnswer().ToString();
                            b5_y.Text = YAnswer().ToString();
                            break;
                        case 6:
                            b6_x.Text = XAnswer().ToString();
                            b6_y.Text = YAnswer().ToString();
                            break;
                    }
                }
            }
            catch (Exception)
            {
                ////MessageBox.Show(
                ////    "Failed to randomly generate an 'Translation Easy' exam. " + Properties.Strings.CriticalFailuer,
                ////    Properties.Strings.EM_CriticalFailure + "400 L", System.Windows.MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void RefreshText()   //Used to refresh the text UI.
		{
			question_no.Content = "Question: " + Exams.QuestionPos.ToString() + "/6";
			score.Content = "Score: " + Exams.ScoreValue.ToString() + "/6";
			attempts.Content = "Attempts: " + Exams.Attmepts.ToString() + "/2";
			question.Content = "Which translation vector maps the original shape to the ghost shape?";
		}   
		private void CheckAnswer(int button)   //used to check if the pressed button matches the answer or not.
		{
			if (button == CorrectAnswerValue)   //If correct add a value to the score and skip to the next question
			{
                Show(CorrectAnswer);
				Exams.AddPoint();
              	NextQuestion();
			}
			else //Answer is incorrect
			{
				Exams.AddAttempt();
				if (Exams.Attmepts > 2) //If the attempt counter is over 3 then skip to next question.
				{
                    Show(OutofAttempts);
                    NextQuestion();
				}
				else
				{
                    Show(WrongAnswer);
                }
				RefreshText();
			}
		}
      
        //When the button is pressed check the answer.
        private void OptionPressed(object sender, RoutedEventArgs e)
		{
			CheckAnswer(Convert.ToInt32(((Button)sender).Tag.ToString()));
		}
	
        //Used to setup the exam
        private void CanvasLoaded(object sender, RoutedEventArgs e)
        {
			GridLine = new GridLine().DrawGrid(3500, ScaleFactor, MyCanvas);
			Transformations.Scaling.Main(TranslationTransformCanvas, scaleTransformCanvas, xSlider, ySlider, sliderSf, border);
            Randomise();
            NextQuestion();
            foreach (Label t in GridLine.Labels)
            {
                MyCanvas.Children.Add(t);
            }
        }
        private void Exit(object sender, RoutedEventArgs e)
		{
			MessageBoxResult exit = MessageBox.Show("Are you sure you wish to abandon this exam?", "Are you sure?",
				 System.Windows.MessageBoxButton.OKCancel,
				 MessageBoxImage.Warning);

			if (exit == MessageBoxResult.OK)
			{
				TakeExam exam = new TakeExam();
				exam.Show();
				this.Close();
			}
		}
		private void Scaling(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			Transformations.Scaling.Main(TranslationTransformCanvas, scaleTransformCanvas, xSlider, ySlider, sliderSf, border);
		}
        private void MouseMove(object sender, MouseEventArgs e)
		{
			cords.Content = "( " + (Convert.ToDouble(Mouse.GetPosition(MyCanvas).X) / ScaleFactor).ToString("0.0") + "  ,  " + (-Convert.ToDouble(Mouse.GetPosition(MyCanvas).Y) / ScaleFactor).ToString("0.0") + " )";
		}
        private void Show(Border type) //Shows the correct, incorrect or skip answer method.
        {
            type.Visibility = System.Windows.Visibility.Visible;

            var a = new DoubleAnimation
            {
                From = 1.0,
                To = 0.0,
                FillBehavior = FillBehavior.Stop,
                BeginTime = TimeSpan.FromSeconds(1),
                Duration = new Duration(TimeSpan.FromSeconds(0.5))
            };
            var storyboard = new Storyboard();

            storyboard.Children.Add(a);
            Storyboard.SetTarget(a, type);
            Storyboard.SetTargetProperty(a, new PropertyPath(OpacityProperty));
            storyboard.Completed += delegate { type.Visibility = System.Windows.Visibility.Hidden; };
            storyboard.Begin();
        }
    }
}
