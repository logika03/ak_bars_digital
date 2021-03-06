using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using YandexDisk.Client;
using YandexDisk.Client.Http;
using YandexDisk.Client.Protocol;

namespace ConsoleApp1
{
    public class UploadFiles
    {
        private const string OauthToken = "AgAAAABR3T5gAAbyUgrSWP_sEEHFifi_f6Fyv6U";

        public static async void Upload(string localDirectory, string directoryYdPath)
        {
            try
            {
                if (!ExistsPath(localDirectory)) return;
                var files = Directory.GetFiles(localDirectory);
                IDiskApi diskApi = new DiskHttpApi(OauthToken);

                await CreateFolders(directoryYdPath, diskApi);

                foreach (var file in files)
                {
                    _ = Upload(file, directoryYdPath, diskApi);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static async Task CreateFolders(string directoryYdPath,
            IDiskApi diskApi)
        {
            var folders = directoryYdPath.Split("/");
            var rootFolder = await diskApi.MetaInfo.GetInfoAsync(new ResourceRequest()
            {
                Path = "/"
            });
            var curPath = "";
            foreach (var folder in folders)
            {
                curPath += $"/{folder}";
                if (!rootFolder.Embedded.Items.Any(item =>
                    item.Name.Equals(folder) && item.Type == ResourceType.Dir))
                    await diskApi.Commands.CreateDictionaryAsync(curPath);
                rootFolder = await diskApi.MetaInfo.GetInfoAsync(new ResourceRequest()
                {
                    Path = curPath
                });
            }
        }

        private static async Task Upload(string file, string directoryYdPath, IDiskApi diskApi)
        {
            var fileName = Path.GetFileName(file);
            Console.WriteLine($"{fileName}: загрузка");
            var pathYd = $"/{directoryYdPath}/{fileName}";
            var link = await diskApi.Files.GetUploadLinkAsync(pathYd, overwrite: true);
            await using (var fs = File.OpenRead(file))
            {
                await diskApi.Files.UploadAsync(link, fs);
            }

            Console.WriteLine($"{fileName}: загружено");
        }

        private static bool ExistsPath(string localDirectory)
        {
            if (Directory.Exists(localDirectory)) return true;
            Console.WriteLine($"this path {localDirectory} is not exists");
            return false;
        }
    }
}