#region Usings
using Reservo.ViewModels;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
#endregion

namespace Reservo.Views
{
    /// <summary>
    /// Interaction logic for InvoiceWindow.xaml
    /// </summary>
    public partial class InvoiceWindow : Window
    {
        public InvoiceWindow(InvoiceViewModel vm)
        {
            InitializeComponent();

            DataContext = vm;
        }

        // Regex pattern used to allow only positive integer input (digits 0-9) in the Quantity TextBox.
        private static readonly Regex _onlyPositiveIntegers = new Regex("^[0-9]+$");

        // Event handler for the TextBox PreviewTextInput event.
        // Blocks any input that does not match the _onlyPositiveIntegers regex.
        // Ensures only positive integer values can be typed into the Quantity field.
        private void QuantityTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !_onlyPositiveIntegers.IsMatch(e.Text);
        }

        // Event handler for the DataGrid PreviewKeyDown event.
        // Overrides the default behavior of Enter and Tab keys to navigate between rows.
        // If Shift is held, moves to the previous row; otherwise moves to the next row.
        // Selects the target row and sets focus on the current column for editing.
        private void DataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Tab)
            {
                e.Handled = true;

                var grid = (DataGrid)sender;
                var currentCell = grid.CurrentCell;
                int currentRow = grid.Items.IndexOf(currentCell.Item);
                int nextRow = Keyboard.Modifiers == ModifierKeys.Shift ? currentRow - 1 : currentRow + 1;

                if (nextRow >= 0 && nextRow < grid.Items.Count)
                {
                    grid.SelectedItem = grid.Items[nextRow];
                    grid.CurrentCell = new DataGridCellInfo(grid.Items[nextRow], currentCell.Column);
                    grid.BeginEdit();
                }
            }
        }
    }
}
