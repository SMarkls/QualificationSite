namespace QualificationSite.Services.Interfaces;

public interface IDataBaseService
{
    public Task<string> GetNameById(long id);
    public Task<long> GetIdByName(string name);
}