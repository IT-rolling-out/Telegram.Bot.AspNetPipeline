    public class BotFatherController : BotController
    {
        /// <summary>
        /// Вызывается 1 раз, остальные данные получаем через BotExtensions.ReadMessage().
        /// </summary>
        [BotPath("/newbot", UpdateType.Message)]
        public async Task NewBot()
        {
            this.Bot.SendTextMessageAsync("Enter bot name: ");
            Message msg = await this.BotExtensions.ReadMessage();
            var name = msg.Text;
            this.Bot.SendTextMessageAsync("Enter bot nikname: ");
            msg = await this.BotExtensions.ReadMessage();
            var nick = msg.Text;
            //Creating bot...
            this.Bot.SendTextMessageAsync("Bot created.");
        }
    }

    public class BotFatherController : BotController
    {
        /// <summary>
        /// Метод вызывается трижды, при запуске командой и при получении данных.
        /// </summary>
        [BotPath("/newbot", UpdateType.Message)]
        public async Task NewBot(Message msg)
        {
            var name = this.Session.TryGet<string>("botname");
            if (name == null)
            {
                if (this.Session.TryGet<bool?>("enterBotNameMsgSended") == true)
                {
                    //Второй вызов метода, получаем имя бота.
                    name = msg.Text;
                    this.Session.Set("botname", name);
                }
                else
                {
                    //Первый вызов метода, команда /newbot
                    this.Bot.SendTextMessageAsync("Enter bot name: ");
                    this.Session.Set("enterBotNameMsgSended", true);
                    return;
                }
            }

            var nickname = this.Session.TryGet<string>("nikname");
            if (nickname == null)
            {
                if (this.Session.TryGet<bool?>("enterBotNicknameMsgSended") == true)
                {
                    //Третий вызов метода, получаем ник бота.
                    nickname = msg.Text;
                    this.Session.Set("nikname", name);
                }
                else
                {
                    //Все еще второй вызов метода.
                    this.Bot.SendTextMessageAsync("Enter bot nivkname: ");
                    this.Session.Set("enterBotNicknameMsgSended", true);
                    return;
                }
            }

            //Creating bot...
            this.Bot.SendTextMessageAsync("Bot created.");
        }
    }