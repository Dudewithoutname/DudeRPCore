using System;
using System.Collections.Generic;
using System.Collections;
using Logger = Rocket.Core.Logging.Logger;
using Steamworks;
using Rocket.Unturned;
using Rocket.Core.Plugins;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using Rocket.Core;
using UnityEngine;
using Rocket.Unturned.Chat;
using Rocket.API.Collections;
using SDG.Unturned;
using System.Linq;

namespace DudeRPCore
{
    public class RPCore : RocketPlugin<RPConfig>
    {
        // I used this plugin for my roleplay server
        // It's only for learning i don't recommend anyone to put this on his server :D 
        // And translations are in Czech and Slovak Lang 
        public static RPCore instance;

        public DatabaseManager db = DatabaseManager.Instance();

        public Dictionary<CSteamID, PreName> prenames;

        public List<CSteamID> EMS;
        public List<CSteamID> Policia;
        public List<CSteamID> Mechanik;
        public List<CSteamID> Pravnik;
        public List<CSteamID> Exekutor;
        public List<CSteamID> Taxi;

        protected override void Load()
        {
            instance = this;

            Logger.Log("XXXX   X   X  XXXX   XXXX    XXX   XXX  ");
            Logger.Log("X   X  X   X  X   X  X       X  X  X  X ");
            Logger.Log("X   X  X   X  X   X  XXX     XXX   XXX  ");
            Logger.Log("X   X  X   X  X   X  X       X  X  X    ");
            Logger.Log("XXXX    XXX   XXXX   XXXX    X  X  X    ");
            //Logger.Log("Oi Mate bri ish");
            Logger.Log($" {Assembly.GetName()} by Dudewithoutname#3129 was loaded!");

            db.Server = Configuration.Instance.DatabaseServer;
            db.DatabaseName = Configuration.Instance.DatabaseName;
            db.UserName = Configuration.Instance.DatabaseUsername;
            db.Password = Configuration.Instance.DatabasePassword;
            db.Port = Configuration.Instance.DatabasePort;

            db.IsConnect();
            db.CheckIfExist();
            prenames = new Dictionary<CSteamID, PreName>();
            EMS = new List<CSteamID>();
            Policia = new List<CSteamID>();
            Mechanik = new List<CSteamID>();
            Pravnik = new List<CSteamID>();
            Exekutor = new List<CSteamID>();
            Taxi = new List<CSteamID>();

            UnturnedPlayerEvents.OnPlayerUpdateExperience += CheckExperience;
            U.Events.OnPlayerConnected += OnPlayerConnect;
            U.Events.OnPlayerDisconnected += OnPlayerDisconnected;
            UnturnedPlayerEvents.OnPlayerChatted += CheckRich;
            EffectManager.onEffectButtonClicked += OnAccepted;
            EffectManager.onEffectTextCommitted += OnTextCommited;
            // UI Effects Names : Meno , Priezvisko , Potvrdit
        }

        protected override void Unload()
        {
            instance = null;
            UnturnedPlayerEvents.OnPlayerUpdateExperience -= CheckExperience;
            UnturnedPlayerEvents.OnPlayerChatted -= CheckRich;
            EffectManager.onEffectButtonClicked -= OnAccepted;
            EffectManager.onEffectTextCommitted -= OnTextCommited;
            U.Events.OnPlayerConnected -= OnPlayerConnect;
            U.Events.OnPlayerDisconnected -= OnPlayerDisconnected;
            db.Close();

            prenames = null;
            EMS = null;
            Policia = null;
            Mechanik = null;
            Pravnik = null;
            Exekutor = null;
            Taxi = null;

            Logger.Log($"{Name} has been unloaded!", ConsoleColor.Yellow);
        }


        private void CheckExperience(UnturnedPlayer player, uint amount)
        {
            if (player.Experience >= Configuration.Instance.ExpGroups[0].Exp)
            {
                for (int i = 0; i < Configuration.Instance.ExpGroups.Count; i++)
                {
                    if (player.Experience >= Configuration.Instance.ExpGroups[i].Exp && !R.Permissions.GetGroup(Configuration.Instance.ExpGroups[i].GroupId).Members.Contains(player.CSteamID.ToString()))
                        R.Permissions.AddPlayerToGroup(Configuration.Instance.ExpGroups[i].GroupId, player);
                }
            }
        }

        private void CheckRich(UnturnedPlayer player, ref Color color, string message, EChatMode chatMode, ref bool cancel)
        {
            if (cancel == false)
                if (message.Contains("<") || message.Contains(">") || message.StartsWith("/") )
                {
                    cancel = true;
                }
                else
                {
                    cancel = true;
                    ChatManager.say($"{R.Permissions.GetGroups(player, false)[0].DisplayName} | {player.Player.name} : {message}", Color.white, true);
                }
        }

        private void OnPlayerConnect(UnturnedPlayer player)
        {
            if (db.Get(player.CSteamID.ToString(),0) == null )
            {
                player.Player.setPluginWidgetFlag(EPluginWidgetFlags.ShowInteractWithEnemy, false);

                player.Player.setPluginWidgetFlag(EPluginWidgetFlags.ForceBlur, true);
                player.Player.setPluginWidgetFlag(EPluginWidgetFlags.Modal, true);

                EffectManager.sendUIEffect(27931, 51, player.CSteamID, true);
                EffectManager.sendUIEffect(27932, 52, player.CSteamID, true);
                EffectManager.sendUIEffect(27933, 53, player.CSteamID, true);
                EffectManager.sendUIEffect(27930, 50, player.CSteamID, true, Translations.Instance.Translate("sk_jazyk"), Translations.Instance.Translate("sk_meno"), Translations.Instance.Translate("sk_priezvisko"));
                
                prenames.Add(player.CSteamID, new PreName() );
            }
            else
            {
                player.Player.setPluginWidgetFlag(EPluginWidgetFlags.ShowInteractWithEnemy, false);

                player.Player.name = db.Get(player.CSteamID.ToString(),1);

            }

            var getBestGroup = R.Permissions.GetGroups(player, false)[0].Id;

            if(getBestGroup == Configuration.Instance.StaffRole || getBestGroup == Configuration.Instance.ZkStaffRole)
                getBestGroup = R.Permissions.GetGroups(player, false)[1].Id;

            switch (getBestGroup)
            {
                case "policajt":
                    Policia.Add(player.CSteamID);
                    break;

                case "zdravotnik":
                    EMS.Add(player.CSteamID);
                    break;

                case "mechanik":
                    Mechanik.Add(player.CSteamID);
                    break;

                case "pravnik":
                    Pravnik.Add(player.CSteamID);
                    break;

                case "taxi":
                    Taxi.Add(player.CSteamID);
                    break;

                case "exekutor":
                    Exekutor.Add(player.CSteamID);
                    break;

                default:
                    break;
            }

            Logger.Log($"Connected : [{player.DisplayName} | {player.CSteamID} | {player.IP}]");

        }
        private void OnPlayerDisconnected(UnturnedPlayer player)
        {
            Logger.Log($"Disconnected : [{player.DisplayName} | {player.CSteamID} | {player.IP}]");

            if (prenames.ContainsKey(player.CSteamID))
                prenames.Remove(player.CSteamID);

            if (EMS.Contains(player.CSteamID))
                EMS.Remove(player.CSteamID);

            if (Policia.Contains(player.CSteamID))
                Policia.Remove(player.CSteamID);

            if (Pravnik.Contains(player.CSteamID))
                Pravnik.Remove(player.CSteamID);

            if (Taxi.Contains(player.CSteamID))
                Taxi.Remove(player.CSteamID);

            if (Exekutor.Contains(player.CSteamID))
                Exekutor.Remove(player.CSteamID);

            if (Mechanik.Contains(player.CSteamID))
                Mechanik.Remove(player.CSteamID);
        }

        private void OnAccepted(Player player, string buttonName)
        {
            if (buttonName == "Potvrdit")
            {
                var untPlayer = UnturnedPlayer.FromPlayer(player);
                string name = prenames[untPlayer.CSteamID].name;
                string surname = prenames[untPlayer.CSteamID].surname;

                string fullname = name + " " + surname;

                if(name.Contains(" ") || surname.Contains(" ") || name == String.Empty || surname == String.Empty || name.All(char.IsDigit) == true || surname.All(char.IsDigit) == true)
                {
                    UnturnedChat.Say(untPlayer, Translations.Instance.Translate("name_invalid"), UnityEngine.Color.red);
                    return;
                }

                if(fullname.Length > 20)
                {
                    UnturnedChat.Say(untPlayer, Translations.Instance.Translate("name_toolong"), UnityEngine.Color.red);
                }
                else
                {
                    untPlayer.Player.setPluginWidgetFlag(EPluginWidgetFlags.ForceBlur, false);
                    untPlayer.Player.setPluginWidgetFlag(EPluginWidgetFlags.Modal, false);
                    untPlayer.GiveItem(2, 1);
                    untPlayer.GiveItem(167, 1);
                    untPlayer.Experience += 2500;
                    EffectManager.askEffectClearByID(27930, untPlayer.CSteamID);
                    EffectManager.askEffectClearByID(27931, untPlayer.CSteamID);
                    EffectManager.askEffectClearByID(27932, untPlayer.CSteamID);
                    EffectManager.askEffectClearByID(27933, untPlayer.CSteamID);

                    db.Add(untPlayer.CSteamID.ToString(), fullname);
                    Logger.Log("Novy obcan menom "+fullname);

                    untPlayer.Player.name = fullname;

                    prenames.Remove(untPlayer.CSteamID);
                }
            }
        }

        private void OnTextCommited(Player player, string buttonName, string text)
        {
            switch(buttonName)
            {
                case "Meno":

                    if(text.ToLower() == "cz" || text.ToLower() == "sk")
                    {
                        ChangeLanguage(player, text.ToLower());
                    }

                    if (prenames.ContainsKey(UnturnedPlayer.FromPlayer(player).CSteamID))
                    {
                        prenames[UnturnedPlayer.FromPlayer(player).CSteamID].name = text;
                    }
                    break;
                case "Priezvisko":
                    if (prenames.ContainsKey(UnturnedPlayer.FromPlayer(player).CSteamID))
                    {
                        prenames[UnturnedPlayer.FromPlayer(player).CSteamID].surname = text;
                    }
                    break;
            }
        }

        public void ChangeLanguage(Player caller, string arg)
        {
            CSteamID SID = UnturnedPlayer.FromPlayer(caller).CSteamID;
            switch (arg.ToLower())
            {
                case "cz":
                    EffectManager.sendUIEffect(27930, 50, SID, true, RPCore.instance.Translations.Instance.Translate("cz_jazyk"), RPCore.instance.Translations.Instance.Translate("cz_meno"), RPCore.instance.Translations.Instance.Translate("cz_priezvisko"));
                    EffectManager.sendUIEffect(27931, 51, SID, true);
                    EffectManager.sendUIEffect(27932, 52, SID, true);
                    EffectManager.sendUIEffect(27933, 53, SID, true);
                    break;

                case "sk":
                    EffectManager.sendUIEffect(27930, 50, SID, true, RPCore.instance.Translations.Instance.Translate("sk_jazyk"), RPCore.instance.Translations.Instance.Translate("sk_meno"), RPCore.instance.Translations.Instance.Translate("sk_priezvisko"));
                    EffectManager.sendUIEffect(27931, 51, SID, true);
                    EffectManager.sendUIEffect(27932, 52, SID, true);
                    EffectManager.sendUIEffect(27933, 53, SID, true);
                    break;
            }
        }

        public void commandTurnOffList(CSteamID steamid)
        {
            StartCoroutine("TurnOffList", steamid);
        }

        private IEnumerator TurnOffList(CSteamID steamid)
        {
            yield return new WaitForSeconds(5.5f);
            EffectManager.askEffectClearByID(27970, steamid);
            EffectManager.askEffectClearByID(27971, steamid);
        }

        public override TranslationList DefaultTranslations
        {
            get
            {
                return new TranslationList(){
                    {"sk_meno","Meno"},
                    {"sk_priezvisko","Priezvisko"},
                    {"sk_jazyk","Pre zmenie jazyku napis CZ alebo SK do prveho riadku"},
                    {"cz_meno","Jmeno"},
                    {"cz_priezvisko","Primeni"},
                    {"cz_jazyk","Pre zmenu jazyku napis CZ nebo SK do prveho radku"},
                    {"name_toolong","Mas moc dlhe cele meno limit je 19 characterov"},
                    {"name_invalid","Tvoje obsahuje nepovoleny znak"},
                    {"pay_tolow","Zadana ciastka je moc mala"},
                    {"pay_notfound","Hrac nebol najdeny"},
                    {"pay_notenough","Nemas dostatok XP"},
                    {"pay_error","Zly syntax napis /pay hrac ciastka"},
                    {"pay_errorInt","Zle zadana ciastka pouzivaj iba cele cisla"},
                    {"pay_payermess","Uspesne si zaplatil hracovi {0} {1} XP"},
                    {"pay_targetmess","Obdrzal si {0} XP od hraca {1}"},
                    {"rob_police","Obcan {0} bol okradnuty nedaleko {1}"},
                    {"rob_target","Niekto ta okrada"},
                    {"rob_attacker","Okradas obcana"},
                    {"rob_fail","Nepodarilo sa najst hraca"},
                    {"storage_open", "Otvoril si storage"},
                    {"storage_error", "Nepodarilo sa otvorit storage"},
                };
            }
        }
    }

    public class PreName 
    {
        public string name { get; set; }
        public string surname { get; set; }
    }
}
