namespace THPARKING.Business.InOut
{
    public class DuplicatePlateChecker
    {
        public bool IsPlateMismatch(string expectedPlateNormalized, string actualPlateNormalized)
        {
            var expected = Normalize(expectedPlateNormalized);
            var actual = Normalize(actualPlateNormalized);

            if (string.IsNullOrWhiteSpace(expected))
                return false;

            if (string.IsNullOrWhiteSpace(actual))
                return false;

            return expected != actual;
        }

        public string Normalize(string plateNumber)
        {
            if (string.IsNullOrWhiteSpace(plateNumber))
                return null;

            return plateNumber
                .Trim()
                .Replace(" ", string.Empty)
                .Replace(".", string.Empty)
                .Replace("-", string.Empty)
                .ToUpperInvariant();
        }
    }
}