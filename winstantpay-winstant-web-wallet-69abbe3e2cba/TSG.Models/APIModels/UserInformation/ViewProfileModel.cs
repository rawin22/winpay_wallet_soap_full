namespace TSG.Models.APIModels.UserInformation
{
    public class ViewProfileModel : ListOfUserAliases
    {
        public string Username { get; set; }
        public string Customer { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }
}