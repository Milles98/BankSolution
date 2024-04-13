using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace DataLibrary.Attributes
{
    public class MinimumAgeAttribute : ValidationAttribute, IClientModelValidator
    {
        private readonly int _minimumAge;

        public MinimumAgeAttribute(int minimumAge)
        {
            _minimumAge = minimumAge;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is int yearOfBirth)
            {
                if (yearOfBirth > DateTime.Now.Year - _minimumAge)
                {
                    return new ValidationResult(GetErrorMessage());
                }
            }

            return ValidationResult.Success;
        }

        private string GetErrorMessage()
        {
            var earliestYear = DateTime.Now.Year - _minimumAge;
            return $"User must be at least {_minimumAge} years old. {earliestYear} or earlier.";
        }

        public void AddValidation(ClientModelValidationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            MergeAttribute(context.Attributes, "data-val", "true");
            MergeAttribute(context.Attributes, "data-val-minimumage", GetErrorMessage());
            MergeAttribute(context.Attributes, "data-val-minimumage-minimumage", _minimumAge.ToString());
        }

        private bool MergeAttribute(IDictionary<string, string> attributes, string key, string value)
        {
            if (attributes.ContainsKey(key))
            {
                return false;
            }

            attributes.Add(key, value);
            return true;
        }
    }
}
