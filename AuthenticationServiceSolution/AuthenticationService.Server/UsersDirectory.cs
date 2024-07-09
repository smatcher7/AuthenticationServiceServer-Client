namespace AuthenticationService.Server
{
    public class UsersDirectory
    {
        private Dictionary<string, string> _users;

        public UsersDirectory()
        {
            _users = new Dictionary<string, string>();
        }

        public bool IsAvailableUser(string userName)
        {
            return !_users.ContainsKey(userName);
        }

        public KeyValuePair<string, string>? GetUser(string userName)
        {
            return _users.FirstOrDefault(x => x.Key == userName);
        }

        public void AddUser(string userName, string passwordHash)
        {
            _users.Add(userName, passwordHash);
        }

        public List<string> GetUsers()
        {
            return _users.Keys.ToList();
        }
    }
}
