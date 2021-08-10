using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace PlantBU.DataModel.Annotations
{
    /// <summary>
    /// usage [IsEmpty(ErrorMessage = "Should not be null or empty.")]
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public sealed class IsEmptyAttribute : ValidationAttribute
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'IsEmptyAttribute.IsValid(object)'
        public override bool IsValid(object value)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'IsEmptyAttribute.IsValid(object)'
        {
            var inputValue = value as string;
            return !string.IsNullOrEmpty(inputValue);
        }
    }
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'WeekendValidation'
    public sealed class WeekendValidation : Attribute
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'WeekendValidation'
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'WeekendValidation.WeekendValidate(DateTime)'
        public static ValidationResult WeekendValidate(DateTime date)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'WeekendValidation.WeekendValidate(DateTime)'
        {
            return date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Friday
                ? new ValidationResult("The wekeend days aren't valid")
                : ValidationResult.Success;
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ValueCompare'
    public sealed class ValueCompare : ValidationAttribute
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ValueCompare'
    {
        private readonly string _chars;
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ValueCompare.ValueCompare(string)'
        public ValueCompare(string chars) : base("{0} contains invalid character.")
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ValueCompare.ValueCompare(string)'
        {
            _chars = chars;
        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ValueCompare.IsValid(object, ValidationContext)'
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ValueCompare.IsValid(object, ValidationContext)'
        {
            if (value != null)
            {
                for (int i = 0; i < _chars.Length; i++)
                {
                    var valueAsString = value.ToString();
                    if (valueAsString.Contains(_chars[i]))
                    {
                        var errorMessage = FormatErrorMessage(validationContext.DisplayName);
                        return new ValidationResult(errorMessage);
                    }
                }
            }
            return ValidationResult.Success;
        }
    }
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'EmailAttribute'
    public sealed class EmailAttribute : ValidationAttribute
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'EmailAttribute'
    {

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'EmailAttribute.IsValid(object)'
        public override bool IsValid(object value)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'EmailAttribute.IsValid(object)'
        {
            return new RegularExpressionAttribute(@"^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,6}$").IsValid(Convert.ToString(value).Trim());
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'StringRangeAttribute'
    public sealed class StringRangeAttribute : ValidationAttribute
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'StringRangeAttribute'
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'StringRangeAttribute.AllowableValues'
        public string[] AllowableValues { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'StringRangeAttribute.AllowableValues'

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'StringRangeAttribute.IsValid(object, ValidationContext)'
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'StringRangeAttribute.IsValid(object, ValidationContext)'
        {
            if (AllowableValues?.Contains(value?.ToString()) == true)
            {
                return ValidationResult.Success;
            }

            var msg = $"Please enter one of the allowable values: {string.Join(", ", (AllowableValues ?? new string[] { "No allowable values found" }))}.";
            return new ValidationResult(msg);
        }
    }
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'IntRangeAttribute'
    public sealed class IntRangeAttribute : ValidationAttribute
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'IntRangeAttribute'
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'IntRangeAttribute.WhiteList'
        public IEnumerable<int> WhiteList
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'IntRangeAttribute.WhiteList'
        {
            get;
        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'IntRangeAttribute.IntRangeAttribute(params int[])'
        public IntRangeAttribute(params int[] whiteList)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'IntRangeAttribute.IntRangeAttribute(params int[])'
        {
            WhiteList = new List<int>(whiteList);
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'IntRangeAttribute.IsValid(object, ValidationContext)'
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'IntRangeAttribute.IsValid(object, ValidationContext)'
        {
            if (WhiteList.Contains((int)value))
            {
                return ValidationResult.Success;
            }

            var msg = $"Must have one of these values: {String.Join(",", WhiteList)}";
            return new ValidationResult(msg);
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'DateGreaterThanAttribute'
    public sealed class DateGreaterThanAttribute : ValidationAttribute
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'DateGreaterThanAttribute'
    {
        private string DateToCompareFieldName { get; set; }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'DateGreaterThanAttribute.DateGreaterThanAttribute(string)'
        public DateGreaterThanAttribute(string dateToCompareFieldName)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'DateGreaterThanAttribute.DateGreaterThanAttribute(string)'
        {
            DateToCompareFieldName = dateToCompareFieldName;
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'DateGreaterThanAttribute.IsValid(object, ValidationContext)'
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'DateGreaterThanAttribute.IsValid(object, ValidationContext)'
        {
            DateTime laterDate = (DateTime)value;

            DateTime earlierDate = (DateTime)validationContext.ObjectType.GetProperty(DateToCompareFieldName).GetValue(validationContext.ObjectInstance, null);

            if (laterDate > earlierDate)
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult(string.Format("{0} precisa ser menor!", DateToCompareFieldName));
            }
        }
    }
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ListStringAllowNoRepeatedAttribute'
    public sealed class ListStringAllowNoRepeatedAttribute : ValidationAttribute
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ListStringAllowNoRepeatedAttribute'
    {

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ListStringAllowNoRepeatedAttribute.ListStringAllowNoRepeatedAttribute()'
        public ListStringAllowNoRepeatedAttribute()
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ListStringAllowNoRepeatedAttribute.ListStringAllowNoRepeatedAttribute()'
        {
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ListStringAllowNoRepeatedAttribute.IsValid(object, ValidationContext)'
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ListStringAllowNoRepeatedAttribute.IsValid(object, ValidationContext)'
        {
            IEnumerable<string> duplicates = (value as List<string>).GroupBy(x => x)
                                            .Where(g => g.Count() > 1)
                                            .Select(x => x.Key);
            return duplicates.Count() > 0
               ? new ValidationResult("Duplicate item found!")
               : ValidationResult.Success;


        }
    }
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    
#pragma warning disable CS1587 // XML comment is not placed on a valid language element
/// <summary>
    /// Performs validation on a member if the property with the specified name (and on the same instance as the member we are validating) equals one of
    /// the values provided in this constructor.
    /// usage : [RequiredIf("IsLastNameRequired", true, ErrorMessage = "Last name is required.")]
    /// </summary>
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'RequiredIfAttribute'
    public sealed class RequiredIfAttribute : ValidationAttribute
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'RequiredIfAttribute'
#pragma warning restore CS1587 // XML comment is not placed on a valid language element
    {
        /// <summary>
        /// The name of the property which must equal one of 'Values' for validation to occur.
        /// </summary>
        public string PropertyName { get; private set; }

        /// <summary>
        /// Gets or sets the values which when 'PropertyName' matches one or more of them, validation will occur.
        /// </summary>
        public object[] Values { get; private set; }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'RequiredIfAttribute.RequiredIfAttribute(string, params object[])'
        public RequiredIfAttribute(string propertyName, params object[] equalsValues)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'RequiredIfAttribute.RequiredIfAttribute(string, params object[])'
        {
            this.PropertyName = propertyName;
            this.Values = equalsValues;
        }

        /// <summary>
        /// Performs the conditional validation.
        /// </summary>
        /// <param name="value">This is the actual value of the property who has the RequiredIf attribute on it (e.g. the property that we are validating).</param>
        /// <param name="validationContext">The validation context which contains the instance that owns the member which we are validating.</param>
        /// <returns>Returns ValidationResult.Success upon success, and a ValidationResult with the specified ErrorMessage upon failure.</returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            bool isRequired = this.IsRequired(validationContext);

            if (isRequired && string.IsNullOrEmpty(Convert.ToString(value)))
                return new ValidationResult(this.ErrorMessage);
            return ValidationResult.Success;
        }

        /// <summary>
        /// Determines whether or not validation should occur.
        /// </summary>
        /// <param name="validationContext">The validation context which contains the instance that owns the member which we are validating.</param>
        /// <returns>Returns true if validation should occur, false otherwise.</returns>
        private bool IsRequired(ValidationContext validationContext)
        {
            var property = validationContext.ObjectType.GetProperty(this.PropertyName);
            var currentValue = property.GetValue(validationContext.ObjectInstance, null);

            foreach (var val in this.Values)
                if (object.Equals(currentValue, val))
                    return true;
            return false;
        }
    }

}
