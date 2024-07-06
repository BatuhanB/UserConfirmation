using Microsoft.Extensions.Caching.Memory;

namespace UserConfirmation.Services.CacheStore;
public class TempPasswordStore : ITempPasswordStore
{
    private readonly IMemoryCache _cache;
    private readonly MemoryCacheEntryOptions _cacheOptions;

    public TempPasswordStore(IMemoryCache cache)
    {
        _cache = cache;
        _cacheOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(5)); // Set expiration as needed
    }

    public void StorePassword(string userId, string password)
    {
        var encryptedPassword = EncryptionHelper.Encrypt(password);
        _cache.Set(userId, encryptedPassword, _cacheOptions);
        Console.WriteLine($"Cache Set {userId} - {encryptedPassword}");
    }

    public string RetrievePassword(string userId)
    {
        if (_cache.TryGetValue(userId, out string encryptedPassword))
        {
            return EncryptionHelper.Decrypt(encryptedPassword);
        }

        return null;
    }
}
