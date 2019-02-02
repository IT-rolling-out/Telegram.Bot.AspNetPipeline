using System;
using System.IO;
using File = System.IO.File;

namespace IRO.Tests.Telegram
{
    public static class BotStaticTestsHelpers
    {
        public static string GetToken()
        {
            try
            {
                //Read token from gitignored file.
                var path = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..\\..\\..\\..\\..\\",
                    "test_token.txt"));
                var token = File.ReadAllText(path).Trim();
                if (string.IsNullOrWhiteSpace(token))
                    throw new Exception();
                return token;
            }
            catch(Exception ex)
            {
                throw new Exception("Wrong token. Please, check 'test_token.txt' exists in solution folder.", ex);
            }
        }
    }
}
