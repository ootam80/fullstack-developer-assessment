namespace MyLocations.Infrastructure.Persistence
{
    public interface ISettingsRepository
    {
        Task<Setting?> GetSetting(string key);

        Task<ICollection<Setting>> GetSettings(string[] keys);
    }

    public class Setting
    {
        public string Key { get; set; }

        public string Value { get; set; }
    }
}
