using System;
using GameKit.UI.Core;

namespace GameKit.UI
{
    public class Settings
    {
        public string PrefabsFolder { get; private set; } = "Views";
        public Transition Transition { get; private set; } = Transition.Sequence;

        public Settings SetPrefabsFolder(string path)
        {
            PrefabsFolder = path;
            return this;
        }

        public Settings SetTransition(Transition transition)
        {
            Transition = transition;
            return this;
        }
    }
}