using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Transformations
{
	/// <summary>
	/// Rotation hard exam, two shapes will spawn, the original and the rotated, the user is given the rotation amount and must specify the center of rotation.
	/// </summary>
	public partial class Rotation_HardExam : Window
	{
		readonly Exam Exams = new Exam(0, -3, "Rotation Hard Exam", 8);
		List<Shapes> MyShapes = new List<Shapes>();

		readonly public int[] Values = {  90,  180,  270, };
		public List<int> XAnswer = new List<int>();
		public List<int> YAnswer = new List<int>();

		GridLine GirdLine;
		public const int ScaleFactor = 50;
	
		public Rotation_HardExam()
		{
			InitializeComponent();
			Exams.Timer.DispatcherTimer.Tick += new EventHandler(TimerTick);
			Exams.Timer.DispatcherTimer.Interval = new TimeSpan(0, 0, 1);
			Exams.Timer.DispatcherTimer.Start();
			border.MouseWheel += new MouseWheelEventHandler((sender, e) => Transformations.Scaling.MouesWheel(sender, e, sliderSf));
			border.MouseUp += new MouseButtonEventHandler(Transformations.Scaling.BorderMouseUp);
			border.MouseMove += new MouseEventHandler((sender, e) => Transformations.Scaling.BorderMouseMove(sender, e, xSlider, ySlider, MyCanvas, Cursor));
			border.MouseDown += new MouseButtonEventHandler((sender, e) => Transformations.Scaling.BorderMouseDown(sender, e, MyCanvas));
			if (Properties.Settings.Default.DarkMode)
				border.Background = new SolidColorBrush(Color.FromArgb(255, 31, 31, 31));
		}
        private void Randomise()
        {
            try
            {
                for (int z = 0; z < 6; z++)
                {
					switch (Rnd.RandomNumber(1, 7))
					{
						case 1:
							MyShapes.Add((new FreeForm().SpawnCustomShape(ShapePoints.Star(ScaleFactor * 5), MyCanvas)));
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
					}

                    Canvas.SetLeft(MyShapes[MyShapes.Count - 1].MyShape, Rnd.RandomY(border, ScaleFactor));
                    Canvas.SetTop(MyShapes[MyShapes.Count - 1].MyShape, Rnd.RandomY(border, ScaleFactor));

					MyShapes.Add((new FreeForm().WrongGhost(0, 255, 0, (Polygon)MyShapes[MyShapes.Count - 1].MyShape, MyCanvas)));
					
					if (Rnd.RandomNumber(0, 2) == 0)
                    {
                        MyShapes[MyShapes.Count - 1].MyRotateTransform.CenterX = Rnd.RandomNumber(7, 11) * ScaleFactor;
                    }
                    else
                    {
                        MyShapes[MyShapes.Count - 1].MyRotateTransform.CenterX = Rnd.RandomNumber(1, 5) * -ScaleFactor;
                    }
                    if (Rnd.RandomNumber(0, 2) == 0)
                    {
                        MyShapes[MyShapes.Count - 1].MyRotateTransform.CenterY = Rnd.RandomNumber(7, 11) * ScaleFactor;
                    }
                    else
                    {
                        MyShapes[MyShapes.Count - 1].MyRotateTransform.CenterY = Rnd.RandomNumber(1, 5) * -ScaleFactor;
                    }


                    MyShapes[MyShapes.Count - 1].MyRotateTransform.Angle = Values[Rnd.RandomNumber(0, 3)];
                    XAnswer.Add(Convert.ToInt32((Canvas.GetLeft(MyShapes[MyShapes.Count - 1].MyShape) + MyShapes[MyShapes.Count - 1].MyRotateTransform.CenterX) / ScaleFactor));
                    YAnswer.Add(-Convert.ToInt32((Canvas.GetTop(MyShapes[MyShapes.Count - 1].MyShape) + MyShapes[MyShapes.Count - 1].MyRotateTransform.CenterY) / ScaleFactor));
			    	MyShapes.Add((new Circle("dupe_rotation").MakerSpawn((Canvas.GetLeft(MyShapes[MyShapes.Count - 1].MyShape) + MyShapes[MyShapes.Count - 1].MyRotateTransform.CenterX), (Canvas.GetTop(MyShapes[MyShapes.Count - 1].MyShape) + MyShapes[MyShapes.Count - 1].MyRotateTransform.CenterY), MyCanvas)));
				  }
            }
            catch (Exception)
            {
                MessageBox.Show(
                    "Failed to randomly generate an 'Rotation Hard' exam. " + Properties.Resources.CriticalFailuer,
                    "Critical Program Failure: 400 K", System.Windows.MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void AnswerSubmit(object sender, RoutedEventArgs e)
		{
			try
			{
				if (X_Guess.Text == XAnswer[Exams.QuestionPos - 1].ToString() && Y_Guess.Text == YAnswer[Exams.QuestionPos - 1].ToString())
				{
                    Show(CorrectAnswer);
					Exams.AddPoint();
					NextQuestion();
				}
				else
				{
                    double XGuess = Convert.ToInt64(X_Guess.Text) * ScaleFactor;
                    double YGuess = -Convert.ToInt64(Y_Guess.Text) * ScaleFactor;

					Exams.AddAttempt();
					if (Exams.Attmepts > 2)
                    {
                        Show(OutofAttempts);
                        NextQuestion();
                    }
                    else
                    {
                        Show(WrongAnswer);
						MyShapes.Add((new Circle("dupe_rotation").MakerSpawn(XGuess, YGuess,  MyCanvas)));
						MyShapes[MyShapes.Count - 1].MyShape.Fill = new SolidColorBrush(Color.FromArgb(50, 0, 0, 0));

						MyShapes.Add((new FreeForm()).WrongGhost(0, 0, 0, (Polygon)MyShapes[Exams.ArrayPos].MyShape, MyCanvas));
						MyShapes[MyShapes.Count - 1].MyRotateTransform.CenterX =  XGuess - Canvas.GetLeft(MyShapes[MyShapes.Count - 1].MyShape); 
                        MyShapes[MyShapes.Count - 1].MyRotateTransform.CenterY = YGuess - Canvas.GetTop(MyShapes[MyShapes.Count - 1].MyShape); 
                        MyShapes[MyShapes.Count - 1].MyRotateTransform.Angle = MyShapes[Exams.ArrayPos + 1].MyRotateTransform.Angle;
                    }
                }
                RefreshText();
            }
			catch (Exception)
			{
				MessageBox.Show(
					"The coordinates entered were not in the correct format; only numerical values are allowed. " + Properties.Resources.UserError,
					"Invalid Input Type Error: 300 H", System.Windows.MessageBoxButton.OK, MessageBoxImage.Warning);
			}
		}
		private void NextQuestion()
		{
			Exams.AddQuesPos();
			Exams.ArrayPos += 3;
			Exams.ResetAttempts();

			foreach (var shape in MyShapes)
			{
				shape.MyShape.Visibility = Visibility.Hidden;
			}

			if (Exams.QuestionPos > 6)
			{
				Exams.Timer.DispatcherTimer.Stop();
				BlurEffect myBlurEffect = new BlurEffect {Radius = 10};
				window.Effect = myBlurEffect;
				this.Topmost = false;

				ExamResults exam = new ExamResults(Exams);
				exam.Show();
				this.Owner = exam;
				this.IsEnabled = false;
			}
			else
			{
				MyShapes[Exams.ArrayPos].MyShape.Visibility = Visibility.Visible;        //Original Shape
				MyShapes[Exams.ArrayPos + 1].MyShape.Visibility = Visibility.Visible;    //Ghost Shape
				if (hint.IsChecked == true)
				{
					MyShapes[Exams.ArrayPos + 2].MyShape.Visibility = Visibility.Visible;    //Marker Point
				}
				X_Guess.Text = "";
				Y_Guess.Text = "";

				xSlider.Value = 0;
				ySlider.Value = 0;
				sliderSf.Value = 0;
				Transformations.Scaling.Main(TranslationTransformCanvas, scaleTransformCanvas, xSlider, ySlider, sliderSf, border);

				RefreshText();
			}
		}
		private void RefreshText()
		{
			question_no.Content = "Question: " + Exams.QuestionPos.ToString() + "/6";
			score.Content = "Score: " + Exams.ScoreValue.ToString() + "/6";
			attempts.Content = "Attempts: " + Exams.Attmepts.ToString() + "/2";
			question.Content = "What is the coordinate for center of rotation?";
			ShapeRotAmount.Content = "Hint: This shape has rotated: "+ MyShapes[Exams.ArrayPos+1].MyRotateTransform.Angle + " Degrees Clockwise"; 
		}
        private void CanvasLoaded(object sender, RoutedEventArgs e)
		{
			GirdLine = new GridLine().DrawGrid(3500, ScaleFactor, MyCanvas);
			Transformations.Scaling.Main(TranslationTransformCanvas, scaleTransformCanvas, xSlider, ySlider, sliderSf, border);
			Randomise();
			NextQuestion();
			foreach (Label t in GirdLine.Labels)
			{
				MyCanvas.Children.Add(t);
			}
		}
		private void TimerTick(object sender, EventArgs e)
		{
			Exams.Timer.Seconds++;
            timer.Content = Exams.Timer.Seconds <= 9 ? timer.Content = Exams.Timer.Minutes + ":0" + Exams.Timer.Seconds : timer.Content = Exams.Timer.Minutes + ":" + Exams.Timer.Seconds;

            if (Exams.Timer.Seconds >= 59)
			{
				Exams.Timer.Seconds = -1;
				Exams.Timer.Minutes++;
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
        private void HintsOff(object sender, RoutedEventArgs e)
		{
			try
			{
				MyShapes[Exams.ArrayPos + 2].MyShape.Visibility = Visibility.Hidden;    //Marker Point
			}
			catch (Exception)
			{ }
		}
        private void HintsOn(object sender, RoutedEventArgs e)
		{
			try
			{
				MyShapes[Exams.ArrayPos + 2].MyShape.Visibility = Visibility.Visible;    //Marker Point
			}
			catch (Exception)
			{	}

		}
        private void KeyPressed(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				AnswerSubmit(sender, e);
			}
		}
        private void MouseMove(object sender, MouseEventArgs e)
		{
			cords.Content = "( " + (Convert.ToDouble(Mouse.GetPosition(MyCanvas).X) / ScaleFactor).ToString("0.0") + "  ,  " + (-Convert.ToDouble(Mouse.GetPosition(MyCanvas).Y) / ScaleFactor).ToString("0.0") + " )";
		}
        private void Show(Border type)
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
