using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Shapes;

namespace Transformations
{
	/// <summary>
	/// Enlargement easy exam - 2 shapes spawn, an original and an enlarged shape, the user must then select the scale factor in the drop down menu.
	/// </summary>
	public partial class Enlargement_EasyExam : Window
	{
		readonly Exam Exams = new Exam(0, -2, "Enlargement Easy Exam", 1);
		List<Shapes> MyShapes = new List<Shapes>();
		public readonly double[] values = { -2, -1, 0.25, 0.5, 0.75, 2 };
		public List<int> Answers = new List<int>();
        
		GridLine GridLine;
		public const int ScaleFactor = 30;

		List<RayLines> MyRayLines = new List<RayLines>();
		public bool IsDrawingRays = false;
		
		public int ShapeY;
		public int ShapeX;
		public double NewLeft;
		public double NewTop;
        		
		public Enlargement_EasyExam()
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
                    switch (Rnd.RandomNumber(1, 8))
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

                    switch (Rnd.RandomNumber(1, 5))
                    {
                        case 1:
                            //Top Horizontal
                            ShapeX = Rnd.RandomNumber(-5 * ScaleFactor, 5 * ScaleFactor);
                            NewLeft = (Round.ToNearest((ShapeX), (ScaleFactor)));
                            Canvas.SetLeft(MyShapes[MyShapes.Count - 1].MyShape, NewLeft);
                            Canvas.SetTop(MyShapes[MyShapes.Count - 1].MyShape, -ScaleFactor * 6);
                            break;
                        case 2:
                            //Bottom Horizontal
                            ShapeX = Rnd.RandomNumber(-5 * ScaleFactor, 5 * ScaleFactor);
                            NewLeft = (Round.ToNearest((ShapeX), (ScaleFactor)));
                            Canvas.SetLeft(MyShapes[MyShapes.Count - 1].MyShape, NewLeft);
                            Canvas.SetTop(MyShapes[MyShapes.Count - 1].MyShape, ScaleFactor);

                            break;
                        case 3:
                            //Right Vertical
                            ShapeY = Rnd.RandomNumber(-5 * ScaleFactor, 5 * ScaleFactor);
                            NewTop = Round.ToNearest(ShapeY, ScaleFactor);
                            Canvas.SetTop(MyShapes[MyShapes.Count - 1].MyShape, NewTop);
                            Canvas.SetLeft(MyShapes[MyShapes.Count - 1].MyShape, ScaleFactor);

                            break;
                        case 4:
                            //Left Vertical
                            ShapeY = Rnd.RandomNumber(-5 * ScaleFactor, 5 * ScaleFactor);
                            NewTop = Round.ToNearest(ShapeY, ScaleFactor);
                            Canvas.SetTop(MyShapes[MyShapes.Count - 1].MyShape, NewTop);
                            Canvas.SetLeft(MyShapes[MyShapes.Count - 1].MyShape, -6 * ScaleFactor);

                            break;
                    }

					MyShapes.Add((new FreeForm().WrongGhost(233, 255, 0, (Polygon)MyShapes[MyShapes.Count - 1].MyShape, MyCanvas)));
					

		
                    Answers.Add(Rnd.RandomNumber(0, 6));
                    MyShapes[MyShapes.Count - 1].MyScalingTransform.CenterX = -(Canvas.GetLeft(MyShapes[MyShapes.Count - 1].MyShape));
                    MyShapes[MyShapes.Count - 1].MyScalingTransform.CenterY = -(Canvas.GetTop(MyShapes[MyShapes.Count - 1].MyShape));

                    MyShapes[MyShapes.Count - 1].MyScalingTransform.ScaleX = values[Answers[Answers.Count - 1]];
                    MyShapes[MyShapes.Count - 1].MyScalingTransform.ScaleY = values[Answers[Answers.Count - 1]];
                }
            }
            catch (Exception)
            {
                MessageBox.Show(
                    "Failed to randomly generate an 'Enlargement Easy' exam. " + Properties.Resources.CriticalFailuer,
                    "Critical Program Failure: 400 B", System.Windows.MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void SubmitAnswer(object sender, RoutedEventArgs e)
        {
            if (Answers[Exams.QuestionPos - 1] == enlargement_answer.SelectedIndex)
            {
                Show(CorrectAnswer);
				Exams.AddPoint();
                NextQuestion();
            }
            else
            {
				Exams.AddAttempt();
            
                if (Exams.Attmepts > 2)
                {
                    Show(OutofAttempts);
                    NextQuestion();
                }
                else
                {
                    Show(WrongAnswer);

					MyShapes.Add((new FreeForm().WrongGhost(0, 0, 0, (Polygon)MyShapes[Exams.ArrayPos].MyShape, MyCanvas)));

			        MyShapes[MyShapes.Count - 1].MyScalingTransform.CenterX = -((Canvas.GetLeft(MyShapes[MyShapes.Count - 1].MyShape)));
                    MyShapes[MyShapes.Count - 1].MyScalingTransform.CenterY = (-(Canvas.GetTop(MyShapes[MyShapes.Count - 1].MyShape)));
                    MyShapes[MyShapes.Count - 1].MyScalingTransform.ScaleX = values[enlargement_answer.SelectedIndex];
                    MyShapes[MyShapes.Count - 1].MyScalingTransform.ScaleY = values[enlargement_answer.SelectedIndex];
                }
            }
            RefreshText();
        }
        private void NextQuestion()
		{
			Exams.AddQuesPos();
			Exams.ArrayPos += 2;
			Exams.ResetAttempts();

            if (Exams.QuestionPos > 6)
            {
                Exams.Timer.DispatcherTimer.Stop();
                BlurEffect myBlurEffect = new BlurEffect { Radius = 10 };
                window.Effect = myBlurEffect;
                this.Topmost = false;

                ExamResults exam = new ExamResults(Exams);
                exam.Show();
                this.Owner = exam;
                this.IsEnabled = false;
            }
            else
            {
                foreach (var raygroup in MyRayLines)
                {
                    foreach (var line in raygroup.RayLinesList)
                    {
                        MyCanvas.Children.Remove(line);
                    }
                }

                foreach (var shape in MyShapes)
                {
                    shape.MyShape.Visibility = Visibility.Hidden;
                }

				MyShapes.Add((new Circle("dupe_enlargement").MakerSpawn(0, 0, MyCanvas)));
                MyShapes[Exams.ArrayPos].MyShape.Visibility = Visibility.Visible;
                MyShapes[Exams.ArrayPos + 1].MyShape.Visibility = Visibility.Visible;
                RefreshText();

                xSlider.Value = 0;
                ySlider.Value = 0;
                sliderSf.Value = 0;
				Transformations.Scaling.Main(TranslationTransformCanvas, scaleTransformCanvas, xSlider, ySlider, sliderSf, border);
            }
        }
		private void RefreshText()
		{
			question_no.Content = "Question: " + Exams.QuestionPos.ToString() + "/6";
			score.Content = "Score: " + Exams.ScoreValue.ToString() + "/6";
			attempts.Content = "Attempts: " + Exams.Attmepts.ToString() + "/2";
			question.Content = "What is the scale factor preformed on the ghost shape?";
		}
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
        private void KeyPressed(object sender, System.Windows.Input.KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				SubmitAnswer(sender, e);
			}
		}
        private void DrawRays(object sender, RoutedEventArgs e)
		{
			IsDrawingRays = true;
			this.Cursor = Cursors.Cross;

			MyRayLines.Add(new RayLines());
			MyRayLines[MyRayLines.Count - 1].RayLinesList.Add(new Line() {
				Stroke = Brushes.DarkOrange,
				StrokeThickness = 1
			});
			Panel.SetZIndex(MyRayLines[MyRayLines.Count - 1].RayLinesList[((MyRayLines[MyRayLines.Count - 1].RayLinesList).Count) - 1], 2);
			MyCanvas.Children.Add(MyRayLines[MyRayLines.Count - 1].RayLinesList[((MyRayLines[MyRayLines.Count - 1].RayLinesList).Count) - 1]);
		}
        private void CanvasClick(object sender, MouseButtonEventArgs e)
		{
			if (IsDrawingRays)    //If User is drawing ray lines.
			{
				if ((((MyRayLines[MyRayLines.Count - 1].RayLinesList).Count) - 1) < ((MyShapes[Exams.ArrayPos].MyShape as Polygon).Points.Count) - 1)
				{
					MyRayLines[MyRayLines.Count - 1].RayLinesList.Add(new Line() {
						Stroke = Brushes.DarkOrange,
						StrokeThickness = 1
					});
					Panel.SetZIndex(MyRayLines[MyRayLines.Count - 1].RayLinesList[((MyRayLines[MyRayLines.Count - 1].RayLinesList).Count) - 1], 2);
					MyCanvas.Children.Add(MyRayLines[MyRayLines.Count - 1].RayLinesList[((MyRayLines[MyRayLines.Count - 1].RayLinesList).Count) - 1]);
				}
				else
				{
					this.Cursor = Cursors.Arrow;
					IsDrawingRays = false;
				}
				CanvasMove(sender, e);
			}
		}
        private void CanvasMove(object sender, MouseEventArgs e)
		{
			if (IsDrawingRays && MyRayLines[MyRayLines.Count - 1].RayLinesList.Count >= 1)
			{
				LineCaculator((Convert.ToDouble(((MyShapes[Exams.ArrayPos].MyShape as Polygon).Points[((MyRayLines[MyRayLines.Count - 1].RayLinesList).Count) - 1].X) + Convert.ToDouble(Canvas.GetLeft(MyShapes[Exams.ArrayPos].MyShape)))), (Convert.ToDouble(((MyShapes[Exams.ArrayPos].MyShape as Polygon).Points[((MyRayLines[MyRayLines.Count - 1].RayLinesList).Count) - 1].Y) + Convert.ToDouble(Canvas.GetTop(MyShapes[Exams.ArrayPos].MyShape)))));
			}
			cords.Content = "( " + (Convert.ToDouble(Mouse.GetPosition(MyCanvas).X) / ScaleFactor).ToString("0.0") + "  ,  " + (-Convert.ToDouble(Mouse.GetPosition(MyCanvas).Y) / ScaleFactor).ToString("0.0") + " )";
		}
        private void LineCaculator(double X, double Y)
		{
			try
			{
				double m = (((Convert.ToDouble(Y) - Convert.ToDouble(Mouse.GetPosition(MyCanvas).Y)) / (Convert.ToDouble(X) - Convert.ToDouble(Mouse.GetPosition(MyCanvas).X))));
				double c = -(Convert.ToDouble(Y) - (m * Convert.ToDouble(X)));

				//Left
				MyRayLines[MyRayLines.Count - 1].RayLinesList[((MyRayLines[MyRayLines.Count - 1].RayLinesList).Count) - 1].Y1 = (-(c) + ((3500) * (m)));
				MyRayLines[MyRayLines.Count - 1].RayLinesList[((MyRayLines[MyRayLines.Count - 1].RayLinesList).Count) - 1].X2 = -3500;

				//Right
				MyRayLines[MyRayLines.Count - 1].RayLinesList[((MyRayLines[MyRayLines.Count - 1].RayLinesList).Count) - 1].X1 = 3500;
				MyRayLines[MyRayLines.Count - 1].RayLinesList[((MyRayLines[MyRayLines.Count - 1].RayLinesList).Count) - 1].Y2 = (-(c) - ((3500) * (m)));

			}
			catch (Exception)
			{

			}
		}
        private void DeleteRays(object sender, RoutedEventArgs e)
		{
			foreach (var raygroup in MyRayLines)
			{
				foreach (var line in raygroup.RayLinesList)
				{
					MyCanvas.Children.Remove(line);
				}
			}
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
