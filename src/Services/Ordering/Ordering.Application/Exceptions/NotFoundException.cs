using System;


namespace Ordering.Application.Exceptions
{
    public class NotFoundException : ApplicationException
    {
        public NotFoundException(string name, object key) 
            : base($"{name} {key} was not found.")
        {

        }
    }
}
