using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Shapes;

namespace Transformations
{
    /// <summary>
    /// A translation hard exam generates two different shapes, the user must then manually enter the translation vector into the text boxs.
    /// </summary>
    public partial class Translation_HardExam : Window
	{
        Exam Exams;	//Creates a new exam object
		List<Shapes> MyShapes = new List<Shapes>();								//Creates a list to contain the shape objects
		GridLine Grid;						//Creates a grid object
        const int ScaleFactor = 25;	//Declares the scale factor 
				
		public Translation_HardExam()
		{
			InitializeComponent();
            //Sets a variety of events for both the timer and the canvas- for timing and scaling/moving around.
            //This allows code to be reused and prevents the need for redundant code.
            Exams = new Exam(0, -2, Properties.Strings.THardExam, 4, timer);
            border.MouseWheel += new MouseWheelEventHandler((sender, e) => Transformations.Scaling.MouesWheel(sender, e, sliderSf));
			border.MouseUp += new MouseButtonEventHandler(Transformations.Scaling.BorderMouseUp);
			border.MouseMove +=	new MouseEventHandler((sender, e) => Transformations.Scaling.BorderMouseMove(sender, e, xSlider, ySlider, canvas, Cursor));
			border.MouseDown += new MouseButtonEventHandler((sender, e) => Transformations.Scaling.BorderMouseDown(sender, e, canvas));
			if (Properties.Settings.Default.DarkMode)
				border.Background = new SolidColorBrush(Color.FromArgb(255, 31, 31, 31));
		}
        private void Randomise()		//A function called upon at start up to create a new random exam
        {
            try
            {
                for (int x = 0; x < 6; x++)				//Repeats 6 times for each question
                {
                    switch (Rnd.RandomNumber(1, 8))		//Generate a random number between 1 to 7
                    {	//The random number generated is then used to specify the type of shape spawned.
						case 1:
							MyShapes.Add((new FreeForm().SpawnCustomShape(ShapePoints.Rectangle(ScaleFactor * 5), canvas)));
							break;
						case 2:
							MyShapes.Add((new FreeForm().SpawnCustomShape(ShapePoints.Triangle(ScaleFactor * 5), canvas)));
							break;
						case 3:
							MyShapes.Add((new FreeForm().SpawnCustomShape(ShapePoints.Parallelogram(ScaleFactor * 5), canvas)));
							break;
						case 4:
							MyShapes.Add((new FreeForm().SpawnCustomShape(ShapePoints.Pentogan(ScaleFactor * 5), canvas)));
							break;
						case 5:
							MyShapes.Add((new FreeForm().SpawnCustomShape(ShapePoints.Arrow(ScaleFactor * 5), canvas)));
							break;
						case 6:
							MyShapes.Add((new FreeForm().SpawnCustomShape(ShapePoints.LShape(ScaleFactor * 5), canvas)));
							break;
						case 7:
							MyShapes.Add((new FreeForm().SpawnCustomShape(ShapePoints.Star(ScaleFactor * 5), canvas)));
							break;
					}
					//The location of the shape is randomly generated- but limits are in place to ensure it is within the view-port
                    Canvas.SetLeft(MyShapes[MyShapes.Count - 1].MyShape, Rnd.RandomX(border, ScaleFactor));
                    Canvas.SetTop(MyShapes[MyShapes.Count - 1].MyShape, Rnd.RandomY(border, ScaleFactor));
					
					//A ghost shape is spawned from the original and then also randomly placed within the view-port
					MyShapes.Add((new FreeForm().WrongGhost(142, 0, 217, (Polygon)MyShapes[MyShapes.Count - 1].MyShape, canvas)));
					Canvas.SetLeft(MyShapes[MyShapes.Count - 1].MyShape, Rnd.RandomX(border, ScaleFactor));
                    Canvas.SetTop(MyShapes[MyShapes.Count - 1].MyShape, Rnd.RandomY(border, ScaleFactor));
                }
            }
            catch (Exception)
            {
                MessageBox.Show(
                    Properties.Strings.FailedToMakeExam + Properties.Strings.CriticalFailuer,
                    Properties.Strings.EM_CriticalFailure + "400 M", System.Windows.MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
        private void AnswerSubmit(object sender, RoutedEventArgs e)		//Called upon when the user submits an answer
		{
			try
			{
				if (x_cords.Text.ToString() == XAnswer().ToString() && y_cords.Text.ToString() == YAnswer().ToString())	//If both the X and Y values entered equal the answer
				{
                    Show(CorrectAnswer);	//Show the correct answer overlay
					Exams.AddPoint();		//Add a point
					NextQuestion();			//Goto the next question
				}
				else //Else- the answer was wrong
				{
					//The user input is converted to a double, multiplied into its pixel value and added onto the original
					double XGuess = Canvas.GetLeft(MyShapes[Exams.ArrayPos].MyShape) + Convert.ToDouble(x_cords.Text) * ScaleFactor;
                    double YGuess = Canvas.GetTop(MyShapes[Exams.ArrayPos].MyShape) - Convert.ToDouble(y_cords.Text) * ScaleFactor;

					Exams.AddAttempt();		//An attempt is added 
					//A new ghost shape is spawned to show the user where their wrong guess was.
					MyShapes.Add((new FreeForm().WrongGhost(0, 0, 0, (Polygon)MyShapes[Exams.ArrayPos].MyShape, canvas)));			
					MyShapes[MyShapes.Count - 1].MyShape.Visibility = Visibility.Visible;
					Canvas.SetLeft(MyShapes[MyShapes.Count - 1].MyShape, XGuess);
                    Canvas.SetTop(MyShapes[MyShapes.Count - 1].MyShape, YGuess);

                   
                    if (Exams.Attmepts > 2)	//If they have made 3 incorrect guess
                    {
                        Show(OutofAttempts);	//Show the out of attempts overlay
                        NextQuestion();			//Skip onto the next question
                    }
                    else
                        Show(WrongAnswer);		//Show the wrong answer overlay
                }
			}
			catch (Exception)
			{
                MessageBox.Show(
                    Properties.Strings.VectorNotInFormat + Properties.Strings.UserError,
                    Properties.Strings.EM_InvalidInputTypeError + "302 I", System.Windows.MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            finally
            {
                RefreshText();
            }
		}
		private int XAnswer()	//Calculates the X answer
		{
			int x_answer ;
			double original_x = Canvas.GetLeft(MyShapes[Exams.ArrayPos].MyShape);
			double ghost_x = Canvas.GetLeft(MyShapes[Exams.ArrayPos + 1].MyShape);

			double answer_pixel = ghost_x - original_x;
			x_answer = Convert.ToInt32(answer_pixel / ScaleFactor);
			return x_answer;
		}
		private int YAnswer()	//Calculates the Y answer
		{
			int y_answer;
			double original_y = Canvas.GetTop(MyShapes[Exams.ArrayPos].MyShape);
			double ghost_y =  Canvas.GetTop(MyShapes[Exams.ArrayPos + 1].MyShape);

			double answer_pixel = original_y - ghost_y;
			y_answer = Convert.ToInt32(answer_pixel / ScaleFactor);
			return y_answer;
		}
        private void NextQuestion()	//Called upon to advance to the next question
        {
			Exams.AddQuesPos();		//Question position is incremented
            Exams.ArrayPos += 2;	//Array position is incremented by two (due to two shapes per question)
            if (Exams.QuestionPos > 6)	//If the user has reached the end of the questions (6)
            {
                Exams.Timer.Stop();								//Stop the timer
                BlurEffect myBlurEffect = new BlurEffect { Radius = 10 };		//Add a blur to the window
                window.Effect = myBlurEffect;
                this.Topmost = false;

                ExamResults exam = new ExamResults(Exams);	//Show the exam results window
                exam.Show();
                this.Owner = exam;
                this.IsEnabled = false;
            }
            else //Else next question
            {
                //makes all shapes invisible 
                MyShapes.ForEach(p => p.MyShape.Visibility = Visibility.Hidden);
                //Make only the two shapes needed visible
                MyShapes[Exams.ArrayPos].MyShape.Visibility = Visibility.Visible;
                MyShapes[Exams.ArrayPos + 1].MyShape.Visibility = Visibility.Visible;

				Exams.ResetAttempts();	//Reset the number of attempts on the current question
                RefreshText();          //Refresh the UI text to match these changes

				//Set all other values back to defaults
				xSlider.Value = 0;
                ySlider.Value = 0;
                sliderSf.Value = 0;
				Transformations.Scaling.Main(TranslationTransformCanvas, scaleTransformCanvas, xSlider, ySlider, sliderSf, border);

                x_cords.Text = "";
                y_cords.Text = "";                   
            }
        }
        private void RefreshText()	//Used to update the text in the UI
		{
            Exams.Refresh(question_no, score, attempts);
            question.Content = Properties.Strings.THardEText;
		}
        private void Canvas_Loaded(object sender, RoutedEventArgs e)	//Once the canvas has loaded spawn the grid
		{
			Grid = new GridLine().DrawGrid(3500, ScaleFactor, canvas);
			Transformations.Scaling.Main(TranslationTransformCanvas, scaleTransformCanvas, xSlider, ySlider, sliderSf, border);
			Randomise();          
			NextQuestion();
            Grid.Labels.ForEach(p => canvas.Children.Add(p));
        }
		

		private void Scaling(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			Transformations.Scaling.Main(TranslationTransformCanvas, scaleTransformCanvas, xSlider, ySlider, sliderSf, border);
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
			cords.Content = "( " + (Convert.ToDouble(Mouse.GetPosition(canvas).X) / ScaleFactor).ToString("0.0") + "  ,  " + (-Convert.ToDouble(Mouse.GetPosition(canvas).Y) / ScaleFactor).ToString("0.0") + " )";
		}

        private void Show(Border type) //Shows the correct, incorrect or skip answer method.
        {
            Exams.Show(type);
        }
        private void Exit(object sender, RoutedEventArgs e)
        {
            Exams.Exit(this);
        }
    }
}
