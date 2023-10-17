using System;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;

namespace HimawariWallpaper
{
    internal class Program
    {
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern bool SystemParametersInfo(uint uiAction, uint uiParam, string pvParam, uint fWinIni);
        private static readonly HttpClient httpClient = new HttpClient();
        private static readonly string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "latest.png");
        private static readonly string regexPattern = "\\{\"date\":\"([0-9]{4}-[0-9]{2}-[0-9]{2}\\s[0-9]{2}:[0-9]{2}:[0-9]{2})\",\"file\":\".*\"\\}";
        private static readonly Regex dateRegex = new Regex(regexPattern, RegexOptions.IgnoreCase);

        async private static void ChangeWallpaper()
        {
            try
            {
                var latestJson = await httpClient.GetAsync("https://himawari8.nict.go.jp/img/D531106/latest.json").Result.Content.ReadAsStringAsync();
                var latestDate = DateTime.ParseExact(dateRegex.Match(latestJson).Groups[1].Value, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

                var imageUrl = $"https://himawari8.nict.go.jp/img/D531106/1d/550/{latestDate:yyyy/MM/dd/HHmmss}_0_0.png";
                var imageBytes = await httpClient.GetByteArrayAsync(imageUrl);
                File.WriteAllBytes("latest.png", imageBytes);

                SystemParametersInfo(20, 0, imagePath, 1);
            }
            catch (Exception) { }
        }

        static void Main()
        {
            while (true)
            {
                ChangeWallpaper();
                Thread.Sleep(600000);
            }
        }
    }
}
