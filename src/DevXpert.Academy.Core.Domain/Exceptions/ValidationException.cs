using System;

namespace DevXpert.Academy.Core.Domain.Exceptions
{
    public class ValidationException : Exception
    {
        public string Property { get; private set; }

        public ValidationException(string property, string message)
            : base(message)
        { 
            Property = property;
        }
    }
}
