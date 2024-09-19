namespace PhotoStudio.Application.DTOs
{
        public class UserDTO
        {
            public int Id { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public string PhoneNumber { get; set; }
            public string Address { get; set; }
            public int Credits { get; set; }
            public bool IsVerified { get; set; } 

        }
}
