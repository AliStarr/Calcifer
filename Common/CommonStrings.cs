﻿using System.Diagnostics;
using System.Reflection;

namespace Booper.Common
{
    public class CommonStrings
    {
        // Bot strings
        public static string BotVersion()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = fvi.FileVersion;

            return version;
        }

        // Major.Minor.{RevisionLetter}-Release


        public static string gitRepo = "";

        //8ball
        public static string[] answers =
        {
            "It is certain", "It is decidedly so", "Without a doubt", "Yes definitely", "You may rely on it", "As I see it, yes", "Most likely", "Outlook good", "Yes", "Signs point to yes",
            "Reply hazy try again", "Ask again later", "Better not tell you now", "Cannot predict now", "Concentrate and ask again",
            "Don't count on it", "My reply is no", "My sources say no", "Outlook not so good",
            "Very doubtful"
        };

        // Reboot
        public static string[] restartStrings =
        {
            "Brb, nerds.", "Restarting...", "I will return shortly.", "Brb.", "Brb, going to the shops.", "Rebooting...", "Back in a second.", "Brb, Smoko.", "Restart string"
        };

        // Quit
        public static string[] quitStrings =
        {
            "Later, nerds", "Bye!", "Seeya!", "Adios", "Good Bye! :wave:", ":sleeping: Good night", "Night night :zzz:", "IM OUT! :v:"
        };
    }
}