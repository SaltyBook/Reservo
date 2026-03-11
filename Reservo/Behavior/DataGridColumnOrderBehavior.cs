using System.Windows;
using System.Windows.Controls;

namespace Reservo.Behavior
{
    public static class DataGridColumnOrderBehavior
    {
        public static DataGrid LoadedGrid = null;

        public static readonly DependencyProperty EnableProperty =
            DependencyProperty.RegisterAttached(
                "Enable",
                typeof(bool),
                typeof(DataGridColumnOrderBehavior),
                new PropertyMetadata(false, OnEnableChanged));

        public static void SetEnable(DataGrid element, bool value) =>
            element.SetValue(EnableProperty, value);

        public static bool GetEnable(DataGrid element) =>
            (bool)element.GetValue(EnableProperty);

        private static void OnEnableChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            if (d is not DataGrid grid)
                return;

            LoadedGrid = grid;

            if ((bool)e.NewValue)
            {
                grid.Loaded += Grid_Loaded;
            }
            else
            {
                grid.Loaded -= Grid_Loaded;
            }
        }

        private static void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            var grid = (DataGrid)sender;

            var order = ColumnOrderService.Load();
            if (order != null)
                ColumnOrderService.Apply(grid, order);
        }
    }
}
