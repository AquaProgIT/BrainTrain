using System;
using System.Windows;

namespace Brian_Release
{
    /// <summary>
    /// Логика взаимодействия для InfoWindow.xaml
    /// </summary>
    public partial class InfoWindow : Window
	{
        public InfoWindow()
        {
            InitializeComponent();
            Foo(MainWindow.Right, MainWindow.Wrong);
        }
        public void Foo (int a, int b)
        {
            RightAnswers.Text = $"Правильних відповідей: {a}";
            WrongAnswers.Text = $"Неправильних відповідей: {b}";
            if (a + b != 0)
                Mark.Text = $"Oцінка: {12 - ( 12 * b ) / ( a + b )}";
            else
                Mark.Text = $"Oцiнка не задовiльна!";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
