using System;
using System.Collections.Generic;
using Rocket.API;
using SDG.Unturned;
using Rocket.Unturned.Player;
using UnityEngine;

namespace DudeRPCore
{
    public class ChatDo : IRocketCommand
    {
        #region Properties
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "do";

        public string Help => "do";

        public string Syntax => "/do";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string> { "dude.do" };
        #endregion

        public void Execute(IRocketPlayer caller, string[] args)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;
            
            foreach (SteamPlayer SteamP in Provider.clients)
            {
                var LoopPlayer = UnturnedPlayer.FromSteamPlayer(SteamP);
                float distance = (LoopPlayer.Position - player.Position).sqrMagnitude;
                if (distance <= 450)
                    ChatManager.say(LoopPlayer.CSteamID,$"<color=#E1C038><b>Do > {player.Player.name} |</b></color><color=#ECE2BC> {string.Join(" ", args)} </color>", Color.white, true);
            }
           
        }
    }

    public class ChatMe : IRocketCommand
    {
        #region Properties
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "me";

        public string Help => "me";

        public string Syntax => "/me";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string> { "dude.me" };
        #endregion

        public void Execute(IRocketPlayer caller, string[] args)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;

            foreach (SteamPlayer SteamP in Provider.clients)
            {
                var LoopPlayer = UnturnedPlayer.FromSteamPlayer(SteamP);
                float distance = (LoopPlayer.Position - player.Position).sqrMagnitude;
                if (distance <= 450)
                    ChatManager.say(LoopPlayer.CSteamID, $"<color=#69dba0><b>Me > {player.Player.name} |</b></color><color=#bce8d1> {string.Join(" ", args)} </color>", Color.white, true);
            }

        }
    }

    public class ChatBM : IRocketCommand
    {
        #region Properties
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "blackmarket";

        public string Help => "bm";

        public string Syntax => "/bm";

        public List<string> Aliases => new List<string>() { "bm" };

        public List<string> Permissions => new List<string> { "dude.bm" };
        #endregion

        public void Execute(IRocketPlayer caller, string[] args)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;

            if (args.Length < 1 || args[0].Length < 1 && player.HasPermission("policia") && !player.IsAdmin)
                return;

            foreach (SteamPlayer SteamP in Provider.clients)
            {
                var LoopPlayer = UnturnedPlayer.FromSteamPlayer(SteamP);
                if(!player.HasPermission(RPCore.instance.Configuration.Instance.PoliceRole))
                    ChatManager.say(LoopPlayer.CSteamID, $"<color=#242424><b>Blackmarket > </b></color><color=#adadad> {string.Join(" ", args)} </color>", Color.white, true);
            }
        }
    }

    public class ChatTweet : IRocketCommand
    {
        #region Properties
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "tweet";

        public string Help => "tweet";

        public string Syntax => "/tweet";

        public List<string> Aliases => new List<string>() { "twt" };

        public List<string> Permissions => new List<string> { "dude.tweet" };
        #endregion

        public void Execute(IRocketPlayer caller, string[] args)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;

            ChatManager.say( $"<color=#1DA1F2><b>Twitter</b></color><color=#AAB8C2> | {player.Player.name} :</color><color=#E1E8ED> { string.Join(" ", args)} </color>", Color.white, true);

        }
    }
}