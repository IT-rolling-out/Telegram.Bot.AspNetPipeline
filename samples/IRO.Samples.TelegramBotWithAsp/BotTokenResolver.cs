using System;
using System.IO;
using File = System.IO.File;

namespace IRO.Samples.TelegramBotWithAsp
{
    public static class BotTokenResolver
    {
        public static string GetToken()
        {
            //Just crunch to read token from test_token.txt (gitignored) file in solution root directory.
            try
            {
                try
                {
                    var path = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..\\..\\..\\..\\..\\",
                        "test_token.txt"));
                    var token = File.ReadAllText(path).Trim();
                    if (string.IsNullOrWhiteSpace(token))
                        throw new Exception();
                    return token;
                }
                catch { }

                {
                    //Read token from gitignored file.
                    var path = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..\\..\\..\\..\\..\\..\\",
                        "test_token.txt"));
                    var token = File.ReadAllText(path).Trim();
                    if (string.IsNullOrWhiteSpace(token))
                        throw new Exception();
                    return token;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Wrong token. Please, check 'test_token.txt' exists in solution folder.", ex);
            }
        }
    }
}
