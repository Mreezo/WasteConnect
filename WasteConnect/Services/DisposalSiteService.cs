using WasteConnect.Models;

namespace WasteConnect.Services
{
    public class DisposalSiteService
    {
        public List<DisposalSite> GetDisposalSites()
        {
            return new List<DisposalSite>
            {
                new DisposalSite
                {
                    Id = 1,
                    Name = "New England Road Landfill Site",
                    Address = "New England Road, Pietermaritzburg",
                    Latitude = -29.6370,
                    Longitude = 30.3920,
                    ImageUrl = "/images/New-England.jpeg",
                    Description = "A disposal facility serving areas around Pietermaritzburg."
                },

           
            };
        }
    }
}