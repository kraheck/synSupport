using System;
using System.Threading.Tasks;

using Microsoft.Bot.Builder.Azure;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;



public class Person
{
    public string Vorname;
    public string Nachname;
    public string Anzeigename { get { return $"{Vorname} {Nachname}"; } }

    public DemoData.Fachbereich Bereich;
}

public class DemoData : List<Person>
{

    public enum Fachbereich { Entwicklung, Vertrieb }
    public DemoData()
    {
        Add(new Person() { Vorname = "Oliver", Nachname = "Kraheck", Bereich = Fachbereich.Entwicklung });
        Add(new Person() { Vorname = "Manuel", Nachname = "Wasmuth", Bereich = Fachbereich.Entwicklung });
        Add(new Person() { Vorname = "Fabian", Nachname = "Felten", Bereich = Fachbereich.Vertrieb });
        Add(new Person() { Vorname = "Norbert", Nachname = "Schmidt", Bereich = Fachbereich.Vertrieb });
    }

    public Person getPersonFromText(string text)
    {
        text = text.ToLower();
        foreach (Person p in this)
        {
            if (text.Contains(p.Nachname.ToLower()) || text.Contains(p.Vorname.ToLower()))
                return p;
        }

        return null;
    }
}


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
        await context.PostAsync($"Es ist " + TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("W. Europe Standard time")).ToString("HH:mm") + " Uhr.");
        context.Wait(MessageReceived);
    }

    [LuisIntent("Person_Fachbereich")]
    public async Task Person_FachbereichIntent(IDialogContext context, LuisResult result)
    {
        EntityRecommendation ent;
        if (result.TryFindEntity("person", out ent))
        {
            DemoData data = new DemoData();
            Person p = data.getPersonFromText(ent.Entity);
            if (p == null)
                await context.PostAsync($"Ich konnte leider keine passende Person mit dem Namen '{ent.Entity}' finden.");
            else
                await context.PostAsync($"{p.Anzeigename} arbeitet im Bereich {p.Bereich.ToString()}.");
        }
        else
        {
            await context.PostAsync($"Bitte geben Sie den Namen einer Person an.");
        }
        context.Wait(MessageReceived);
    }

    [LuisIntent("Dank")]
    public async Task DankIntent(IDialogContext context, LuisResult result)
    {
        await context.PostAsync($"Gern geschehen! :-)");
        context.Wait(MessageReceived);
    }

}