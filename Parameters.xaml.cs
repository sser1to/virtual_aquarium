using System;
using System.Windows;

namespace virtual_aquarium
{
    /// <summary>
    /// Логика взаимодействия для Parameters.xaml
    /// </summary>
    public partial class Parameters : Window
    {

        public int Amount { get; set; }
        public int Type { get; set; }

        public Parameters()
        {
            InitializeComponent();

            // Запрет на изменение размера окна
            this.ResizeMode = ResizeMode.NoResize;
            this.MaxHeight = this.Height;
            this.MaxWidth = this.Width;
        }


        public void BtnOK(object sender, RoutedEventArgs e) 
        {
            // Проверка вводимого значения
            try
            {
                Amount = int.Parse(SpeedTextBox.Text);
                if (this.Amount >= 1 && this.Amount <= 10)
                {
                    int amount;
                    bool isInt = int.TryParse(SpeedTextBox.Text, out amount);
                    if (isInt)
                    {
                        this.DialogResult = true;
                    }
                    else
                    {
                        throw new Exception("Вы ввели некорректное количество!");
                    }
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Введите число от 1 до 10!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                Type = cbx.SelectedIndex;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        } 

        private void BtnCancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
