using Reservo.Helpers;
using Reservo.Infrastructure;
using System.IO;
using System.Text.Json;
using System.Windows.Controls;

namespace Reservo
{
    public static class ColumnOrderService
    {
        private static string FilePath => Path.Combine(Paths.ResourcesPath, "columnOrder.json");

        //Loads the column order for the datagrid
        public static List<ColumnOrderInfo>? Load()
        {
            if (!File.Exists(FilePath))
                return null;

            var json = File.ReadAllText(FilePath);
            return JsonSerializer.Deserialize<List<ColumnOrderInfo>>(json);
        }

        //Saves the column order from the current loaded datagrid
        public static void Save(DataGrid grid)
        {
            var order = grid.Columns
                .Select(c => new ColumnOrderInfo
                {
                    Key = c.Header?.ToString() ?? "",
                    DisplayIndex = c.DisplayIndex
                })
                .ToList();

            var json = JsonSerializer.Serialize(order, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(FilePath, json);
        }

        //Applies the column order to the datagrid
        public static void Apply(DataGrid grid, List<ColumnOrderInfo> order)
        {
            var sorted = order.OrderBy(o => o.DisplayIndex).ToList();
            int index = 0;

            foreach (var info in sorted)
            {
                var column = grid.Columns.FirstOrDefault(c =>
                    c.Header?.ToString() == info.Key);

                if (column is not null)
                    column.DisplayIndex = index++;
            }
        }
    }
}
