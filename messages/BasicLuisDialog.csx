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

    public enum Fachbereich { Geschaeftsfuehrung, Buchhaltung, Vertrieb, IT, Marketing, Projektleitung, Projektarbeit, Service }
    public DemoData()
    {
        Add(new Person() { Vorname = "Bettina", Nachname = "Flauser", Bereich = Fachbereich.Geschaeftsfuehrung });
        Add(new Person() { Vorname = "Gregor", Nachname = "Zimmer", Bereich = Fachbereich.Buchhaltung });
        Add(new Person() { Vorname = "Bernd", Nachname = "Kreuler", Bereich = Fachbereich.IT });
        Add(new Person() { Vorname = "Mario", Nachname = "Goslo", Bereich = Fachbereich.IT });
        Add(new Person() { Vorname = "Irina", Nachname = "Weissmann", Bereich = Fachbereich.Marketing });
        Add(new Person() { Vorname = "Michael", Nachname = "Kubek", Bereich = Fachbereich.Marketing });
        Add(new Person() { Vorname = "Franziska", Nachname = "Römer", Bereich = Fachbereich.Projektleitung });
        Add(new Person() { Vorname = "Anja", Nachname = "Molke", Bereich = Fachbereich.Projektarbeit });
        Add(new Person() { Vorname = "Rainer", Nachname = "Egemann", Bereich = Fachbereich.Buchhaltung });
        Add(new Person() { Vorname = "Thomas", Nachname = "Seneke", Bereich = Fachbereich.Vertrieb });
        Add(new Person() { Vorname = "Marco", Nachname = "Spinker", Bereich = Fachbereich.Vertrieb });
        Add(new Person() { Vorname = "Daniel", Nachname = "Gonso", Bereich = Fachbereich.Service });
        Add(new Person() { Vorname = "Markus", Nachname = "Sammler", Bereich = Fachbereich.Geschaeftsfuehrung });
        Add(new Person() { Vorname = "Anja", Nachname = "Kramer", Bereich = Fachbereich.Geschaeftsfuehrung });
    }

    public List<Person> getPersonFromText(string text)
    {
        List<Person> result = new List<Person>();
        text = text.ToLower();
        foreach (Person p in this)
        {
            if (text.Contains(p.Nachname.ToLower()) || text.Contains(p.Vorname.ToLower()))
                result.Add(p);
        }

        return result;
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
            List<Person> p = data.getPersonFromText(ent.Entity);
            if (p.Count == 0)
                await context.PostAsync($"Ich konnte leider keine Person mit dem Namen '{ent.Entity}' finden.");
            else
            {
                string s = "";
                foreach (Person pp in p)
                {
                    if (!string.IsNullOrEmpty(s))
                        s += "\n";
                    s += $"{p.Anzeigename} arbeitet im Bereich {p.Bereich.ToString()}.";
                }
                await context.PostAsync(s);
            }
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