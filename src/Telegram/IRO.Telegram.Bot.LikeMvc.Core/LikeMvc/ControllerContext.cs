using IRO.Telegram.Bot.ProcessingPipeline.Core;

namespace IRO.Telegram.Bot.ProcessingPipeline.LikeMvc
{
    public class ControllerContext
    {
        public ControllerContext(UpdateContext updateContext, ActionDescriptor actionDescriptor)
        {
            UpdateContext = updateContext;
            ActionDescriptor = actionDescriptor;
        }

        public UpdateContext UpdateContext { get; }

        public ActionDescriptor ActionDescriptor { get; }

        public void StartAnotherAction(string name)
        {

        }
    }
}
