// Ported to C# from https://www.reddit.com/r/PowerShell/comments/8wj4cu/i_wrote_a_powershell_script_that_uses_the_console/
// Credtis to u/wrktrway and u/TheIncorrigible1
using System.Text.RegularExpressions;

namespace PleOps.LibreGlucoseWatcher.TrayIcon;

internal static class BeepMusic
{
    private static readonly Dictionary<string, int> NoteTypes = new()
    {
        { "W", 1600 },
        { "W.", 2000 },
        { "H", 1000},
        { "H.", 1500 },
        { "Q", 400 },
        { "Q.", 600 },
        { "E", 200 },
        { "E.", 300 },
        { "S", 100 },
        { "S.", 150 },
    };

    private static readonly Dictionary<string, double[]> NoteIndex = new()
    {
        { "C", new[] { 16.35, 32.7, 65.41, 130.8, 261.6, 523.3, 1047, 2093, 4186 } },
        { "C#", new[] {17.32, 34.65, 69.3, 138.6, 277.2, 554.4, 1109, 2217, 4435 } },
        { "D", new[] {18.35, 36.71, 73.42, 146.8, 293.7, 587.3, 1175, 2349, 4699 } },
        { "EB", new[] { 19.45, 38.89, 77.78, 155.6, 311.1, 622.3, 1245, 2489, 4978 } },
        { "E", new[] { 20.6, 41.2, 82.41, 164.8, 329.6, 659.3, 1319, 2637, 5274 } },
        { "F", new[] { 21.83, 43.65, 87.31, 174.6, 349.2, 698.5, 1397, 2794, 5588 } },
        { "F#", new[] { 23.12, 46.25, 92.5, 185, 370, 740, 1480, 2960, 5920 } },
        { "G", new[] { 24.5, 49, 98, 196, 392, 784, 1568, 3136, 6272 } },
        { "G#", new[] { 25.96, 51.91, 103.8, 207.7, 415.3, 830.6, 1661, 3322, 6645 } },
        { "A", new[] { 27.5, 55, 110, 220, 440, 880, 1760, 3520, 7040 } },
        { "BB", new[] { 29.14, 58.27, 116.5, 233.1, 466.2, 932.3, 1865, 3729, 7459 } },
        { "B", new[] { 30.87, 61.74, 123.5, 246.9, 493.9, 987.8, 1976, 3951, 7902 } },
        { "R", new[] { 0.0 } },
    };

    public static void PlaySong()
    {
        PlayNote("R0H,G6E,F#6E,E6E,E6E,F#6H,R0H,R0Q,R0E,A5E,G6E,F#6E,E6E,E6E,F#6Q.,D6Q,E6E");
        PlayNote("A5H,R5E,R0Q.,A5E,E6Q,F#6E,G6Q.,E6E,C#6Q,D6Q.,E6Q,A5E,A5Q,F#6Q.,R0H");
        PlayNote("R0H,G6E,F#6E,E6E,E6E,F#6H,R0H,R0Q,R0E,A5E,G6E,F#6E,E6E,E6Q,F#6E,D6Q.,E6E");
        PlayNote("A5H,R5E,R0Q.,E6Q,F#6E,G6Q.,E6E,C#6Q.,D6E,E6Q,A5E,D6E,E6E");
        PlayNote("F6E,E6E,D6E,C6E,R0Q,A5E,Bb5E,C6Q,F6Q,E6E,D6E,D6E,C6E,D6E,C6E,C6Q,C6Q,A5E,Bb5E");
        PlayNote("C6Q,F6Q,G6E,F6E,E6E,D6E,D6E,E6E,F6Q,F6Q,G6E,A6E,Bb6E,Bb6E,A6Q,G6Q,F6E,G6E");
        PlayNote("A6E,A6E,G6Q,F6Q,D6E,C6E,D6E,F6E,F6E,E6Q,E6E,F#6E,F#6Q.");
        PlayNote("A6E,A6E,G6Q,F6Q,D6E,C6E,D6E,F6E,F6E,E6Q,E6E,F#6E,F#6H");
        PlayNote("G6E,A6E,A6Q,R0Q,R0E,G6E,F#6E,F#6Q");
        PlayNote("G6E,A6E,A6Q,R0Q,R0E,G6E,F#6E,F#6Q");
    }

    public static void PlayPokemon()
    {
        PlayNote("A5E, A5E, A5E, A5, A5, G5Q, E5E, C5");
        PlayNote("R0, C5, A5, A5E, G5Q, F5E, G5");
        PlayNote("R0, F5, Bb5, Bb5, Bb5E, A5, G5E, F");
        PlayNote("F5, A5, A5, G5E, F5E, A5H");

        PlayNote("A5E, A5, A5E, A5, A5E, G5, E5E, C5");
        PlayNote("A5E, A5H, G5, F5E, G5H");
        PlayNote("B5, B5E, B5E, B5, B5, A5E, G5, F5");
        PlayNote("F5, A5E, A5, G5H, F5E, A5H");
    }

    public static void PlayJurassicPark()
    {
        PlayNote("d4e, c4e, d4h");
        PlayNote("d4e, c4e, d4h");
        PlayNote("d4e, c4e, d4h");
        PlayNote("e4q, e4h, g4q, g4h");
        PlayNote("f4e, d4e, e4q, c4e, a3h");
        PlayNote("f4e, d4e, e4q");
        PlayNote("a4e, d4e, g4q, f4e, f4q, e4e, e4h");
        PlayNote("d4e, c4e, d4q, a3q, g3h");
        PlayNote("d4e, c4e, d4q, a3q, g3h");
        PlayNote("d4e, c4e, c4e, d4h, a3q, d3q, c4h");
        PlayNote("d4e, c4e, d4q, a3q, g3h");
        PlayNote("d4e, c4e, d4q, a3q, g3h");
        PlayNote("d4e, c4e, c4e, d4h, a3q, d3q, c4w");
    }

    public static void PlayAerithTheme()
    {
        PlayNote("F#3q, A3q, D4w");
        PlayNote("c4q, a3q, e3w");
        PlayNote("f#3q, a3q, d4, c4, e4, d4, b3, c4q, a3w, e3w");
        PlayNote("F#3q, A3q, D4w");
        PlayNote("c4q, a3q, e3w");
        PlayNote("d3q, e3q, d3q, R0q, f3q, e3q, d3q, e3q, d3w");
    }

    public static void PlayFFVIIVictoryFanfare()
    {
        PlayNote("c5e, c5e, c5e, c5q, G4, A4, c5q, A4e, C5w");
    }

    public static void PlayGerudoValley()
    {
        PlayNote("C4e, F4e, G4e, A4q, C4e, F4e, G4e, A4h");
        PlayNote("D4e, F4e, G4e, A4q, D4e, F4e, G4e, A4h");
        PlayNote("b3e, E4e, F4e, G4q, B3e, E4e, F4e, G4h, F4e, G4e, F4e, F4h");

        PlayNote("C4e, F4e, G4e, A4q, C4e, F4e, G4e, A4h");
        PlayNote("D4e, F4e, G4e, A4q, D4e, F4e, G4e, A4h");
        PlayNote("b3e, E4e, F4e, G4q, B3e, E4e, F4e, G4h, a4e, b4e, a4e, G4h");
    }

    public static void PlayRandomSong()
    {
        var random = Random.Shared.Next(0, 6);
        switch (random)
        {
            case 0:
                PlaySong();
                break;
            case 1:
                PlayPokemon();
                break;
            case 2:
                PlayJurassicPark();
                break;
            case 3:
                PlayAerithTheme();
                break;
            case 4:
                PlayFFVIIVictoryFanfare();
                break;
            case 5:
                PlayGerudoValley();
                break;
        }
    }

    public static void PlayNote(string noteListText, double tempo = 1)
    {
        var regex = new Regex(
            "(?<Pitch>[A-G][#|b]?|[R])(?<Octave>[0-8])?(?<NoteType>[w|h|q|e|s]\\.?)?",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        string[] noteList = noteListText.Split(',');
        foreach (string note in noteList)
        {
            var matches = regex.Match(note);
            var pitch = matches.Groups["Pitch"];
            var octave = matches.Groups["Octave"];
            var noteType = matches.Groups["NoteType"];

            double duration = (noteType is null || !noteType.Success)
                ? 400
                : NoteTypes[noteType.Value.ToUpperInvariant()];
            duration *= tempo;

            if (pitch.Value == "R")
            {
                Thread.Sleep((int)duration);
                continue;
            }

            var pitchFrequencies = NoteIndex[pitch.Value.ToUpperInvariant()];
            var frequency = octave?.Value switch
            {
                "0" => pitchFrequencies[0], // Beep() does not support any frequencies lower than 38
                "1" => pitchFrequencies.First(x => x >= 32 && x <= 62), // using <38 for Rests
                "2" => pitchFrequencies.First(x => x >= 65 && x <= 124),
                "3" => pitchFrequencies.First(x => x >= 130 && x <= 247),
                "4" => pitchFrequencies.First(x => x >= 261 && x <= 494),
                "5" => pitchFrequencies.First(x => x >= 523 && x <= 988),
                "6" => pitchFrequencies.First(x => x >= 1047 && x <= 1978),
                "7" => pitchFrequencies.First(x => x >= 2093 && x <= 3952),
                "8" => pitchFrequencies.First(x => x >= 4186 && x <= 7902),
                _ => pitchFrequencies.First(x => x >= 523 && x <= 988),
            };

            Console.Beep((int)frequency, (int)duration);
        }
    }

    public static void PlaySuperMario()
    {
        Console.Beep(659, 125);
        Console.Beep(659, 125);
        Thread.Sleep(125);
        Console.Beep(659, 125);
        Thread.Sleep(167);
        Console.Beep(523, 125);
        Console.Beep(659, 125);
        Thread.Sleep(125);
        Console.Beep(784, 125);
        Thread.Sleep(375);
        Console.Beep(392, 125);
        Thread.Sleep(375);
        Console.Beep(523, 125);
        Thread.Sleep(250);
        Console.Beep(392, 125);
        Thread.Sleep(250);
        Console.Beep(330, 125);
        Thread.Sleep(250);
        Console.Beep(440, 125);
        Thread.Sleep(125);
        Console.Beep(494, 125);
        Thread.Sleep(125);
        Console.Beep(466, 125);
        Thread.Sleep(42);
        Console.Beep(440, 125);
        Thread.Sleep(125);
        Console.Beep(392, 125);
        Thread.Sleep(125);
        Console.Beep(659, 125);
        Thread.Sleep(125);
        Console.Beep(784, 125);
        Thread.Sleep(125);
        Console.Beep(880, 125);
        Thread.Sleep(125);
        Console.Beep(698, 125);
        Console.Beep(784, 125);
        Thread.Sleep(125);
        Console.Beep(659, 125);
        Thread.Sleep(125);
        Console.Beep(523, 125);
        Thread.Sleep(125);
        Console.Beep(587, 125);
        Console.Beep(494, 125);
        Thread.Sleep(125);
        Console.Beep(523, 125);
        Thread.Sleep(250);
        Console.Beep(392, 125);
        Thread.Sleep(250);
        Console.Beep(330, 125);
        Thread.Sleep(250);
        Console.Beep(440, 125);
        Thread.Sleep(125);
        Console.Beep(494, 125);
        Thread.Sleep(125);
        Console.Beep(466, 125);
        Thread.Sleep(42);
        Console.Beep(440, 125);
        Thread.Sleep(125);
        Console.Beep(392, 125);
        Thread.Sleep(125);
        Console.Beep(659, 125);
        Thread.Sleep(125);
        Console.Beep(784, 125);
        Thread.Sleep(125);
        Console.Beep(880, 125);
        Thread.Sleep(125);
        Console.Beep(698, 125);
        Console.Beep(784, 125);
        Thread.Sleep(125);
        Console.Beep(659, 125);
        Thread.Sleep(125);
        Console.Beep(523, 125);
        Thread.Sleep(125);
        Console.Beep(587, 125);
        Console.Beep(494, 125);
        Thread.Sleep(375);
        Console.Beep(784, 125);
        Console.Beep(740, 125);
        Console.Beep(698, 125);
        Thread.Sleep(42);
        Console.Beep(622, 125);
        Thread.Sleep(125);
        Console.Beep(659, 125);
        Thread.Sleep(167);
        Console.Beep(415, 125);
        Console.Beep(440, 125);
        Console.Beep(523, 125);
        Thread.Sleep(125);
        Console.Beep(440, 125);
        Console.Beep(523, 125);
        Console.Beep(587, 125);
        Thread.Sleep(250);
        Console.Beep(784, 125);
        Console.Beep(740, 125);
        Console.Beep(698, 125);
        Thread.Sleep(42);
        Console.Beep(622, 125);
        Thread.Sleep(125);
        Console.Beep(659, 125);
        Thread.Sleep(167);
        Console.Beep(698, 125);
        Thread.Sleep(125);
        Console.Beep(698, 125);
        Console.Beep(698, 125);
        Thread.Sleep(625);
        Console.Beep(784, 125);
        Console.Beep(740, 125);
        Console.Beep(698, 125);
        Thread.Sleep(42);
        Console.Beep(622, 125);
        Thread.Sleep(125);
        Console.Beep(659, 125);
        Thread.Sleep(167);
        Console.Beep(415, 125);
        Console.Beep(440, 125);
        Console.Beep(523, 125);
        Thread.Sleep(125);
        Console.Beep(440, 125);
        Console.Beep(523, 125);
        Console.Beep(587, 125);
        Thread.Sleep(250);
        Console.Beep(622, 125);
        Thread.Sleep(250);
        Console.Beep(587, 125);
        Thread.Sleep(250);
        Console.Beep(523, 125);
        Thread.Sleep(1125);
        Console.Beep(784, 125);
        Console.Beep(740, 125);
        Console.Beep(698, 125);
        Thread.Sleep(42);
        Console.Beep(622, 125);
        Thread.Sleep(125);
        Console.Beep(659, 125);
        Thread.Sleep(167);
        Console.Beep(415, 125);
        Console.Beep(440, 125);
        Console.Beep(523, 125);
        Thread.Sleep(125);
        Console.Beep(440, 125);
        Console.Beep(523, 125);
        Console.Beep(587, 125);
        Thread.Sleep(250);
        Console.Beep(784, 125);
        Console.Beep(740, 125);
        Console.Beep(698, 125);
        Thread.Sleep(42);
        Console.Beep(622, 125);
        Thread.Sleep(125);
        Console.Beep(659, 125);
        Thread.Sleep(167);
        Console.Beep(698, 125);
        Thread.Sleep(125);
        Console.Beep(698, 125);
        Console.Beep(698, 125);
        Thread.Sleep(625);
        Console.Beep(784, 125);
        Console.Beep(740, 125);
        Console.Beep(698, 125);
        Thread.Sleep(42);
        Console.Beep(622, 125);
        Thread.Sleep(125);
        Console.Beep(659, 125);
        Thread.Sleep(167);
        Console.Beep(415, 125);
        Console.Beep(440, 125);
        Console.Beep(523, 125);
        Thread.Sleep(125);
        Console.Beep(440, 125);
        Console.Beep(523, 125);
        Console.Beep(587, 125);
        Thread.Sleep(250);
        Console.Beep(622, 125);
        Thread.Sleep(250);
        Console.Beep(587, 125);
        Thread.Sleep(250);
        Console.Beep(523, 125);
    }
}
