using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LocalizeText
{
    [System.Serializable]
    public struct Localize
    {
        public int LocalizeLabel;
        [TextArea(2, 5)]
        public string Text;
    }

    [SerializeField] private List<Localize> _texts;

    public string GetText(int language) => _texts.Find(i => i.LocalizeLabel == language).Text;
}
