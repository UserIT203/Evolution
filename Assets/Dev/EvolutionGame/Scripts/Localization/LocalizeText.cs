using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class LocalizeText
{
    [SerializeField] private List<Localize> _texts;

    public LocalizeText(Localize[] local)
    {
        _texts = local.ToList();
    }

    public string GetText(int localIndex)
    {
        return _texts.Find(
            i => i.LocalizeLabel == localIndex).Text;
    }
}

[System.Serializable]
public struct Localize
{
    public int LocalizeLabel;
    [TextArea(2, 5)]
    public string Text;
}
