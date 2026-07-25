using System.Windows;
using System.Windows.Input;

namespace Reservo.Behavior
{
    public static class DropBehavior
    {
        public static readonly DependencyProperty IsDraggingProperty =
        DependencyProperty.RegisterAttached(
        "IsDragging",
        typeof(bool),
        typeof(DropBehavior),
        new PropertyMetadata(false));

        public static void SetIsDragging(DependencyObject obj, bool value)
        {
            obj.SetValue(IsDraggingProperty, value);
        }

        public static bool GetIsDragging(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsDraggingProperty);
        }

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached(
            "Command",
            typeof(ICommand),
            typeof(DropBehavior),
            new PropertyMetadata(null, OnCommandChanged));

        public static void SetCommand(DependencyObject element, ICommand value)
        {
            element.SetValue(CommandProperty, value);
        }

        public static ICommand GetCommand(DependencyObject element)
        {
            return (ICommand)element.GetValue(CommandProperty);
        }

        private static void OnCommandChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            if (d is UIElement element)
            {
                element.AllowDrop = true;

                element.DragOver += Element_DragOver;
                element.Drop += Element_Drop;
                element.DragEnter += Element_DragEnter;
                element.DragLeave += Element_DragLeave;
            }
        }

        private static void Element_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.Text))
            {
                e.Effects = DragDropEffects.Copy;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }

            e.Handled = true;
        }

        private static void Element_Drop(object sender, DragEventArgs e)
        {
            if (sender is DependencyObject element &&
                e.Data.GetDataPresent(DataFormats.Text))
            {
                string text = e.Data.GetData(DataFormats.Text) as string;

                var command = GetCommand(element);

                if (command?.CanExecute(text) == true)
                {
                    command.Execute(text);
                }
                SetIsDragging(element, false);
            }
        }

        private static void Element_DragEnter(object sender, DragEventArgs e)
        {
            if (sender is DependencyObject d)
                SetIsDragging(d, true);
        }

        private static void Element_DragLeave(object sender, DragEventArgs e)
        {
            if (sender is DependencyObject d)
                SetIsDragging(d, false);
        }
    }
}
