using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;

namespace PlantBU.Utilities
{
    class Validation
    {
    }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'IValidatable<T>'
    public interface IValidatable<T> : INotifyPropertyChanged
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'IValidatable<T>'
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'IValidatable<T>.Validations'
        List<IValidationRule<T>> Validations { get; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'IValidatable<T>.Validations'

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'IValidatable<T>.Errors'
        List<string> Errors { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'IValidatable<T>.Errors'

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'IValidatable<T>.Validate()'
        bool Validate();
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'IValidatable<T>.Validate()'

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'IValidatable<T>.IsValid'
        bool IsValid { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'IValidatable<T>.IsValid'
    }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ValidatableObject<T>'
    public class ValidatableObject<T> : IValidatable<T>
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ValidatableObject<T>'
    {
#pragma warning disable CS0067 // The event 'ValidatableObject<T>.PropertyChanged' is never used
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ValidatableObject<T>.PropertyChanged'
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ValidatableObject<T>.PropertyChanged'
#pragma warning restore CS0067 // The event 'ValidatableObject<T>.PropertyChanged' is never used

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ValidatableObject<T>.Validations'
        public List<IValidationRule<T>> Validations { get; } = new List<IValidationRule<T>>();
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ValidatableObject<T>.Validations'

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ValidatableObject<T>.Errors'
        public List<string> Errors { get; set; } = new List<string>();
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ValidatableObject<T>.Errors'

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ValidatableObject<T>.CleanOnChange'
        public bool CleanOnChange { get; set; } = true;
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ValidatableObject<T>.CleanOnChange'

        T _value;
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ValidatableObject<T>.Value'
        public T Value
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ValidatableObject<T>.Value'
        {
            get => _value;
            set
            {
                _value = value;

                if (CleanOnChange)
                    IsValid = true;
            }
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ValidatableObject<T>.IsValid'
        public bool IsValid { get; set; } = true;
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ValidatableObject<T>.IsValid'

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ValidatableObject<T>.Validate()'
        public virtual bool Validate()
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ValidatableObject<T>.Validate()'
        {
            Errors.Clear();

            IEnumerable<string> errors = Validations.Where(v => !v.Check(Value))
                .Select(v => v.ValidationMessage);

            Errors = errors.ToList();
            IsValid = !Errors.Any();

            return this.IsValid;
        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ValidatableObject<T>.ToString()'
        public override string ToString()
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ValidatableObject<T>.ToString()'
        {
            return $"{Value}";
        }
    }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'IValidationRule<T>'
    public interface IValidationRule<T>
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'IValidationRule<T>'
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'IValidationRule<T>.ValidationMessage'
        string ValidationMessage { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'IValidationRule<T>.ValidationMessage'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'IValidationRule<T>.Check(T)'
        bool Check(T value);
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'IValidationRule<T>.Check(T)'
    }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'IsNotNullOrEmptyRule<T>'
    public class IsNotNullOrEmptyRule<T> : IValidationRule<T>
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'IsNotNullOrEmptyRule<T>'
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'IsNotNullOrEmptyRule<T>.ValidationMessage'
        public string ValidationMessage { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'IsNotNullOrEmptyRule<T>.ValidationMessage'

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'IsNotNullOrEmptyRule<T>.Check(T)'
        public bool Check(T value)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'IsNotNullOrEmptyRule<T>.Check(T)'
        {
            if (value == null)
            {
                return false;
            }

            var str = $"{value }";
            return !string.IsNullOrWhiteSpace(str);
        }
    }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'IsLenghtValidRule<T>'
    public class IsLenghtValidRule<T> : IValidationRule<T>
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'IsLenghtValidRule<T>'
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'IsLenghtValidRule<T>.ValidationMessage'
        public string ValidationMessage { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'IsLenghtValidRule<T>.ValidationMessage'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'IsLenghtValidRule<T>.MinimunLenght'
        public int MinimunLenght { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'IsLenghtValidRule<T>.MinimunLenght'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'IsLenghtValidRule<T>.MaximunLenght'
        public int MaximunLenght { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'IsLenghtValidRule<T>.MaximunLenght'

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'IsLenghtValidRule<T>.Check(T)'
        public bool Check(T value)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'IsLenghtValidRule<T>.Check(T)'
        {
            if (value == null)
            {
                return false;
            }

            var str = value as string;
            return (str.Length > MinimunLenght && str.Length <= MaximunLenght);
        }
    }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'HasValidAgeRule<T>'
    public class HasValidAgeRule<T> : IValidationRule<T>
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'HasValidAgeRule<T>'
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'HasValidAgeRule<T>.ValidationMessage'
        public string ValidationMessage { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'HasValidAgeRule<T>.ValidationMessage'

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'HasValidAgeRule<T>.Check(T)'
        public bool Check(T value)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'HasValidAgeRule<T>.Check(T)'
        {
            if (value is DateTime bday)
            {
                DateTime today = DateTime.Today;
                int age = today.Year - bday.Year;
                return (age >= 18);
            }

            return false;
        }
    }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'IsValueTrueRule<T>'
    public class IsValueTrueRule<T> : IValidationRule<T>
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'IsValueTrueRule<T>'
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'IsValueTrueRule<T>.ValidationMessage'
        public string ValidationMessage { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'IsValueTrueRule<T>.ValidationMessage'

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'IsValueTrueRule<T>.Check(T)'
        public bool Check(T value)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'IsValueTrueRule<T>.Check(T)'
        {
            return bool.Parse($"{value}");
        }
    }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'IsValidEmailRule<T>'
    public class IsValidEmailRule<T> : IValidationRule<T>
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'IsValidEmailRule<T>'
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'IsValidEmailRule<T>.ValidationMessage'
        public string ValidationMessage { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'IsValidEmailRule<T>.ValidationMessage'

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'IsValidEmailRule<T>.Check(T)'
        public bool Check(T value)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'IsValidEmailRule<T>.Check(T)'
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress($"{value}");
                return addr.Address == $"{value}";
            }
            catch
            {
                return false;
            }
        }
    }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'IsValidPasswordRule<T>'
    public class IsValidPasswordRule<T> : IValidationRule<T>
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'IsValidPasswordRule<T>'
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'IsValidPasswordRule<T>.ValidationMessage'
        public string ValidationMessage { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'IsValidPasswordRule<T>.ValidationMessage'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'IsValidPasswordRule<T>.RegexPassword'
        public Regex RegexPassword { get; set; } = new Regex("(?=.*[A-Z])(?=.*\\d)(?=.*[¡!@#$%*¿?\\-_.\\(\\)])[A-Za-z\\d¡!@#$%*¿?\\-\\(\\)&]{8,20}");
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'IsValidPasswordRule<T>.RegexPassword'

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'IsValidPasswordRule<T>.Check(T)'
        public bool Check(T value)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'IsValidPasswordRule<T>.Check(T)'
        {
            return (RegexPassword.IsMatch($"{value}"));
        }
    }
    /* public class MatchPairValidationRule<T> : IValidationRule<ValidatablePair<T>>
     {
         public string ValidationMessage { get; set; }

         public bool Check(ValidatablePair<T> value)
         {
             return value.Item1.Value.Equals(value.Item2.Value);
         }
     }*/
}
