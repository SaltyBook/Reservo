namespace Reservo.Infrastructure
{
    public static class ContactValues
    {
        public static Array All => Enum.GetValues(typeof(Contact));

        public enum Contact
        {
            Gruppenhaus = 1,
            Gruppenunterkünfte,
            Sonstiges
        }
    }
}
