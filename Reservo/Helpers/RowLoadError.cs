namespace Reservo.Helpers
{
    public class RowLoadError
    {
        public int RowNumber { get; }
        public string Message { get; }
        public Exception? Exception { get; }

        public RowLoadError(int rowNumber, string message, Exception? exception = null)
        {
            RowNumber = rowNumber;
            Message = message;
            Exception = exception;
        }

        public override string ToString()
        {
            return $"Zeile {RowNumber}: {Message}";
        }
    }
}
