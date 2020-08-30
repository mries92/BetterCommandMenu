using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace BetterCommandMenu
{
    class BuffChecker : NetworkBehaviour
    {
        private int _timeBetweenBuffs;
        private float _lastBuff;
        void Awake()
        {
            _timeBetweenBuffs = SettingsManager.protectionCooldown.Value;
            _lastBuff = 0;
        }

        public bool CanBuff
        {
            get
            {
                if (Time.time - _lastBuff > _timeBetweenBuffs)
                {
                    _lastBuff = Time.time;
                    return true;
                }
                else return false;
            }
        }

    }
}
