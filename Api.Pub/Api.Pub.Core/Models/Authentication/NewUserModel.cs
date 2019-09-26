namespace Api.Pub.Core.Models.Authentication
{
    public class NewUserModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CPF { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
