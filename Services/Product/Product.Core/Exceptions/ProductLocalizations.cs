namespace Product.Core.Exceptions;

public class ProductLocalizations
{
    private static readonly string Base = "Product";

    public static class Errors
    {
        private static readonly string BaseError = $"{Base}.Errors";

        public static class Validation
        {
            private static readonly string Error = $"{BaseError}.Validations";

            public static readonly string NameEmpty = $"{Error}.NameEmpty";
            public static readonly string SkuEmpty = $"{Error}.SkuEmpty";
            public static readonly string PriceNegative = $"{Error}.PriceNegative";
            public static readonly string StockNegative = $"{Error}.StockNegative";
        }

        public static class Business
        {
            private static readonly string Error = $"{BaseError}.Business";

            public static readonly string NotFound = $"{Error}.NotFound";
            public static readonly string SkuAlreadyExists = $"{Error}.SkuAlreadyExists";
            public static readonly string AlreadyDiscontinued = $"{Error}.AlreadyDiscontinued";
        }
    }
}
