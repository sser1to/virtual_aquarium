using System;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace virtual_aquarium
{
    public class Rock : Canvas
    {
        public int Type { get; set; }
        
        /// <summary>
        /// Создание изображения препятствия
        /// </summary>
        public Image AddObject()
        {
            Image image = new Image();

            BitmapImage bitmap = new BitmapImage();

            bitmap.BeginInit();
            switch (Type)
            {
                case 0:
                    bitmap.UriSource = new Uri(@"images\objects\скала.png", UriKind.Relative);
                    image.Width = 300;
                    break;
                case 1:
                    bitmap.UriSource = new Uri(@"images\objects\водоросли.png", UriKind.Relative);
                    image.Width = 300;
                    break;
                case 2:
                    bitmap.UriSource = new Uri(@"images\objects\ракушка.png", UriKind.Relative);
                    image.Width = 150;
                    break;
                default:
                    bitmap.UriSource = new Uri(@"images\objects\notfound.png", UriKind.Relative);
                    image.Width = 250;
                    break;
            }
            bitmap.EndInit();

            image.Source = bitmap;
            return image;
        }

        /// <summary>
        /// Создание прямоугольника и прикрелпение к нему изображения препятствия
        /// </summary>
        public Rectangle AddObjectRectangle(Image obj)
        {
            Rectangle rectangle = new Rectangle();

            switch (Type)
            {
                case 0:
                    rectangle.Width = 300;
                    rectangle.Height = 300;
                    break;
                case 1:
                    rectangle.Width = 300;
                    rectangle.Height = 300;
                    break;
                case 2:
                    rectangle.Width = 150;
                    rectangle.Height = 150;
                    break;
                default:
                    rectangle.Width = 250;
                    rectangle.Height = 250;
                    break;
            }
            
            rectangle.Fill = new ImageBrush(obj.Source);

            return rectangle;
        }
    }
}
