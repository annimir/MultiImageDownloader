using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace MultiImageDownloader.Converters
{
    public class IndexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            // Проверяем, что переданный объект — кнопка
            if (value is FrameworkElement element && element.Parent is StackPanel panel)
            {
                // Ищем родительский ItemsControl
                var itemsControl = FindParent<ItemsControl>(panel.Parent);
                if (itemsControl != null && itemsControl.Items.Count == 3)
                {
                    // Возвращаем индекс 0, 1 или 2
                    for (int i = 0; i < 3; i++)
                    {
                        if (itemsControl.Items[i] == panel.DataContext)
                            return i;
                    }
                }
            }
            return 0; // Дефолтное значение, если что-то пошло не так
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private static T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parent = VisualTreeHelper.GetParent(child);
            return parent is T foundParent ? foundParent : FindParent<T>(parent);
        }
    }
}
