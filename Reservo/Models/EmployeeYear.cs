using System.Collections.ObjectModel;

namespace Reservo.Models
{
    public class EmployeeYear
    {
        public int Year { get; set; }

        public ObservableCollection<EmployeeHours> Employees { get; set; } = new();
    }
}
