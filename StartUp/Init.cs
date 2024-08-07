using System;
using System.Collections.Generic;
using System.Text;
using bedrock.NetHub.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace bedrock.NetHub.StartUp
{
    public class Init
    {
        public static void Initialize()
        {
            string permissionsJsonPath = Path.Join(Program.programRoot,"config","default", "permissions.json");
            if (!File.Exists(permissionsJsonPath))
            {
                Program.stdhubLOGGER.Info("`§apermissions.json§a`Not Found.");
                return;
            }
            PermissionsJsonSchema permissionsJson = FileIO.ReadAsJSON<PermissionsJsonSchema>(permissionsJsonPath);
            if (!permissionsJson.allowed_modules.Contains("@minecraft/server-net"))
            {
                permissionsJson.allowed_modules.Add("@minecraft/server-net");
                File.Copy(permissionsJsonPath, permissionsJsonPath + ".bak", true);
                FileIO.WriteAsJSON<PermissionsJsonSchema>(permissionsJsonPath, permissionsJson);
                Program.stdhubLOGGER.Info(string.Format("§aSuccessfully patched `§e{0}§a`.", permissionsJsonPath));
            }

            string levelDatPath = Path.Join(Program.GetLevelRoot(), "level.dat");
            string levelDatBackupPath = Path.Join(Program.GetLevelRoot(), "level.dat.bak");
            if (!File.Exists(levelDatPath))
            {
                Program.stdhubLOGGER.Info("`§alevel.dat§a`Not Found.");
                return;
            }
            byte[] patchedExpBytes = [
                0x65, 0x78, 0x70, 0x65, 0x72, 0x69, 0x6D, 0x65, 0x6E, 0x74, 0x73, 0x01, 0x15, 0x00,
                0x65, 0x78, 0x70, 0x65, 0x72, 0x69, 0x6D, 0x65, 0x6E, 0x74, 0x73, 0x5F, 0x65, 0x76, 0x65, 0x72,
                0x5F, 0x75, 0x73, 0x65, 0x64, 0x01, 0x01, 0x08, 0x00, 0x67, 0x61, 0x6D, 0x65, 0x74, 0x65, 0x73,
                0x74, 0x01, 0x01, 0x1E, 0x00, 0x73, 0x61, 0x76, 0x65, 0x64, 0x5F, 0x77, 0x69, 0x74, 0x68, 0x5F,
                0x74, 0x6F, 0x67, 0x67, 0x6C, 0x65, 0x64, 0x5F, 0x65, 0x78, 0x70, 0x65, 0x72, 0x69, 0x6D, 0x65,
                0x06E, 0x74, 0x73, 0x01, 0x00, 0x01, 0x0A, 0x00
            ];
            string originalLevelDatString = FileIO.ReadFile(levelDatPath);
            byte[] originalLevelDatBytes = FileIO.ReadAsBinary(levelDatPath);

            int startOffset = originalLevelDatString.IndexOf("experiments") + 1;
            int endOffset = originalLevelDatString.IndexOf("falldamage") + 1;
            byte[] targetBytes = originalLevelDatBytes.Skip(startOffset - 1).Take(endOffset - startOffset).ToArray();
            if (!patchedExpBytes.SequenceEqual(targetBytes))
            {
                byte[] frontBytes = originalLevelDatBytes[8..startOffset];
                byte[] afterBytes = originalLevelDatBytes[endOffset..];
                byte[] patchedBodyBytes = [.. frontBytes, .. patchedExpBytes, .. afterBytes];
                byte[] patchedLengthBytes = BitConverter.GetBytes(patchedBodyBytes.Length);
                if (!BitConverter.IsLittleEndian)
                {
                    _ = patchedLengthBytes.Reverse();
                }
                byte[] patchedLevelDatBytes = [0x08, 0x00, 0x00, 0x00, .. patchedLengthBytes,.. patchedBodyBytes];
                File.Copy(levelDatPath, levelDatBackupPath,true);
                FileIO.WriteAsBytes(levelDatPath, patchedLevelDatBytes);
                Program.stdhubLOGGER.Info("§aSuccessfully patched `§elevel.dat§a`.");
            }
        }
    }
}
