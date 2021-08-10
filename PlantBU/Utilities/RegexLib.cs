using System.Text.RegularExpressions;

namespace PlantBU.Utilities
{
    class RegexLib
    {
        /// <summary>
        /// The equipment code pattern
        /// </summary>
        const string EquipmentCodePattern = @"^([0-9])([a-z A-Z 0-9])([a-z A-Z 0-9])([a-z A-Z]){2}([0-9]){2}$";
        /// <summary>
        /// The component code pattern
        /// </summary>
        const string ComponentCodePattern = @"^([0-9])([a-z A-Z 0-9])([a-z A-Z 0-9])([a-z A-Z]){2}([0-9]){2}([a-z A-Z]){2}([a-z A-Z 0-9]){1,3}$";
        /// <summary>
        /// The check inventory code pattern
        /// </summary>
        const string CheckInventoryCodePattern = @"^\d{10}$";
        //Matches an integer with optional sign. Does NOT allow comma separated digit groups.
        /// <summary>
        /// The check integer pattern
        /// </summary>
        const string CheckIntegerPattern = @"^\d+$";
        //Matches a floating point number literal.
        /// <summary>
        /// The checkfloat pattern
        /// </summary>
        const string CheckfloatPattern = @"^(?:-(?:[1-9](?:\d{0,2}(?:,\d{3})+|\d*))|(?:0|(?:[1-9](?:\d{0,2}(?:,\d{3})+|\d*))))(?:.\d+|)$";
        //Matches "truthy" or "falsy" text values 1/0, y/n, yes/no, t/f, true/false or on/off.
        /// <summary>
        /// The check bool pattern
        /// </summary>
        const string CheckBoolPattern = @"^(?:(1|y(?:es)?|t(?:rue)?|on)|(0|n(?:o)?|f(?:alse)?|off))$";
        //Matches a valid date, in either MM/DD/YYYY or MM-DD-YYYY format.
        /// <summary>
        /// The check date pattern
        /// </summary>
        const string CheckDatePatternAmerican = @"\b(0?[1-9]|1[012])([\/\-])(0?[1-9]|[12]\d|3[01])\2(\d{4})";
        const string CheckDatePatternEnglishUK = @"(0?[1-9]|[12][0-9]|3[01])[- /.](0?[1-9]|1[012])[- /.](19|20)\d\d";

        //Matches the name of a month in English.  Abbreviated (e.g. Jan) and full names are allowed.
        /// <summary>
        /// The check month pattern
        /// </summary>
        const string CheckMonthPattern = @"/^(?:Jan(?:uary)?|Feb(?:ruary)?|Mar(?:ch)?|Apr(?:il)?|May|June?|July?|Aug(?:ust)?|Sep(?:tember)?|Oct(?:ober)?|Nov(?:ember)?|Dec(?:ember)?)$/i";
        //Matches a valid English day of the week. It matches short names as well as long names.
        /// <summary>
        /// The check week day pattern
        /// </summary>
        const string CheckWeekDayPattern = @"/^(?:sun(?:day)?|mon(?:day)?|tue(?:sday)?|wed(?:nesday)?|thu(?:rsday)?|fri(?:day)?|sat(?:urday)?)$/i";
        const string emailPattern = @"^([a-zA-Z0-9_\-\.]+)@([a-zA-Z0-9_\-\.]+)\.([a-zA-Z]{2,5})$";


        /// <summary>
        /// Determines whether [is equipment code] [the specified match string].
        /// </summary>
        /// <param name="MatchString">The match string.</param>
        /// <returns><c>true</c> if [is equipment code] [the specified match string]; otherwise, <c>false</c>.</returns>
        public static bool IsEquipmentCode(string MatchString)
        {
            return new Regex(EquipmentCodePattern).IsMatch(MatchString);
        }
        /// <summary>
        /// Determines whether [is component code] [the specified match string].
        /// </summary>
        /// <param name="MatchString">The match string.</param>
        /// <returns><c>true</c> if [is component code] [the specified match string]; otherwise, <c>false</c>.</returns>
        public static bool IsComponentCode(string MatchString)
        {
            return new Regex(ComponentCodePattern).IsMatch(MatchString);

        }
        /// <summary>
        /// Determines whether [is inventory code] [the specified match string].
        /// </summary>
        /// <param name="MatchString">The match string.</param>
        /// <returns><c>true</c> if [is inventory code] [the specified match string]; otherwise, <c>false</c>.</returns>
        public static bool IsInventoryCode(string MatchString)
        {
            return new Regex(CheckInventoryCodePattern).IsMatch(MatchString);

        }
        /// <summary>
        /// Determines whether the specified match string is integer.
        /// </summary>
        /// <param name="MatchString">The match string.</param>
        /// <returns><c>true</c> if the specified match string is integer; otherwise, <c>false</c>.</returns>
        public static bool IsInteger(string MatchString)
        {
            return new Regex(CheckIntegerPattern).IsMatch(MatchString);

        }
        /// <summary>
        /// Determines whether the specified match string is float.
        /// </summary>
        /// <param name="MatchString">The match string.</param>
        /// <returns><c>true</c> if the specified match string is float; otherwise, <c>false</c>.</returns>
        public static bool IsFloat(string MatchString)
        {
            return new Regex(CheckfloatPattern).IsMatch(MatchString);

        }
        /// <summary>
        /// Determines whether the specified match string is bool.
        /// </summary>
        /// <param name="MatchString">The match string.</param>
        /// <returns><c>true</c> if the specified match string is bool; otherwise, <c>false</c>.</returns>
        public static bool IsBool(string MatchString)
        {
            return new Regex(CheckBoolPattern, RegexOptions.IgnoreCase).IsMatch(MatchString);

        }
        /// <summary>
        /// Determines whether the specified match string is date.
        /// </summary>
        /// <param name="MatchString">The match string.</param>
        /// <returns><c>true</c> if the specified match string is date; otherwise, <c>false</c>.</returns>
        public static bool IsDate(string MatchString)
        {
            return new Regex(CheckDatePatternAmerican).IsMatch(MatchString);

        }
        /// <summary>
        /// Determines whether the specified match string is month.
        /// </summary>
        /// <param name="MatchString">The match string.</param>
        /// <returns><c>true</c> if the specified match string is month; otherwise, <c>false</c>.</returns>
        public static bool IsMonth(string MatchString)
        {
            return new Regex(CheckMonthPattern).IsMatch(MatchString);

        }
        /// <summary>
        /// Determines whether [is week day] [the specified match string].
        /// </summary>
        /// <param name="MatchString">The match string.</param>
        /// <returns><c>true</c> if [is week day] [the specified match string]; otherwise, <c>false</c>.</returns>
        public static bool IsWeekDay(string MatchString)
        {
            return new Regex(CheckWeekDayPattern).IsMatch(MatchString);

        }
        public static bool IsEmail(string MatchString)
        {
            return new Regex(emailPattern).IsMatch(MatchString);

        }
    }
}
