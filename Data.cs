using System;
using System.Linq;


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
        foreach (Person p in this)
        {
            if (text.Contains(p.Nachname) || text.Contains(p.Vorname))
                return p;
        }

        return null;
    }
}
