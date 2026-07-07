namespace WasteConnect.Models
{
    public class DisposalSite
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public string City { get; set; } = "Pietermaritzburg";

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public string ImageUrl { get; set; }

        public string Description { get; set; }
    }
}