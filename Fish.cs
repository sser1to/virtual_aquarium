using System;
using System.Windows.Media;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;

namespace virtual_aquarium
{
    public class Fish : Canvas
    {
        public int Amount { get; set; }

        public int Type { get; set; }

        Random rand = new Random();

        /// <summary>
        /// Создание изображения рыбки
        /// </summary>
        public Image AddFish()
        {
            Image image = new Image();

            image.Stretch = Stretch.Fill;

            BitmapImage bitmap = new BitmapImage();

            image.Width = 150;

            bitmap.BeginInit();
            switch (Type)
            {
                case 0:
                    bitmap.UriSource = new Uri(@"images\fishes\голубой хирург.png", UriKind.Relative);
                    break;
                case 1:
                    bitmap.UriSource = new Uri(@"images\fishes\гуппи.png", UriKind.Relative);
                    break;
                case 2:
                    bitmap.UriSource = new Uri(@"images\fishes\рыба-ангел.png", UriKind.Relative);
                    break;
                case 3:
                    bitmap.UriSource = new Uri(@"images\fishes\цихлида.png", UriKind.Relative);
                    break;
                default:
                    bitmap.UriSource = new Uri(@"images\fishes\notfound.png" , UriKind.Relative);
                    break;
            }
            bitmap.EndInit();

            image.Source = bitmap;
            return image;
        }
        
        /// <summary>
        /// Создание прямоугольника и прикрепление к нему изображения рыбки
        /// </summary>
        public Rectangle AddFishRectangle(Image fish)
        {
            Rectangle rectangle = new Rectangle();

            switch (Type)
            {
                case 0:
                    rectangle.Height = 100;
                    break;
                case 1:
                    rectangle.Height = 100;
                    break;
                case 2:
                    rectangle.Height = 125;
                    break;
                case 3:
                    rectangle.Height = 100;
                    break;
                default:
                    rectangle.Height = 150;
                    break;
            }

            rectangle.Width = 150;

            rectangle.Fill = new ImageBrush(fish.Source);

            return rectangle;
        }

        /// <summary>
        /// Передвижение рыбки
        /// </summary>
        public void StartAnimation(Rectangle fish, Canvas MyCanvas)
        {
            DoubleAnimation da = new DoubleAnimation();
            double oldLeft = Canvas.GetLeft(fish);
            double newLeft = rand.Next(0, (int)(MyCanvas.ActualWidth - fish.ActualWidth - 50));
            da.From = oldLeft;
            da.To = newLeft;
            da.Duration = new Duration(TimeSpan.FromSeconds(rand.Next(3, 6)));
            da.Completed += (s, e) => StartAnimation(fish, MyCanvas);

            DoubleAnimation da2 = new DoubleAnimation();
            da2.From = Canvas.GetTop(fish);
            da2.To = rand.Next(0, (int)(MyCanvas.ActualHeight - fish.ActualHeight - 50));
            da2.Duration = new Duration(TimeSpan.FromSeconds(rand.Next(3, 6)));
            da2.Completed += (s, e) => StartAnimation(fish, MyCanvas);

            fish.BeginAnimation(Canvas.LeftProperty, da);
            fish.BeginAnimation(Canvas.TopProperty, da2);

            ScaleTransform flipTrans = new ScaleTransform();
            fish.RenderTransformOrigin = new Point(0.5, 0.5);
            fish.RenderTransform = flipTrans;

            if (newLeft < oldLeft)
            {
                flipTrans.ScaleX = 1;
            }
            else
            {
                flipTrans.ScaleX = -1;
            }
        }
    }
}
