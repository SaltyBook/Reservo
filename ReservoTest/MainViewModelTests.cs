using Reservo.ViewModels;

namespace ReservoTest
{
    public class MainViewModelTests
    {
        [Fact]
        public void Increment_IncreaseCounter()
        {
            var vm = new MainViewModel();

            vm.Increment();

            Assert.Equal(1, vm.Counter);
        }
    }
}