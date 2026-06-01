namespace E_Clubs.Clubs.Repositories;

public interface IClubRepository
{
    Task<Club> CreateClubAsync(Club club);
    Task<IEnumerable<Club>> GetClubsByUserIdAsync(Guid? userId);
    Task<Club?> GetClubByCodeAsync(string code);
    Task<Club?> GetClubByIdAsync(Guid id);
    Task<bool> ClubExistsAsync(Guid clubId);
    Task<bool> CodeExists(string code);
}