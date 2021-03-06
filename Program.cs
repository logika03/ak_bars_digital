using System;

namespace ConsoleApp1
{
    class Program
    {

        static void Main()
        {
            var localDirectory = @"D:\test_data";
            var directoryYd = @"new_folder";
            UploadFiles.Upload(localDirectory, directoryYd);
            Console.ReadKey();
        }

        
    }
}