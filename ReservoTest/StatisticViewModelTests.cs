using Reservo.ViewModels;

namespace ReservoTest
{
    public class StatisticViewModelTests
    {
        [Fact]
        public void CalculateAverage_ReturnsZeroWhenThereAreNoReservations()
        {
            Assert.Equal(0, StatisticViewModel.CalculateAverage(0, 0));
        }

        [Fact]
        public void CalculateAverage_ReturnsIntegerAverage()
        {
            Assert.Equal(3, StatisticViewModel.CalculateAverage(10, 3));
        }
    }
}