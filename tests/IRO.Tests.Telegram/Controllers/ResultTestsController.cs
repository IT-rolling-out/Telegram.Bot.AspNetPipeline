using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using IRO.Telegram.Bot.ProcessingPipeline.Core;
using IRO.Telegram.Bot.ProcessingPipeline.LikeMvc;

namespace IRO.Tests.Telegram.Controllers
{
    public class ResultTestsController : BotController
    {
        /// <summary>
        /// При получении следующего сообщения роутинг не происходит,
        /// вместо этого сообщение снова передается в этот метод на обработку.
        /// </summary>
        public ControllerActionResult TestContinue()
        {
            return ControllerActionResult.Continue;
        }

        /// <summary>
        /// Выполнение команды завершено успешно.
        /// (Вариант с Task работает для всех типов результата).
        /// </summary>
        public async Task<ControllerActionResult> TestFinishedAsync()
        {
            return ControllerActionResult.Completed;
        }

        /// <summary>
        /// Аналогично.
        /// </summary>
        public ControllerActionResult TestFinished()
        {
            return ControllerActionResult.Completed;
        }

        /// <summary>
        /// Аналогично.
        /// </summary>
        public void TestFinishedByVoid()
        {
        }

        /// <summary>
        /// Завершает даннуб команду и запускает новую по имени, с текущим контекстом.
        /// Команда под вопросом, т.к. слишком костыль.
        /// </summary>
        public void TestStartAnotherCommand()
        {
            ControllerContext.StartAnotherAction(name: "NameFromRouteAttribute");
        }

        /// <summary>
        /// Команда завершена с ошибкой. Сообщение об ошибке будет передано в обработчик.
        /// </summary>
        public ControllerActionResult TestError()
        {
            return ControllerActionResult.Error;
        }

        /// <summary>
        /// Аналогичною
        /// </summary>
        public ControllerActionResult TestException()
        {
            throw new Exception();
        }
    }
}
