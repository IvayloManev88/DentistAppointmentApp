using System;
using System.Collections.Generic;
using System.Text;

namespace DentistApp.GCommon
{
    public static class AppointmentConstants
    {
        public const int NoteMaxLength = 300;
        public const string PhoneRegexValidation = @"^(\+359|0)\d{9}$";
        public const int PhoneNumberMaxLength = 16;
    }
}
