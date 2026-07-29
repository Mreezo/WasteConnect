namespace WasteConnect.Services
{
    public interface IWardLookupService
    {
        Task<int?> FindWardNumberAsync(
            double latitude,
            double longitude);
    }
}