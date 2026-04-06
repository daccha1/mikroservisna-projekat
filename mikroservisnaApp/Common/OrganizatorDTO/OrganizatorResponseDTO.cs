namespace mikroservisnaApp.Models.DTO.OrganizatorDTO
{
    public class OrganizatorResponseDTO
    {
        public int Id { get; set; }
        public string Ime { get; set; }
        public string Prezime { get; set; }
        public List<string> ListaDogadjaja { get; set; }
    }
}
