using System;
using System.Collections.Generic;
using System.Text;
using IRO.Common.Text;

namespace IRO.Tests.Telegram.TwoBotsOneServiceProvider.Services
{
    public class CommonService
    {
        readonly string _randomStr;

        public CommonService()
        {
            _randomStr = TextExtensions.Generate(5);
        }

        public string GetText()
        {
            return $"Hi, i am common services for all bot, i generate string for you: {_randomStr}.";
        }
    }
}
