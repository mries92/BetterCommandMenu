using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BetterCommandMenu
{
    class CounterManager
    {
        // Adds a counter using the players settings
        public static void AddCounter(GameObject gameObject, ItemIndex itemIndex)
        {

        }

        // Adds a counter using specified settings
        public static void AddCounter(GameObject gameObject, ItemIndex itemIndex, CounterInfo info)
        {

        }
    }

    class CounterInfo
    {
        public Color FontColor { get; set; }
        public Color BorderColor { get; set; }
        public string Prefix { get; set; }
    }
}