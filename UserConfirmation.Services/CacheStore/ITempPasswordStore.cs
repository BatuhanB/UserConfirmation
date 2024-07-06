namespace UserConfirmation.Services.CacheStore;
public interface ITempPasswordStore
{
    void StorePassword(string userId, string password);
    string RetrievePassword(string userId);
}
