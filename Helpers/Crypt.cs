namespace Helpers
{
    public class Crypt
    {
        public string CryptPassword(string password)
        {
            string salt = BCrypt.Net.BCrypt.GenerateSalt();
            string hash = BCrypt.Net.BCrypt.HashPassword(password, salt);
            if (BCrypt.Net.BCrypt.Verify(password, hash))
                return hash;
            return null;
        }

        public bool IsMatch(string password, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
    }
}
