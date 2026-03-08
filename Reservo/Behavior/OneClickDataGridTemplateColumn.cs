using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Reservo.Behavior
{
    public class OneClickDataGridTemplateColumn : DataGridTemplateColumn
    {
        protected override object? PrepareCellForEdit(FrameworkElement? editingElement, RoutedEventArgs editingEventArgs)
        {
            if (editingElement is { } && VisualTreeHelper.GetChild(editingElement, 0) is TextBox tb)
            {
                tb.Focus();
                tb.SelectAll();
            }
            return null;
        }
    }
}
