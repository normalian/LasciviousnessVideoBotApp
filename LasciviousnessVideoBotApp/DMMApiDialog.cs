using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace LasciviousnessVideoBotApp
{
    [LuisModel("f5db58c8-d646-409d-b8ba-1705720e424a", "fc745cd7da734f359cd2337287301be7")]
    [Serializable]
    public class DMMApiDialog : LuisDialog<object>
    {
        private const string SERVICES_ENTITY = "Services";
        private const string BUILTIN_DATE = "builtin.datetime.date";
        private const string BUILTIN_TIME = "builtin.datetime.time";
        private const string BUILTIN_SET = "builtin.datetime.set";

        [LuisIntent("")]
        public async Task DoNothing(IDialogContext context, LuisResult result)
        {
            string message = $"貴方の好みが分かる情報を入力してください.";
            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }

        [LuisIntent("VideoSearch")]
        public async Task Search(IDialogContext context, LuisResult result)
        {
            var keyPhrase = await DMMApiCaller.GetGenreFromInput(result.Query);

            string message = string.Empty;
            if (keyPhrase.Count > 0)
            {
                message = string.Format("貴方向けに [{0}] で動画を検索します。", string.Join(", ", keyPhrase.ToArray()));
            }else
            {
                message = string.Format("好みが分からなかったので、人気動画を検索します。");
            }
            await context.PostAsync(message);

            var videos = await DMMApiCaller.SeachVideo(string.Join(" ", keyPhrase.ToArray()));
            foreach (var item in videos.result.items)
            {
                await context.PostAsync(string.Format("\t{0} - {1}", item.title, item.URL));
            }

            context.Wait(MessageReceived);
        }
    }
}