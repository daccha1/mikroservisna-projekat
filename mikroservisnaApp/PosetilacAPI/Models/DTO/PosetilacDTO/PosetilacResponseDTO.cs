namespace mikroservisnaApp.Models.DTO.PosetilacDTO
{
    public class PosetilacResponseDTO
    {
        public int Id { get; set; }
        public string Ime { get; set; }
        public string Prezime { get; set; }
        public string StatusZaposlenja { get; set; }
        public string Interesovanje { get; set; }
        public int DogadjajId { get; set; }
        public List<string> ListaDogadjaja { get; set; }
    }
}
