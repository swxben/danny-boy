using System;

namespace dannyboy
{
    public class InvalidEntityTypeException
        : Exception
    {
        public InvalidEntityTypeException(string message, Type entityType)
            : base(message)
        {
            EntityType = entityType;
        }

        public Type EntityType { get; private set; }
    }
}