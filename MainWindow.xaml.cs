
/*###########################################################*/
/*# Название программы: "Рыбы"                              #*/
/*#                                                         #*/
/*# Назначение программы: визуализация аквариума с рыбками. #*/
/*#                                                         #*/
/*# Разработчик: студент группы ПР-330/б Лецколюк Н.М.      #*/
/*#                                                         #*/
/*# Версия 1.0                                              #*/
/*###########################################################*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Media;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace virtual_aquarium
{
    public partial class MainWindow : Window
    {
        private List<Rectangle> fishes = new List<Rectangle>(); // Список созданных рыб
        private List<Rectangle> pikes = new List<Rectangle>();  // Список созданных щук
        private List<Rectangle> carps = new List<Rectangle>();  // Список созданных карпов
        private List<Rectangle> rocks = new List<Rectangle>();  // Список созданных препятствий

        private bool isDragging = false; // Переменная для проверки перемещения
        private Point lastPosition;      // Последняя позиция перетаскиваемого объекта
        private bool GraphicMode = true; // Проверка на включенный графический режим
        private Rectangle selectedRock;  // Выбранное препятствие

        private DispatcherTimer CollisionTimer;   // Таймер для проверки столкновений
        private DispatcherTimer CarpHuntingTimer; // Таймер для охоты щук на карпов

        Random rand = new Random();

        public MainWindow()
        {
            InitializeComponent();

            MyCanvas.MouseLeftButtonDown += MyCanvas_MouseLeftButtonDown;
            MyCanvas.MouseMove += MyCanvas_MouseMove;
            MyCanvas.MouseLeftButtonUp += MyCanvas_MouseLeftButtonUp;
            MyCanvas.MouseRightButtonDown += Canvas_MouseRightButtonDown;

            // Запуск цикличного таймера на проверку столкновений
            CollisionTimer = new DispatcherTimer();
            CollisionTimer.Interval = TimeSpan.FromMilliseconds(250);
            CollisionTimer.Tick += CheckCollision;
            CollisionTimer.Start();

            // Запуск цикличного таймера для охоты щук на карпов
            CarpHuntingTimer = new DispatcherTimer();
            CarpHuntingTimer.Interval = TimeSpan.FromSeconds(5);
            CarpHuntingTimer.Tick += (s, e) =>
            {
                foreach (var fish in pikes)
                {
                    Pike pk = new Pike();
                    pk.StartPikeChaseAnimation(carps, pikes, MyCanvas);
                }
            };
            CarpHuntingTimer.Start();

            PlayWavFilesAsync();
        }

        /// <summary>
        /// Цикличное воспроизведение музыки из папки "playlist"
        /// </summary>
        public async Task PlayWavFilesAsync()
        {
            string folderPath = "playlist";
            string[] files = Directory.GetFiles(folderPath, "*.wav");

            while (true)
            {
                foreach (string file in files)
                {
                    using (SoundPlayer player = new SoundPlayer(file))
                    {
                        await Task.Run(() => player.PlaySync());
                    }
                }
            }
        }

        private void GraphicModeOn(object sender, RoutedEventArgs e)
        {
            this.btn1.Visibility = Visibility.Hidden;
            this.btn1_1.Visibility = Visibility.Visible;
            this.btn4.Visibility = Visibility.Visible;
            this.btn5.Visibility = Visibility.Visible;
            this.btn6.Visibility = Visibility.Visible;
            this.btn7.Visibility = Visibility.Visible;
            this.btn8.Visibility = Visibility.Visible;
            this.rc1.Visibility = Visibility.Visible;
            this.rc2.Visibility = Visibility.Visible;
            this.tbl1.Visibility = Visibility.Visible;
            this.tbl2.Visibility = Visibility.Visible;
            GraphicMode = true;
        }

        private void GraphicModeOff(object sender, RoutedEventArgs e)
        {
            this.btn1.Visibility = Visibility.Visible;
            this.btn1_1.Visibility = Visibility.Hidden;
            this.btn4.Visibility = Visibility.Hidden;
            this.btn5.Visibility = Visibility.Hidden;
            this.btn6.Visibility = Visibility.Hidden;
            this.btn7.Visibility = Visibility.Hidden;
            this.btn8.Visibility = Visibility.Hidden;
            this.rc1.Visibility = Visibility.Hidden;
            this.rc2.Visibility = Visibility.Hidden;
            this.tbl1.Visibility = Visibility.Hidden;
            this.tbl2.Visibility = Visibility.Hidden;
        }

        private void AboutDeveloper(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(" Разработал студент группы ПР-330/Б" +
                "\n Лецколюк Никита" +
                "\n" +
                "\n Челябинск, 2023 год",
                "\n" +
                "О разработчике",
                 MessageBoxButton.OK,
                 MessageBoxImage.Question);

        }

        private void AboutProgram(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(" Рыбы:" +
                "\n" +
                "\n Программа представляет из себя вирутальный аквариум," +
                "\n в котором могут жить рыбки." +
                "\n" +
                "\n Для того, чтобы включить меню графического режима," +
                "\n нужно нажать на кнопку \"Графический режим\"." +
                "\n" +
                "\n Вы можете добавлять в аквариум рыбок и препятствия," +
                "\n создавать стаи щук и карпов." +
                "\n " +
                "\n Версия 1.0",
                "\n" +
                "О программе",
                 MessageBoxButton.OK,
                 MessageBoxImage.Question);
            Background.Position = TimeSpan.Zero;
            Background.Play();
        }

        private void CloseProgram(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Вы точно хотите закрыть программу?", "Закрытие", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown(); // Закрытие программы
            }
        }

        private void BtnAddFish(object sender, RoutedEventArgs e)
        {
            Parameters par = new Parameters();

            // Вызов окна для ввода параметров
            if (par.ShowDialog() == true)
            {
                Fish fish = new Fish { Amount = par.Amount, Type = par.Type };
                int amount = par.Amount;

                for (int i = 0; i < amount; i++)
                {
                    var fishUIElement = fish.AddFishRectangle(fish.AddFish());

                    // Размещение рыбки в центре MyCanvas
                    double left = (MyCanvas.ActualWidth - fishUIElement.ActualWidth) / 2;
                    double top = (MyCanvas.ActualHeight - fishUIElement.ActualHeight) / 2;
                    Canvas.SetLeft(fishUIElement, left);
                    Canvas.SetTop(fishUIElement, top);

                    fishes.Add(fishUIElement);
                    MyCanvas.Children.Add(fishUIElement);
                }
            }

            Fish fh = new Fish();

            foreach (var fish in fishes)
            {
                fh.StartAnimation(fish, MyCanvas);
            }
        }

        private void BtnAddCarp(object sender, RoutedEventArgs e)
        {
            Carp carp = new Carp();
            var carpUIElement = carp.AddFishRectangle(carp.AddFish());

            // Размещение карпа в центре MyCanvas
            double left = (MyCanvas.ActualWidth - carpUIElement.ActualWidth) / 2;
            double top = (MyCanvas.ActualHeight - carpUIElement.ActualHeight) / 2;
            Canvas.SetLeft(carpUIElement, left);
            Canvas.SetTop(carpUIElement, top);

            carps.Add(carpUIElement);
            MyCanvas.Children.Add(carpUIElement);

            foreach (var fish in carps)
            {
                Carp cr = new Carp();
                cr.StartCarpAnimation(carps, MyCanvas);
            }

            foreach (var fish in pikes)
            {
                Pike pk = new Pike();
                pk.StartPikeChaseAnimation(carps, pikes, MyCanvas);
            }
        }

        private void BtnAddPike(object sender, RoutedEventArgs e)
        {
            Pike pike = new Pike();
            var pikeUIElement = pike.AddFishRectangle(pike.AddFish());

            // Размещение щуки в центре MyCanvas
            double left = (MyCanvas.ActualWidth - pikeUIElement.ActualWidth) / 2;
            double top = (MyCanvas.ActualHeight - pikeUIElement.ActualHeight) / 2;
            Canvas.SetLeft(pikeUIElement, left);
            Canvas.SetTop(pikeUIElement, top);

            pikes.Add(pikeUIElement);
            MyCanvas.Children.Add(pikeUIElement);

            foreach (var fish in pikes)
            {
                Pike pk = new Pike();
                pk.StartPikeChaseAnimation(carps, pikes, MyCanvas);
            }
        }

        private void BtnAddObject(object sender, RoutedEventArgs e)
        {
            Parameters1 par1 = new Parameters1();

            // Вызов окна для ввода параметров
            if (par1.ShowDialog() == true)
            {
                Rock rock = new Rock { Type = par1.Type };
                var rockUIElement = rock.AddObjectRectangle(rock.AddObject());

                // Размещение препятствия в центре MyCanvas
                double left = (MyCanvas.ActualWidth - rockUIElement.ActualWidth) / 2;
                double top = (MyCanvas.ActualHeight - rockUIElement.ActualHeight) / 2;
                Canvas.SetLeft(rockUIElement, left);
                Canvas.SetTop(rockUIElement, top);

                rocks.Add(rockUIElement);
                MyCanvas.Children.Add(rockUIElement);
            }

        }

        private void BtnClear(object sender, RoutedEventArgs e)
        {
            // Очистка всех списков и MyCanvas
            MyCanvas.Children.Clear();
            fishes.Clear();
            rocks.Clear();
            pikes.Clear();
            carps.Clear();
        }

        /// <summary>
        /// Избежание столкновения с препятствиями
        /// </summary>
        private void CheckCollision(object sender, EventArgs e)
        {
            // Для рыбок
            foreach (Rectangle fish in fishes)
            {
                Rect fishRect = new Rect(Canvas.GetLeft(fish), Canvas.GetTop(fish), fish.Width, fish.Height);
                foreach (Rectangle other in fishes.Concat(rocks))
                {
                    if (other == fish)
                        continue;

                    Rect otherRect = new Rect(Canvas.GetLeft(other), Canvas.GetTop(other), other.Width, other.Height);
                    if (fishRect.IntersectsWith(otherRect))
                    {
                        if (rocks.Contains(other))
                        {
                            double dx = Canvas.GetLeft(fish) - Canvas.GetLeft(other);
                            double dy = Canvas.GetTop(fish) - Canvas.GetTop(other);

                            double length = Math.Sqrt(dx * dx + dy * dy);
                            dx /= length;
                            dy /= length;

                            Canvas.SetLeft(fish, Canvas.GetLeft(fish) + dx * 10);
                            Canvas.SetTop(fish, Canvas.GetTop(fish) + dy * 10);

                            Fish fh = new Fish();
                            fh.StartAnimation(fish, MyCanvas);
                        }
                    }
                }
            }

            // Для щук
            foreach (Rectangle pike in pikes)
            {
                Rect pikeRect = new Rect(Canvas.GetLeft(pike), Canvas.GetTop(pike), pike.Width, pike.Height);
                foreach (Rectangle other in pikes.Concat(rocks))
                {
                    if (other == pike)
                        continue;

                    Rect otherRect = new Rect(Canvas.GetLeft(other), Canvas.GetTop(other), other.Width, other.Height);
                    if (pikeRect.IntersectsWith(otherRect))
                    {
                        if (rocks.Contains(other))
                        {
                            double oldLeft = Canvas.GetLeft(pike);
                            double oldTop = Canvas.GetTop(pike);
                            Canvas.SetLeft(pike, oldLeft + (oldLeft - Canvas.GetLeft(other)));
                            Canvas.SetTop(pike, oldTop + (oldTop - Canvas.GetTop(other)));
                            Pike pk = new Pike();
                            pk.StartPikeChaseAnimation(carps, pikes, MyCanvas);
                        }
                    }
                }
            }

            // Для карпов
            foreach (Rectangle carp in carps)
            {
                Rect carpRect = new Rect(Canvas.GetLeft(carp), Canvas.GetTop(carp), carp.Width, carp.Height);
                foreach (Rectangle other in pikes.Concat(rocks))
                {
                    if (other == carp)
                        continue;

                    Rect otherRect = new Rect(Canvas.GetLeft(other), Canvas.GetTop(other), other.Width, other.Height);
                    if (carpRect.IntersectsWith(otherRect))
                    {
                        if (rocks.Contains(other))
                        {
                            double oldLeft = Canvas.GetLeft(carp);
                            double oldTop = Canvas.GetTop(carp);
                            Canvas.SetLeft(carp, oldLeft + (oldLeft - Canvas.GetLeft(other)));
                            Canvas.SetTop(carp, oldTop + (oldTop - Canvas.GetTop(other)));
                            Carp cr = new Carp();
                            cr.StartCarpAnimation(carps, MyCanvas);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Удаление объектов нажатием ПКМ
        /// </summary>
        private void Canvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (GraphicMode)
            {
                Point clickPoint = e.GetPosition(MyCanvas);

                foreach (Rectangle rect in fishes.Concat(rocks).Concat(pikes).Concat(carps))
                {
                    Rect rectBounds = new Rect(Canvas.GetLeft(rect), Canvas.GetTop(rect), rect.Width, rect.Height);
                    if (rectBounds.Contains(clickPoint))
                    {
                        MyCanvas.Children.Remove(rect);
                        fishes.Remove(rect);
                        rocks.Remove(rect);
                        pikes.Remove(rect);
                        carps.Remove(rect);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Начало перетаскивания препятствий с помощью зажатой ЛКМ
        /// </summary>
        private void MyCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var image = e.Source as Rectangle;

            if (image != null && rocks.Contains(image))
            {
                isDragging = true;
                selectedRock = image;
                lastPosition = e.GetPosition(MyCanvas);
                MyCanvas.CaptureMouse();
            }
        }

        /// <summary>
        /// Завершение перетаскивания препятствий с помощью зажатой ЛКМ
        /// </summary>
        private void MyCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (isDragging)
            {
                isDragging = false;
                selectedRock = null;
                MyCanvas.ReleaseMouseCapture();
            }
        }

        /// <summary>
        /// Перетаскивание препятствий с помощью зажатой ЛКМ
        /// </summary>
        private void MyCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging && selectedRock != null && GraphicMode)
            {
                var position = e.GetPosition(MyCanvas);
                var offset = position - lastPosition;
                lastPosition = position;

                var left = Canvas.GetLeft(selectedRock) + offset.X;
                var top = Canvas.GetTop(selectedRock) + offset.Y;

                Canvas.SetLeft(selectedRock, left);
                Canvas.SetTop(selectedRock, top);
            }
        }

        private void AddMusic(object sender, RoutedEventArgs e)
        {
            Process.Start(@"playlist");
        }
    }
}
