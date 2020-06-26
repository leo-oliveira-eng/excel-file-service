namespace Excel.File.Service.Service.Extensions
{
    internal static class NumberExtensions
    {
        internal static string ToLetterRepresentation(this int columnIndex)
        {
            if (columnIndex < 0)
                return "#REF!";

            var column = string.Empty;

            while (columnIndex > 0)
            {
                column = ((char)('A' + (columnIndex % 26))).ToString() + column;
                columnIndex = (columnIndex - (columnIndex % 26)) / 26;
            }

            return column;
        }
    }
}
