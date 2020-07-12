using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PEPatch
{
    public class PatchUtils
    {
        public static void SaveMultiplayerData()
        {
            var path = Path.Combine(GameConfig.GetUserDataPath(), "PlanetExplorers/MultiplayerCharacter");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            else
            {
                foreach (var file in Directory.GetFiles(path))
                {
                    File.Delete(file);
                }
            }

            var rolesToSave = GameClientLobby.Self.myRoles.Where(r => r.deletedFlag == 0).Select(r =>
            {
                var rez = new CustomCharactor.CustomData()
                {
                    appearData = new AppearBlendShape.AppearData(),
                    nudeAvatarData = new CustomCharactor.AvatarData(),
                    charactorName = r.name,
                    sex = (CustomCharactor.ESex)(int)r.sex
                };
                rez.appearData.Deserialize(r.appearData);
                rez.nudeAvatarData.Deserialize(r.nudeData);

                return rez;
            }).ToArray();

            for(int i = 0; i < rolesToSave.Length; i++)
            {
                using (FileStream fs = new FileStream(Path.Combine(path, $"Character_{i}.dat"), FileMode.Create, FileAccess.Write))
                {
                    byte[] buff = rolesToSave[i].Serialize();
                    fs.Write(buff, 0, buff.Length);
                }
            }
        }

        public static void LoadMultiplayerData()
        {
            var path = Path.Combine(GameConfig.GetUserDataPath(), "PlanetExplorers/MultiplayerCharacter");
            if (!Directory.Exists(path))
            {
                return;
            }

            GameClientLobby.Self.myRoles = new List<CustomData.RoleInfo>();
            GameClientLobby.Self.myRolesExisted = new List<CustomData.RoleInfo>();

            foreach (var file in Directory.GetFiles(path))
            {
                using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read))
                {
                    if (fs.Length > 0)
                    {
                        byte[] buff = new byte[fs.Length];
                        fs.Read(buff, 0, buff.Length);

                        var customData = new CustomCharactor.CustomData();
                        customData.Deserialize(buff);

                        var steamId = SteamFriendPrcMgr.Instance.GetMyInfo()._SteamID.m_SteamID;

                        var roleId = (steamId.GetHashCode() ^ customData.charactorName.GetHashCode()) % 13000000;

                        var role = new CustomData.RoleInfo()
                        {
                            appearData = customData.appearData.Serialize(),
                            nudeData = customData.nudeAvatarData.Serialize(),
                            name = customData.charactorName,
                            sex = (byte)(int)customData.sex,
                            steamId = steamId,
                            roleID = roleId
                        };

                        GameClientLobby.Self.myRoles.Add(role);
                        GameClientLobby.Self.myRolesExisted.Add(role);

                    }
                }
            }
        }
    }
}
