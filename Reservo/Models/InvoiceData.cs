namespace Reservo.Models
{
    public class InvoiceData
    {
        public DateTime InvoiceDate = DateTime.Now;

        public string Year { get; set; }
        public Entry Entry { get; set; }
        public string Price1 { get; set; }
        public int Children { get; set; }
        public int Youth { get; set; }
        public int Adults { get; set; }
        public int FromTwentySeven { get; set; }
        public string Price2 { get; set; }
        public int UpToTwentySix { get; set; }
        public string Price3 { get; set; }
        public int UpToSeventeen { get; set; }
        public string Price4 { get; set; }
        public int Guest { get; set; }
        public string Price5 { get; set; }
        public int Heater { get; set; }
        public string Price6 { get; set; }
        public int Bedding { get; set; }
        public string Price7 { get; set; }
        public int Laundry { get; set; }
        public string Price8 { get; set; }
        public int Tent { get; set; }
        public string Price9 { get; set; }
        public int Wood { get; set; }
        public string Price10 { get; set; }
        public string Price11 { get; set; }
        public string Price12 { get; set; }
        public string AdditionName { get; set; }
        public string Price13 { get; set; }
        public string TotalPrice { get; set; }
    }
}
