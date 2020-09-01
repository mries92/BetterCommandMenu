using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace BetterCommandMenu
{
    class PlayerCooldownInfo
    {
        public GameObject masterObject;
        public int cooldownTime;
        public float lastBuffTime;

        public override string ToString()
        {
            return String.Format("Master Object: {0}, Cooldown: {1}, LastBuffTime: {2}", masterObject.name, cooldownTime, lastBuffTime);
        }    
    }
}
