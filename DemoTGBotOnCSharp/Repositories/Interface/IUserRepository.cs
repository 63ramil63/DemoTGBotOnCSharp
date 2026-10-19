using DemoTGBotOnCSharp.Repositories.DTO;

namespace DemoTGBotOnCSharp.Repositories.Interface
{
    internal interface IUserRepository
    {
        Task<UserDTO?> GetUserById(long id);
        Task<string?> GetUserWordById(long id);
        Task AddNewUser(long id, string f_name, string l_name, string username);
        Task SetUserWord(long id, string word);
        Task SetUserLangCode(long chatId, string lang_code);
        Task<string> GetUserLangCode(long chatId);
    }
}
