using Reservo.ViewModels;

namespace ReservoTest
{
    public class BaseViewModelTests
    {
        [Fact]
        public void SetProperty_NotifiesOnlyWhenValueChanges()
        {
            var vm = new TestViewModel();
            var notifications = new List<string?>();
            vm.PropertyChanged += (_, e) => notifications.Add(e.PropertyName);

            vm.Value = 1;
            vm.Value = 1;

            Assert.Equal(1, vm.Value);
            Assert.Equal(new[] { nameof(TestViewModel.Value) }, notifications);
        }

        private sealed class TestViewModel : BaseViewModel
        {
            private int _value;

            public int Value
            {
                get => _value;
                set => SetProperty(ref _value, value);
            }
        }
    }
}
