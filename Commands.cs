using System;
using System.Collections.Generic;
using System.Linq;
using Rocket.API;
using Rocket.Unturned.Chat;
using SDG.Unturned;
using Rocket.Unturned.Player;
using UnityEngine;
using SDG.Framework.Utilities;

namespace DudeRPCore.Commands
{
    public class pay : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "pay";

        public string Help => "Allows you to pay via EXP";

        public string Syntax => "/pay (hrac) (suma)";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>() { "dude.pay" };

        public void Execute(IRocketPlayer caller, string[] args)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;

            try
            {

                if (args.Length != 2)
                {
                    UnturnedChat.Say(caller, RPCore.instance.Translations.Instance.Translate("pay_error"), UnityEngine.Color.red);
                    return;
                }

                var target = UnturnedPlayer.FromName(args[0]);
                uint? exp = (uint)Convert.ToInt32(args[1]);

                if (exp == null)
                {
                    UnturnedChat.Say(caller, RPCore.instance.Translations.Instance.Translate("pay_errorInt"), UnityEngine.Color.red);
                    return;
                }

                if (target == null)
                {
                    UnturnedChat.Say(caller, RPCore.instance.Translations.Instance.Translate("pay_notfound"), UnityEngine.Color.red);
                    return;
                }

                if (player.Experience < exp)
                {
                    UnturnedChat.Say(caller, RPCore.instance.Translations.Instance.Translate("pay_notenough"), UnityEngine.Color.red);
                    return;
                }
                else
                {
                    player.Experience -= (uint)exp;
                    target.Experience += (uint)exp;

                    UnturnedChat.Say(caller, RPCore.instance.Translations.Instance.Translate("pay_payermess", target.DisplayName, exp.ToString()), UnityEngine.Color.yellow);
                    UnturnedChat.Say(target, RPCore.instance.Translations.Instance.Translate("pay_targetmess", exp.ToString(), player.DisplayName), UnityEngine.Color.yellow);
                }
            }
            catch
            {
                UnturnedChat.Say(caller, RPCore.instance.Translations.Instance.Translate("pay_error"), UnityEngine.Color.red);
            }
        }

    }
    public class rob : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "rob";

        public string Help => "rob players";

        public string Syntax => string.Empty;

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>() { "dude.rob" };

        public void Execute(IRocketPlayer caller, string[] args)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;
            PlayerLook look = player.Player.look;

            // TY pustalorc :D
            var nearestNode = LevelNodes.nodes.Where(k => k is LocationNode).Cast<LocationNode>().OrderBy(k => Vector3.Distance(k.point, player.Position)).FirstOrDefault();


            if (player.HasPermission("policia") && !player.IsAdmin)
                return;

            if (PhysicsUtility.raycast(new Ray(look.aim.position, look.aim.forward), out RaycastHit hit, Mathf.Infinity, RayMasks.PLAYER))
            {
                UnturnedPlayer target = UnturnedPlayer.FromPlayer(hit.transform.GetComponent<Player>());

                UnturnedChat.Say(target.CSteamID, RPCore.instance.Translations.Instance.Translate("rob_target"), Color.yellow);
                UnturnedChat.Say(player.CSteamID, RPCore.instance.Translations.Instance.Translate("rob_attacker"), Color.yellow);
                foreach (SteamPlayer sPlayer in Provider.clients)
                {
                    UnturnedPlayer _player = UnturnedPlayer.FromSteamPlayer(sPlayer);

                    if (_player.HasPermission("policia"))
                    {
                        UnturnedChat.Say(_player.CSteamID, RPCore.instance.Translations.Instance.Translate("rob_police", target.Player.name, nearestNode.name), Color.yellow);
                    }
                }
            }
            else
            {
                UnturnedChat.Say(player.CSteamID, RPCore.instance.Translations.Instance.Translate("rob_fail"), Color.red);
            }
        }
    }
    public class showNames : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "shownames";

        public string Help => "Ukaze mena";

        public string Syntax => "/shownames";

        public List<string> Aliases => new List<string>() {"showname"};

        public List<string> Permissions => new List<string> { "dude.shownames" };

        public void Execute(IRocketPlayer caller, string[] args)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;

            UnturnedChat.Say(caller, "ShowNames!", UnityEngine.Color.yellow);
            player.Player.setPluginWidgetFlag(EPluginWidgetFlags.ShowInteractWithEnemy, true);
        }
    }
    public class obcania : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "obcania";

        public string Help => "Ukaze prace";

        public string Syntax => "/obcania";

        public List<string> Aliases => new List<string>() { "aktivneprace", "aktivita" };

        public List<string> Permissions => new List<string> { "dude.aktivita" };

        public void Execute(IRocketPlayer caller, string[] args)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;
            EffectManager.sendUIEffect(27970, 7, player.CSteamID, true, RPCore.instance.Policia.Count.ToString(), RPCore.instance.Pravnik.Count.ToString(), RPCore.instance.Mechanik.Count.ToString());
            EffectManager.sendUIEffect(27971, 51, player.CSteamID, true, RPCore.instance.EMS.Count.ToString(), RPCore.instance.Exekutor.Count.ToString(), RPCore.instance.Taxi.Count.ToString());

            RPCore.instance.commandTurnOffList(player.CSteamID);
        }

    }
}
