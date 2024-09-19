using System;

namespace PhotoStudio.Domain.Exceptions
{
    public class UserNotFoundException : NotFoundException
    {
        public UserNotFoundException(int userId)
            : base($"User with id '{userId}' not found.")
        {
        }
    }
}
