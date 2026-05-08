// See https://aka.ms/new-console-template for more information

var inputString = "Es ist §wirklich§einfach eine §Social-Media $Plattform " +
    "zu §programmieren. §Bald werde ich reich wie Zuckerberg! Dann kaufe " +
    "ich mir die §Donaudampfschifffahrtsgesellschaft.";

////Ziel: alle Wörter separat und nicht in einem String
//var einzelneWörter = inputString.Split(" ");
//PrintWords(einzelneWörter);

////Ziel: nur Wörter nehmen, die mit § beginnen
//var nurParagraphenWörter = einzelneWörter.Where(wort => wort.StartsWith("§"));
//PrintWords(nurParagraphenWörter);

////Ziel: Wörter berücksichtigen, die ein § beinhalten
//var paragraphenSplit = nurParagraphenWörter.SelectMany(wort => wort.Split("§"));
//PrintWords(paragraphenSplit);

////Ziel: "Leer"-Wörter, die durch den vorherigen Schritt enstanden sind, entfernen
//var ohneLeereWörter = paragraphenSplit.Where(wort => wort.Length > 0);
//PrintWords(ohneLeereWörter);

////Ziel: Galaxy-Wörter enden an nicht-Buchstaben, dh. alles bis dahin behalten, den Rest abschneiden
//var wörterOhneSonderzeichen = ohneLeereWörter
//    .Select(wort => wort.Substring(0, FindIndexOfNonLetter(wort)));
//PrintWords(wörterOhneSonderzeichen);

////Ziel: Nur Wörter, die zwischen 5 und 20 Zeichen haben
//var finaleListe = wörterOhneSonderzeichen.Where(wort => wort.Length >= 5 && wort.Length <= 20);
//PrintWords(finaleListe);

var galaxyWords = GetGalaxyWords(inputString);
PrintWords(galaxyWords);

/* Ziel:
 * 
 * wirklich
 * einfach
 * Social
 * programmieren
 * 
 */

string[] GetGalaxyWords(string str)
{
    return str
        .Split(" ") //Aus einem String eine Liste mit Wörtern machen
        .Where(wort => wort.StartsWith("§")) //Nur Wörter die mit § beginnen berücksichtigen
        .SelectMany(wort => wort.Split("§")) //Wörter behandeln, die noch ein § beinhalten
        .Where(wort => wort.Length > 0) //Leere Wörter (die durch vorigen Schritt entstanden sind) entfernen
        .Select(wort => wort.Substring(0, FindIndexOfNonLetter(wort))) //Nicht-Buchstaben entfernen
        .Where(wort => wort.Length >= 5 && wort.Length <= 20) //Länge prüfen
        .ToArray();    
}

int FindIndexOfNonLetter(string s)
{
    for (int i = 0; i < s.Length; i++)
    {
        if (!char.IsLetter(s[i]))
        {
            return i;
        }
    }
    //Falls kein Nicht-Buchstabe enthalten ist, nehmen wir den gesamten String
    return s.Length;
}

void PrintWords(IEnumerable<string> words)
{
    Console.WriteLine("---");
    foreach (var word in words)
    {
        Console.WriteLine(word);
    }
    Console.WriteLine("---");
    Console.WriteLine();
}