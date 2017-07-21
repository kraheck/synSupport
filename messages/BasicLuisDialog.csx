using System;
using System.Threading.Tasks;

using Microsoft.Bot.Builder.Azure;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;

// For more information about this template visit http://aka.ms/azurebots-csharp-luis
[Serializable]
public class BasicLuisDialog : LuisDialog<object>
{
    public BasicLuisDialog() : base(new LuisService(new LuisModelAttribute(Utils.GetAppSetting("LuisAppId"), Utils.GetAppSetting("LuisAPIKey"))))
    {
    }

    [LuisIntent("None")]
    public async Task NoneIntent(IDialogContext context, LuisResult result)
    {
        await context.PostAsync($"Ich habe Sie leider nicht verstanden. Bitte formulieren Sie die Frage neu!"); //
        context.Wait(MessageReceived);
    }

    // Go to https://luis.ai and create a new intent, then train/publish your luis app.
    // Finally replace "MyIntent" with the name of your newly created intent in the following handler
    [LuisIntent("Uhrzeit")]
    public async Task UhrzeitIntent(IDialogContext context, LuisResult result)
    {
        await context.PostAsync($"Die Uhrzeit ist nun " + TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("W. Europe Standard time")).ToString("HH:mm")); 
        context.Wait(MessageReceived);
    }

    [LuisIntent("Dank")]
    public async Task DankIntent(IDialogContext context, LuisResult result)
    {
        await context.PostAsync($"Gern geschehen!"); //
        context.Wait(MessageReceived);
    }
    
}