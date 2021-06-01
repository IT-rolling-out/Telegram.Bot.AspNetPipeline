using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.AspNetPipeline.Core;
using Telegram.Bot.AspNetPipeline.Extensions.ImprovedBot;
using Telegram.Bot.Types;

namespace Telegram.Bot.AspNetPipeline.Extensions.KeyboardKit
{
    public interface IManagedKeyboard
    {
        string KeyboardTitleString { get; set; }
        string CancelButtonString { get; set; }
        string CancelCommandString { get; set; }

        /// <summary>
        /// Default is true.
        /// </summary>
        bool IsInlineKeyboard { get; set; }

        /// <summary>
        /// Default is true.
        /// </summary>
        bool ResizeKeyboard { get; set; }

        /// <summary>
        /// Default updates validator used in <see cref="ManagedKeyboard.StartListeningUpdates"/> or
        /// <see cref="ManagedKeyboard.ListenUpdateOnce"/>. It check if updete type is Message or CallbackQuery and
        /// check <see cref="ManagedKeyboard.CancelButtonString"/> or <see cref="ManagedKeyboard.CancelCommandString"></see> to cancel keyboard.
        /// </summary>
        Task<UpdateValidatorResult> DefaultUpdatesValidator(UpdateContext newCtx,
            UpdateContext originalUpdateContext);

        /// <summary>
        /// Set buttons one dimension array. When refreshing - array will be converted
        /// to 2d array with two columns per row. Will skip buttons where status Visible==false.
        /// </summary>
        /// <param name="buttonInfoCollection"></param>
        /// <param name="twoInRow">If true - will show two buttons in row.</param>
        void SetButtons(IEnumerable<ButtonInfo> buttonInfoCollection, bool twoInRow = true);

        /// <summary>
        /// Set buttons multiple array manually.
        /// </summary>
        /// <param name="buttonInfoCollection"></param>
        void SetButtons(IEnumerable<IEnumerable<ButtonInfo>> buttonInfoCollection);

        /// <summary>
        /// Use this to allow simple listening or make all manually with
        /// <see cref="ManagedKeyboard.GenerateReplyMarkup"/> .
        /// </summary>
        Task<Message> ShowButtonsWithText(string text = null);

        Task<Message> RefreshButtons();
        Task DeleteButtons();
        Task StartListeningUpdates(UpdateValidatorDelegate updateValidatorDelegate = null);

        /// <summary>
        ///
        /// </summary>
        /// <param name="updateValidatorDelegate"> If null - use <see cref="ManagedKeyboard.DefaultUpdatesValidator"/>.</param>
        /// <returns></returns>
        Task ListenUpdateOnce(CancellationToken cancellationToken, UpdateValidatorDelegate updateValidatorDelegate = null);

        void StopListeningUpdates();
    }
}