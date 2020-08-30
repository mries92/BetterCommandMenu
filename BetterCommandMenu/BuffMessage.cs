using R2API.Networking;
using R2API.Networking.Interfaces;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Networking;

namespace BetterCommandMenu
{
    class BuffMessage : INetMessage
    {
        public void Serialize(NetworkWriter writer)
        {
            writer.Write(_body.gameObject);
            writer.WriteBuffIndex(_buffIndex);
            writer.Write(_buffTime);
            writer.Write(_shieldAmount);
            writer.Write(_enableShield);
        }

        public void Deserialize(NetworkReader reader)
        {
            _body = reader.ReadGameObject().GetComponent<CharacterBody>();
            _buffIndex = reader.ReadBuffIndex();
            _buffTime = reader.ReadInt32();
            _shieldAmount = reader.ReadSingle();
            _enableShield = reader.ReadBoolean();
        }

        public void OnReceived()
        {
            if(_buffIndex != BuffIndex.None)
                _body.AddTimedBuff(_buffIndex, _buffTime);
            if(_enableShield)
                _body.healthComponent.AddBarrier((SettingsManager.protectionShieldAmount.Value / 100.0f) * _body.maxBarrier);
        }

        public CharacterBody _body;
        public BuffIndex _buffIndex;
        public int _buffTime;
        public float _shieldAmount;
        public bool _enableShield;
    }
}
