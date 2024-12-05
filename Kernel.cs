using System;
using Sys = Cosmos.System; // Import Cosmos.System for OS features
using System.Text;
using System.Threading;

namespace CosmosKernel
{
    public class Kernel : Sys.Kernel
    {
        private Sys.FileSystem.CosmosVFS fileSystem;
        private string currentDirectory = @"0:\"; // Keep track of the current directory

        protected override void BeforeRun()
        {
            Console.Clear();
            Console.WriteLine(@"
   _ _______              
  (_)_  __(_)______ ____ _
 _   / / / / __/ _ `/ _ `/
(_) /_/ /_/_/  \_,_/\_, / 
                   /___/  ");
            Console.Clear();
            // Initialize the Virtual File System
            fileSystem = new Sys.FileSystem.CosmosVFS();
            Sys.FileSystem.VFS.VFSManager.RegisterVFS(fileSystem);

            Console.Clear();
            Console.WriteLine("Tirag Disk Operating System Alpha5");
            Console.WriteLine("Type 'help' for a list of commands.");
        }

        protected override void Run()
        {
            CheckForNewDrives(); // Continuously check for new drives
            Console.Write(@"---------------------------------------------------------
");
            Console.Write($"{currentDirectory}>");
            string input = Console.ReadLine()?.ToLower();
            string[] commandArgs = input.Split(' ');
            string command = commandArgs[0];

            switch (command)
            {
                case "time":
                    Console.WriteLine(DateTime.Now.ToLongTimeString());
                    break;

                case "ver":
                    Console.WriteLine("Tirag Disk Operating System Alpha5, By HamedRahmati");
                    break;

                case "help":
                    DisplayHelp();
                    break;

                case "cls":
                    Console.Clear();
                    break;

                case "dir":
                    ListFiles();
                    break;

                case "cd":
                    ChangeDirectory(commandArgs);
                    break;

                case "mkdir":
                    CreateDirectory(commandArgs);
                    break;

                case "rmdir":
                    RemoveDirectory(commandArgs);
                    break;

                case "touch":
                    CreateFile(commandArgs);
                    break;

                case "edit":
                    EditFile(commandArgs);
                    break;

                case "read":
                    ReadFile(commandArgs);
                    break;

                case "copy":
                    CopyFile(commandArgs);
                    break;

                case "del":
                    DeleteFile(commandArgs);
                    break;

                case "shutdown":
                    Shutdown();
                    break;

                case "reboot":
                    Reboot();
                    break;

                case "drives":
                    ListDrives();
                    break;
                case "":
                    break;
                default:
                    Console.WriteLine($"Unknown command: {command}");
                    break;
            }
        }

        private void CheckForNewDrives()
        {
            var drives = Sys.FileSystem.VFS.VFSManager.GetVolumes();
            foreach (var drive in drives)
            {
                // Check if the drive is a new USB device
                if (drive.mName.StartsWith("usb"))
                {
                    Console.WriteLine($"New USB drive detected: {drive.mName}");
                }
            }
        }

        private void ListDrives()
        {
            var drives = Sys.FileSystem.VFS.VFSManager.GetVolumes();

            Console.WriteLine("Detected Drives and Partitions:");
            foreach (var drive in drives)
            {
                Console.WriteLine($"Drive Name: {drive.mName}, Size: {drive.mSize / 1024 / 1024} MB");
            }
        }

        private void DisplayHelp()
        {
            Console.WriteLine(@"---------------------------------------------------------

                       AVAILABLE COMMANDS
---------------------------------------------------------
help            : Show this help message.
cls             : Clear the screen.
dir             : List files in the current directory.
cd [path]       : Change the current directory to the specified path.
mkdir [name]    : Create a new directory with the specified name.
rmdir [name]    : Remove the directory with the specified name.
touch [name]    : Create a new file with the specified name.
edit [name]     : Edit the contents of the specified file.
read [name]     : Display the contents of the specified file.
copy [source] [dest] : Copy a file from the source location to the destination.
del [name]      : Delete the specified file.
drives          : List all detected drives and partitions.
shutdown        : Shut down the system.
reboot          : Reboot the system.
time            : Show the date & time.
ver             : Show the version of the operating system.
---------------------------------------------------------
                      END OF COMMAND LIST
---------------------------------------------------------");
        }

        private void ListFiles()
        {
            try
            {
                var directoryListing = Sys.FileSystem.VFS.VFSManager.GetDirectoryListing(currentDirectory);

                Console.WriteLine($"Contents of {currentDirectory}:");
                foreach (var item in directoryListing)
                {
                    Console.WriteLine(item.mName + (item.mEntryType == Sys.FileSystem.Listing.DirectoryEntryTypeEnum.Directory ? " <DIR>" : ""));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private void ChangeDirectory(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: cd [path]");
                return;
            }

            string targetPath = args[1];

            if (targetPath == "..")
            {
                // Navigate to the parent directory
                if (currentDirectory != @"0:\")
                {
                    currentDirectory = currentDirectory.TrimEnd('\\');
                    currentDirectory = currentDirectory.Substring(0, currentDirectory.LastIndexOf('\\') + 1);
                }
            }
            else
            {
                string newPath = currentDirectory + targetPath + @"\";

                // Validate the new path
                try
                {
                    var directoryListing = Sys.FileSystem.VFS.VFSManager.GetDirectoryListing(newPath);
                    currentDirectory = newPath; // Update the current directory if valid
                }
                catch
                {
                    Console.WriteLine($"Directory not found: {targetPath}");
                }
            }
        }

        private void CreateDirectory(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: mkdir [name]");
                return;
            }

            try
            {
                string path = currentDirectory + args[1];
                Sys.FileSystem.VFS.VFSManager.CreateDirectory(path);
                Console.WriteLine($"Directory created: {args[1]}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private void RemoveDirectory(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: rmdir [name]");
                return;
            }

            try
            {
                string path = currentDirectory + args[1];
                Sys.FileSystem.VFS.VFSManager.DeleteDirectory(path, true);
                Console.WriteLine($"Directory removed: {args[1]}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private void CreateFile(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: touch [name]");
                return;
            }

            try
            {
                string path = currentDirectory + args[1];
                Sys.FileSystem.VFS.VFSManager.CreateFile(path);
                Console.WriteLine($"File created: {args[1]}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private void EditFile(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: edit [name]");
                return;
            }

            try
            {
                string path = currentDirectory + args[1];
                Console.WriteLine("Enter file content (end with an empty line):");

                StringBuilder content = new StringBuilder();
                string line;
                while ((line = Console.ReadLine()) != string.Empty)
                {
                    content.AppendLine(line);
                }

                var fileStream = Sys.FileSystem.VFS.VFSManager.GetFile(path).GetFileStream();
                fileStream.SetLength(0); // Clear existing content
                fileStream.Write(Encoding.ASCII.GetBytes(content.ToString()));
                fileStream.Close();

                Console.WriteLine($"File edited: {args[1]}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private void ReadFile(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: read [name]");
                return;
            }

            try
            {
                string path = currentDirectory + args[1];

                // Attempt to get the file, if it fails an exception will be thrown
                var file = Sys.FileSystem.VFS.VFSManager.GetFile(path);

                var fileStream = file.GetFileStream();
                byte[] fileContent = new byte[fileStream.Length];
                fileStream.Read(fileContent, 0, (int)fileStream.Length);
                fileStream.Close();

                Console.WriteLine("File contents:");
                Console.WriteLine(Encoding.ASCII.GetString(fileContent));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private void CopyFile(string[] args)
        {
            if (args.Length < 3)
            {
                Console.WriteLine("Usage: copy [source] [dest]");
                return;
            }

            try
            {
                string sourcePath = currentDirectory + args[1];
                string destPath = currentDirectory + args[2];
                var file = Sys.FileSystem.VFS.VFSManager.GetFile(sourcePath);
                var fileStream = file.GetFileStream();
                var destFile = Sys.FileSystem.VFS.VFSManager.CreateFile(destPath);
                var destStream = destFile.GetFileStream();
                fileStream.CopyTo(destStream);
                fileStream.Close();
                destStream.Close();

                Console.WriteLine($"File copied from {args[1]} to {args[2]}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private void DeleteFile(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: del [name]");
                return;
            }

            try
            {
                string path = currentDirectory + args[1];
                Sys.FileSystem.VFS.VFSManager.DeleteFile(path);
                Console.WriteLine($"File deleted: {args[1]}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private void Shutdown()
        {
            Console.WriteLine("Shutting down...");
            Sys.Power.Shutdown();
        }

        private void Reboot()
        {
            Console.WriteLine("Rebooting...");
            Sys.Power.Reboot();
        }
    }
}
