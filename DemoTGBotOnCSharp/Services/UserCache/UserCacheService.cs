using DemoTGBotOnCSharp.Handler.Exceptions;
using DemoTGBotOnCSharp.Repositories;
using DemoTGBotOnCSharp.Repositories.Interface;
using System.Collections.Concurrent;

namespace DemoTGBotOnCSharp.Services.UserCache
{
    internal class UserCacheService
    {
        private static readonly Lazy<UserCacheService> _instance =
            new Lazy<UserCacheService>(() => new UserCacheService(UserRepository.Instance));
        public static UserCacheService Instance => _instance.Value;
        private readonly ConcurrentDictionary<long, string> langCache = new ConcurrentDictionary<long,string>();
        private readonly IUserRepository userRepository;

        private UserCacheService(IUserRepository userRepository)
        {
            if (userRepository == null) throw new ComponentInitializeException("User repository is null");
            this.userRepository = userRepository;
        }

        // Получаем язык (сначала ищем в кэше, если нет - берем данные из бд напрямую)
        public async Task<string> GetUserLanguage(long chatId)
        {
            if (langCache.TryGetValue(chatId, out var langCode))
            {
                return langCode;
            }

            // Если нет в кэше, запрашиваем из бд
            string langCodeFromDB = await userRepository.GetUserLangCode(chatId);
            langCache[chatId] = langCodeFromDB;
            
            return langCodeFromDB;
        }

        // Обновляем язык и в кэше и в бд
        public async Task UpdateUserLanguageAsync(long chatId, string langCode)
        {
            langCache[chatId] = langCode;
            await userRepository.SetUserLangCode(chatId, langCode);
        }

        public void SetLanguageCodeInCache(long chatId, string langCode)
        {
            langCache[chatId] = langCode;
        }
    }
}
