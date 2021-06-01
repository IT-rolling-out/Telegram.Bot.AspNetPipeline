using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IRO.Common.Text;
using Telegram.Bot.AspNetPipeline.Core;
using Telegram.Bot.AspNetPipeline.Extensions.ImprovedBot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Telegram.Bot.AspNetPipeline.Extensions.KeyboardKit
{
    public class ManagedKeyboard : IManagedKeyboard
    {
        IDictionary<string, ButtonInfo> _buttonsByDataKey;
        Message _messageWithKeyboard;
        ITelegramBotClient _bot;
        readonly UpdateContext _updateContext;
        CancellationTokenSource _listeningTokenSource;
        bool _twoInRow;
        IEnumerable<ButtonInfo> _buttonInfoCollection_OneDimension;
        IEnumerable<IEnumerable<ButtonInfo>> _buttonInfoCollection = new ButtonInfo[0][];

        #region Options.

        public string KeyboardTitleString { get; set; } = "⌨️ㅤ";

        public string CancelButtonString { get; set; } = "🗙ㅤ";

        public string CancelCommandString { get; set; } = "/cancel";

        /// <summary>
        /// Default is true.
        /// </summary>
        public bool IsInlineKeyboard { get; set; } = true;

        /// <summary>
        /// Default is true.
        /// </summary>
        public bool ResizeKeyboard { get; set; } = true;

        /// <summary>
        /// Default updates validator used in <see cref="StartListeningUpdates(ImprovedBot.UpdateValidatorDelegate)"/> or
        /// <see cref="ListenUpdateOnce(ImprovedBot.UpdateValidatorDelegate)"/>. It check if updete type is Message or CallbackQuery and
        /// check <see cref="CancelButtonString"/> or <see cref="CancelCommandString"></see> to cancel keyboard.
        /// </summary>
        public async Task<UpdateValidatorResult> DefaultUpdatesValidator(UpdateContext newCtx,
            UpdateContext originalUpdateContext)
        {
            if (newCtx.Update.Type == UpdateType.Message)
            {
                var text = newCtx.Message?.Text ?? "";
                text = text.Trim();
                if (text.StartsWith(CancelCommandString) || text.StartsWith(CancelButtonString))
                {
                    return UpdateValidatorResult.AbortWaiter;
                }
                if (text.StartsWith("/"))
                {
                    return UpdateValidatorResult.ContinueWaiting;
                }
            }

            if (newCtx.Update.Type == UpdateType.CallbackQuery)
            {
                //Cancel crunch.
                var str = newCtx.Update.CallbackQuery.Data;
                if (str == CancelButtonString)
                {
                    return UpdateValidatorResult.AbortWaiter;
                }
            }

            return UpdateValidatorResult.Valid;
        }
        #endregion

        public ManagedKeyboard(UpdateContext updateContext)
        {
            _bot = updateContext.Bot;
            _updateContext = updateContext;
        }

        /// <summary>
        /// Set buttons one dimension array. When refreshing - array will be converted
        /// to 2d array with two columns per row. Will skip buttons where status Visible==false.
        /// </summary>
        /// <param name="buttonInfoCollection"></param>
        /// <param name="twoInRow">If true - will show two buttons in row.</param>
        public void SetButtons(IEnumerable<ButtonInfo> buttonInfoCollection, bool twoInRow = true)
        {
            _twoInRow = twoInRow;
            _buttonInfoCollection_OneDimension = buttonInfoCollection;
            _buttonInfoCollection = null;
        }

        /// <summary>
        /// Set buttons multiple array manually.
        /// </summary>
        /// <param name="buttonInfoCollection"></param>
        public void SetButtons(IEnumerable<IEnumerable<ButtonInfo>> buttonInfoCollection)
        {
            _buttonInfoCollection = buttonInfoCollection;
            _buttonInfoCollection_OneDimension = null;
        }


        /// <summary>
        /// Use this to allow simple listening or make all manually with
        /// <see cref="GenerateReplyMarkup"/> .
        /// </summary>
        public async Task<Message> ShowButtonsWithText(string text = null)
        {
            text = text ?? KeyboardTitleString;
            var replyMarkup = GenerateReplyMarkup();
            var msg = await _bot.SendTextMessageAsync(
                _updateContext.ChatId,
                text,
                replyMarkup: replyMarkup
            );
            _messageWithKeyboard = msg;
            return msg;
        }

        public async Task<Message> RefreshButtons()
        {
            ThrowIfKeyboardWasntShown();
            var bot = _bot;

            var replyMarkup = GenerateReplyMarkup();
            Message msg;
            if (replyMarkup is InlineKeyboardMarkup inlineMarkup)
            {
                msg = await bot.EditMessageReplyMarkupAsync(
                    _messageWithKeyboard.Chat,
                    _messageWithKeyboard.MessageId,
                    replyMarkup: inlineMarkup
                );
            }
            else
            {
                await DeleteButtons();
                msg = await bot.SendTextMessageAsync(
                    _messageWithKeyboard.Chat,
                    KeyboardTitleString,
                    replyMarkup: replyMarkup
                );
            }
            _messageWithKeyboard = msg;
            return msg;
        }

        public async Task DeleteButtons()
        {
            ThrowIfKeyboardWasntShown();
            var bot = _bot;

            await bot.DeleteMessageAsync(
                _messageWithKeyboard.Chat,
                _messageWithKeyboard.MessageId
            );
        }

        public async Task StartListeningUpdates(UpdateValidatorDelegate updateValidatorDelegate = null)
        {
            ThrowIfKeyboardWasntShown();
            updateValidatorDelegate = updateValidatorDelegate ?? DefaultUpdatesValidator;
            if (_listeningTokenSource != null)
            {
                throw new Exception("Listening was started before.");
            }
            _listeningTokenSource = new CancellationTokenSource();
            while (!_listeningTokenSource.Token.IsCancellationRequested)
            {
                try
                {
                    await ListenUpdateOnce(_listeningTokenSource.Token, updateValidatorDelegate);
                }
                catch (TaskCanceledException ex)
                {
                    if (!_listeningTokenSource.Token.IsCancellationRequested)
                        throw;
                    break;
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="updateValidatorDelegate"> If null - use <see cref="DefaultUpdatesValidator"/>.</param>
        /// <returns></returns>
        public async Task ListenUpdateOnce(CancellationToken cancellationToken, UpdateValidatorDelegate updateValidatorDelegate = null)
        {
            updateValidatorDelegate = updateValidatorDelegate ?? DefaultUpdatesValidator;
            var upd = await _updateContext.BotExt.ReadUpdateAsync(updateValidatorDelegate, cancellationToken);
            await ProcessUpdate(_bot, upd);
        }

        public void StopListeningUpdates()
        {
            _listeningTokenSource?.Cancel();
        }

        IEnumerable<IEnumerable<ButtonInfo>> GetButtons()
        {
            if (_buttonInfoCollection_OneDimension != null)
            {
                //If one dimension row.
                var visible = _buttonInfoCollection_OneDimension.Where(r => r.Visible);
                return visible.ConvertToMultiple(_twoInRow);
            }
            else if (_buttonInfoCollection != null)
            {
                return _buttonInfoCollection;
            }
            else
            {
                throw new System.NullReferenceException(nameof(_buttonInfoCollection));
            }
        }

        async Task ProcessUpdate(ITelegramBotClient bot, Update update)
        {
            var text = update.CallbackQuery?.Data ?? update.Message?.Text ?? "";

            if (!_buttonsByDataKey.TryGetValue(text, out var buttonInfo))
            {
                return;
            }

            var evArgs = new ButtonClickHandlerArgs()
            {
                Bot = bot,
                Update = update,
                ButtonInfo = buttonInfo
            };

            await buttonInfo.ClickHandler.Invoke(this, evArgs);
        }

        /// <summary>
        /// Send message with reply markup and use <see cref="ProcessUpdate(ITelegramBotClient, Update)"/>
        /// to process updates.
        /// </summary>
        /// <returns></returns>
        IReplyMarkup GenerateReplyMarkup()
        {
            if (IsInlineKeyboard)
            {
                var dict = new Dictionary<string, ButtonInfo>();
                var list = new List<IList<InlineKeyboardButton>>();
                var buttonInfoCollection = GetButtons();
                foreach (var smallCol in buttonInfoCollection)
                {
                    var smallList = new List<InlineKeyboardButton>();
                    foreach (var btnInfo in smallCol)
                    {
                        if (!btnInfo.Visible)
                            continue;
                        var key = TextExtensions.Generate(10);
                        dict[key] = btnInfo;
                        dict[btnInfo.Text] = btnInfo;

                        var btn = (btnInfo.Button as InlineKeyboardButton) ?? new InlineKeyboardButton();
                        btn.Text = btnInfo.Text;
                        btn.CallbackData = key;

                        btnInfo.Button = btn;
                        smallList.Add(btn);
                    }

                    if (smallList.Any())
                        list.Add(smallList);
                }

                _buttonsByDataKey = dict;
                return new InlineKeyboardMarkup(list);
            }
            else
            {
                var dict = new Dictionary<string, ButtonInfo>();
                var list = new List<IList<KeyboardButton>>();
                var buttonInfoCollection = GetButtons();
                foreach (var smallCol in buttonInfoCollection)
                {
                    var smallList = new List<KeyboardButton>();
                    foreach (var btnInfo in smallCol)
                    {
                        if (!btnInfo.Visible)
                            continue;
                        dict[btnInfo.Text] = btnInfo;

                        var btn = (btnInfo.Button as KeyboardButton) ?? new KeyboardButton();
                        btn.Text = btnInfo.Text;

                        btnInfo.Button = btn;
                        smallList.Add(btn);
                    }

                    if (smallList.Any())
                        list.Add(smallList);
                }

                _buttonsByDataKey = dict;
                return new ReplyKeyboardMarkup(list, ResizeKeyboard);
            }
        }

        void ThrowIfKeyboardWasntShown()
        {
            if (_messageWithKeyboard == null)
            {
                throw new Exception(
                    $"Can't call this method if not show keyboard via '{nameof(ShowButtonsWithText)}'."
                );
            }
        }

    }
}