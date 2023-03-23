using Discord.Commands;
using System;
using System.Threading.Tasks;

namespace Calcifer.Modules
{
    public class ConvertersModule : ModuleBase
    {
        [Command("temp")]
        [Alias("converttemp")]
        [Summary("Converts Celcius to Fahrenheit or Kelvin eg, ~temp 21 c f")]
        [Remarks("~temp <Temp> <from> <to>")]
        public async Task ConvertTempAsync(int temp, char fromMetric, char toMetric)
        {
            try
            {
                int result = 0;

                if (fromMetric == 'c' && toMetric == 'f') // Celcius TO Fahrenheit
                    result = (temp * 9) / (5 + 32);

                if (fromMetric == 'c' && toMetric == 'k') // Celcius TO Kelvin
                    result = temp + 273;

                if (fromMetric == 'f' && toMetric == 'c') // Fahrenheit TO Celcius
                    result = (temp - 32) * 5 / 9;

                if (fromMetric == 'f' && toMetric == 'k') // Farenheit TO Kelvin
                    result = result = ((temp - 32) * 5 / 9) + 273;

                if (fromMetric == 'k' && toMetric == 'c') // Kelvin TO Celcius
                    result = temp - 273;

                if (fromMetric == 'k' && toMetric == 'f') // Kelvin TO Fahrenheit
                    result = 273 - ((temp - 32) * (5 / 9));

                await ReplyAsync($"{temp}°{fromMetric.ToString().ToUpper()} is {result}°{toMetric.ToString().ToUpper()}");
            }
            catch (Exception ex)
            {
                await ReplyAsync($"Exception thrown!!!\n{ex.Message}\nEND OF EXCEPTION MESSAGE\n" +
                    $"Message ALister with **~bugreport**");
                throw;
            }
        }
    }
}
