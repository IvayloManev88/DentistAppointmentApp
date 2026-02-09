namespace DentistApp.GCommon
{
    public static class EntityConstants
    {
        public static class ApplicationUser
        {
            /// <summary>
            /// First Name should be a text with lenght not greater than 50
            /// </summary>
            public const int ApplicationUserFirstNameMaxLength = 50;

            /// <summary>
            /// First Name should be a text with lenght at least 2
            /// </summary>
            public const int ApplicationUserFirstNameMinLength = 2;


            /// <summary>
            /// Last Name should be a text with lenght not greater than 50
            /// </summary>
            public const int ApplicationUserLastNameMaxLength = 50;

            /// <summary>
            /// Last Name should be a text with lenght at least 2
            /// </summary>
            public const int ApplicationUserLastNameMinLength = 2;
        }

        public static class Manipulation
        {
            /// <summary>
            /// Manipulation name should be a text with lenght not greater than 100
            /// </summary>
            public const int ManipulationNameMaxLength = 100;

            /// <summary>
            /// Manipulation name be a text with lenght at least 2
            /// </summary>
            public const int ManipulationNameMinLength = 2;


            /// <summary>
            /// Expexted price should be a text with lenght not greater than 10. It will have values like 50-100
            /// </summary>
            public const int ExpectedPriceMaxLenght = 10;

            /// <summary>
            /// Expexted price should be a text with with lenght at least 1 symbol long
            /// </summary>
            public const int ApplicationUserLastNameMinLength = 1;
        }
        public static class Appointment
        {
            /// <summary>
            /// Phone number length should not exceed 16 symbols
            /// </summary>
            public const int PhoneNumberMaxLenght = 16;
            /// <summary>
            /// Note length should not exceed 450 symbols
            /// </summary>
            public const int NoteMaxLength = 450;
        }
    }
}
