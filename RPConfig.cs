using Rocket.API;
using System.Collections.Generic;
using System.Xml.Serialization;
using System;

namespace DudeRPCore
{
    public class RPConfig : IRocketPluginConfiguration
    {
        // Database
        public string DatabaseServer;
        public string DatabaseName;
        public string DatabaseUsername;
        public string DatabasePassword;
        public string DatabasePort;

        public string StaffRole;
        public string ZkStaffRole;
        public string PoliceRole;


        [XmlArrayItem(ElementName = "ExpGroup")]
        public List<ExpGroup> ExpGroups;

        public void LoadDefaults()
        {
            DatabaseServer = "127.0.0.1";
            DatabasePort = "3306";
            DatabaseName = "unturned";
            DatabaseUsername = "root";
            DatabasePassword = "";
            StaffRole = "staff";
            ZkStaffRole = "zkstaff";
            PoliceRole = "policia";

            ExpGroups = new List<ExpGroup>() {
                new ExpGroup() { GroupId = "Zivnostnik" , Exp = 10000},
                new ExpGroup() { GroupId = "Podnikatel", Exp = 100000},
                new ExpGroup() { GroupId = "ZkusenyPodnikatel", Exp = 1000000},
                new ExpGroup() { GroupId = "Investor", Exp = 10000000},
            };
        }
    }

    public class ExpGroup
    {
        public ExpGroup() { }
        public string GroupId;
        public uint Exp;
    }
}
