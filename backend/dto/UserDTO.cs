namespace backend.dto
{
    public class UserDTO
    {
        public int Codigo { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public DateTime LasDateEntry { get; set; }
        public bool IsActive { get; set; }
    }
}
