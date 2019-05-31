using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Brian_Release
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Variables//
        bool isBlack = true; //this flag for color of link (true = Black, false = LightCoral)
        Dictionary<Label, Grid []> LinkToGrids = new Dictionary<Label, Grid []>();//Dictionary for link and the grids which link send user
        Grid [] grids_Math = new Grid [2];//Here grid to set visibility visible after clicking link *Math*
        Grid [] grids_Memory = new Grid [2];//And here to *Memory*
        CheckBox [] AllOperations = new CheckBox [4];
        List<char> ActiveOperations = new List<char>();
        public static int Right { get; set; }
        public static int Wrong { get; set; }
        private int k = 0;
        private uint Seconds = 1, Minutes = 3;
        public static int SecondsForMemory, NumbersForMemory = 0;
        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        System.Windows.Forms.Timer timer2 = new System.Windows.Forms.Timer();
        //Variabels//
        public MainWindow()//MainWindow constructor
        {
            InitializeComponent();
            timer.Tick += Timer_Tick;
            timer2.Tick += Timer2_Tick;
            timer.Interval = 1000;
            timer.Enabled = timer2.Enabled = false;
            Numbers_Slider.ValueChanged += Numbers_Slider_ValueChanged;
            Seconds_Slider.ValueChanged += Seconds_Slider_ValueChanged;
            Grid_Math.Visibility = Grid_Memory.Visibility = Visibility.Hidden;//Set Math and Memory work place hidden
            grids_Math.InitializeArray(Grid_Math, Grid_Math_Edit);//add elems to array 
            grids_Memory.InitializeArray(Grid_Memory, Grid_Memory_Edit);//and here again in other
            LinkToGrids.Add(Link_Math, grids_Math);//link and grids which works with MATH
            LinkToGrids.Add(Link_Memory, grids_Memory);//and here with MEMORY
            AllOperations.InitializeArray(checkBox1, checkBox2, checkBox3, checkBox4);
            checkBox1.IsChecked = checkBox2.IsChecked = true;
        }

        private void Timer2_Tick(object sender, EventArgs e)
        {
            Expression_Memory.Visibility = Visibility.Visible;
            k++;
            if (k == SecondsForMemory)
            {
                timer2.Enabled = false;
                Expression_Memory.Visibility = Visibility.Hidden;
                k = 0;
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            Seconds--;
            if (Seconds == 0 && Minutes > 0)
            {
                Seconds = 59;
                Minutes--;
            }
            if (Minutes == 0 && Seconds == 0)
            {
                ( sender as System.Windows.Forms.Timer ).Enabled = false;
                Seconds = 59;
                Minutes = 2;
                Main_Grid.Visibility = Grid_Math_Edit.Visibility = Grid_Memory_Edit.Visibility = Visibility.Visible;
                InfoWindow window = new InfoWindow();
                window.WindowState = WindowState.Normal;
                window.Show();
                Right = Wrong = 0;
                Grid_Math.Visibility = Grid_Math_Work.Visibility = Grid_Memory.Visibility = Grid_Memory_Work.Visibility = Visibility.Hidden;
            }
            TimerLabel.Content = TimerLabel_Copy.Content = ( Seconds < 10 ) ? $"0{Minutes}:0{Seconds}" : $"0{Minutes}:{Seconds}";
        }

        private void Link_Click(object sender, MouseButtonEventArgs e)//Method which calls when user click on a link
        {
            Main_Grid.Visibility = Grid_Math_Work.Visibility = Grid_Memory_Work.Visibility = Visibility.Hidden;//Set nesassery grid visibility to hidden
            foreach (Grid grid in LinkToGrids [sender as Label])
            {
                grid.Visibility = Visibility.Visible;//set another to visible
            }
        }
        private void Link_ChangeColor(object sender, MouseEventArgs e)//And this calls when mouse enter/leave on focus of link
        {
            ( sender as Label ).Foreground = ( isBlack = !isBlack ) ? Brushes.Black : Brushes.LightCoral; //1 line code to change color of link 
        }


        private void Seconds_Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Label_Seconds.Content = $"{Seconds_Slider.Value.ToInt32()} секунды";
        }
        private void Numbers_Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Label_Numbers.Content = $"{Numbers_Slider.Value.ToInt32()} числа";
        }

        private void UserAnswer_Memory_KeyDown(object sender, KeyEventArgs e)
        {
            bool keyIsEnter = e.Key == Key.Enter;
            bool LengthAreMatch = UserAnswer_Memory.Text.Length == Expression_Memory.Content.ToString().Length;
            bool AsNumber = int.TryParse(UserAnswer_Memory.Text, out int a);
            if (keyIsEnter && LengthAreMatch && AsNumber)
            {
                new MemoryWorker(UserAnswer_Memory.Text.ToInt32(), Expression_Memory.Content.ToInt32()).CheckAnswer();
                Expression_Memory.Content = new MemoryWorker().NewExpression();
                Expression_Memory.Visibility = Visibility.Visible;
                timer2.Enabled = true;
                UserAnswer_Memory.Text = "";
            }
        }

        private void BeginWork_Memory_Click(object sender, RoutedEventArgs e)
        {
            timer.Enabled = true;
            timer2.Interval = 1000;
            timer2.Enabled = true;
            Grid_Memory_Edit.Visibility = Visibility.Hidden;
            Grid_Memory_Work.Visibility = Visibility.Visible;
            SecondsForMemory = Seconds_Slider.Value.ToInt32();
            NumbersForMemory = Numbers_Slider.Value.ToInt32();
            Expression_Memory.Content = ( new MemoryWorker().NewExpression() );

        }

        private void BeginButton_Math_Click(object sender, RoutedEventArgs e)
        {
            ActiveOperations.Clear();
            UserAnswer_Math.Text = "";
            timer.Enabled = true;
            foreach (CheckBox checkBox in AllOperations)
            {
                if (checkBox.IsChecked == true)
                    ActiveOperations.Add(checkBox.Content.ToString() [checkBox.Content.ToString().Length - 1]);
            }
            if (ActiveOperations.Count >= 1)
                Expression_Math.Content = new MathWorker(ActiveOperations).NewExpression();
            else
            {
                MessageBox.Show("Потрібно обрати хочаб 1 операцію.");
                return;
            }
            Grid_Math_Edit.Visibility = Visibility.Hidden;
            Grid_Math_Work.Visibility = Visibility.Visible;

        }

        private void UserAnswer_Math_KeyPressed(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && UserAnswer_Math.Text != "")
            {
                try
                {
                    int a;
                    if (int.TryParse(UserAnswer_Math.Text, out a))
                    {
                        new MathWorker(UserAnswer_Math.Text.Eval(), Expression_Math).CheckAnswer();
                        Expression_Math.Content = ( new MathWorker(ActiveOperations).NewExpression() );
                    }
                    else
                        UserAnswer_Math.Text = "";
                }
                catch (EvaluateException)
                {
                }
                finally
                {
                    UserAnswer_Math.Text = "";

                }
            }
        }
    }
    interface IWorker
    {
        string NewExpression();
        void CheckAnswer();
    }
    class MathWorker : IWorker
    {
        private int UserAnswer { get; set; }
        private int RealAnswer { get; }
        private List<char> Operations;
        public MathWorker(int _userAnswer, Label labelWithExpression)
        {
            RealAnswer = labelWithExpression.Content.ToString().Eval();
            UserAnswer = _userAnswer;
        }
        public MathWorker(List<char> _operations) => Operations = _operations;

        public string NewExpression()
        {
            Random random = new Random();
            int num1, num2;
            char op = Operations [random.Next(0, Operations.Count)];
            switch (op)
            {
                case '+':
                case '-':

                    num1 = random.Next(2, 51);
                    num2 = random.Next(2, 51);
                    if (num1 < num2)
                    {
                        ExMethods.Swap(ref num1, ref num2);
                    }
                    break;
                case '/':
                    do
                    {
                        num1 = random.Next(4, 82);
                        num2 = random.Next(4, 10);
                    } while (num1 % num2 != 0 || num1 / num2 == 1 || num1 / num2 > 10);
                    break;
                default:
                    num1 = random.Next(3, 12);
                    num2 = random.Next(3, 12);
                    break;
            }
            return $"{num1}{op}{num2}";
        }

        public void CheckAnswer()
        {
            if (RealAnswer == UserAnswer)
            {
                MainWindow.Right++;
            }
            else
            {
                MainWindow.Wrong++;
            }
        }
    }
    class MemoryWorker : IWorker
    {
        private readonly int toCheck;
        private readonly int realAnswer;
        public MemoryWorker() { }
        public MemoryWorker(int _toCheck, int _realAnswer)
        {
            toCheck = _toCheck;
            realAnswer = _realAnswer;
        }
        public void CheckAnswer()
        {
            if (toCheck == realAnswer)
                MainWindow.Right++;
            else
                MainWindow.Wrong++;
        }

        public string NewExpression()
        {
            string range = "1";
            for (int i = 0; i < MainWindow.NumbersForMemory - 1; i++)
            {
                range += "0";
            }
            return new Random().Next(range.ToInt32(), ( range + "0" ).ToInt32()).ToString();
        }
    }
    static class ExMethods//Bez komentariev dla lichnogo udobstva
    {
        public static int ToInt32<T>(this T self)
        {
            return Convert.ToInt32(self);
        }
        public static void Swap<T>(ref T one, ref T two)
        {
            T temp = one;
            one = two;
            two = temp;
        }
        public static int Eval(this string str)
        {
            try
            {
                return Convert.ToInt32(new DataTable().Compute(str, null));
            }
            catch (SyntaxErrorException)
            {
                return -1;
            }
        }
        public static void InitializeArray<T>(this T [] array, params T [] elems)
        {
            for (int i = 0; i < array.Length; i++)
            {
                array [i] = elems [i];
            }
        }
    }
}
