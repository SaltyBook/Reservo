using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Reservo.ViewModels
{
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        //Sets a field to a new value and notifies the UI if the value has actually changed
        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
            {
                return false;
            }

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        //Sets a field to a new value and additionally notifies dependent properties
        protected bool SetProperty<T>(ref T field, T value, string[] dependentProperties, [CallerMemberName] string? propertyName = null)
        {
            if (!SetProperty(ref field, value, propertyName))
            {
                return false;
            }

            foreach (var dependentProperty in dependentProperties)
            {
                OnPropertyChanged(dependentProperty);
            }

            return true;
        }
    }
}
