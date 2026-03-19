using UnityEngine;

public static class UIShortCust
{
    public static void Show(this CanvasGroup group)
    {
        if (group == null) return;

        group.alpha = 1.0f;
        group.blocksRaycasts = true;
        group.interactable = true;
    }

    public static void Hide(this CanvasGroup group)
    {
        if (group == null) return;

        group.alpha = 0f;
        group.blocksRaycasts = false;
        group.interactable = false;
    }
}
