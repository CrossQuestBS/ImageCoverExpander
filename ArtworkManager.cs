using System;
using Lib.CrossPatcher;
using UnityEngine;


public class ArtworkManager : ICrossPatch
{
    private static Vector2 ExpandedSizeDelta => new (70.5f, 58f);
    private static Vector3 ExpandedPosition => new (-34.4f, -56f, 0f);

    [CrossPostfix]
    [CrossPatch(typeof(StandardLevelDetailView), "OnEnable", new Type[0])]
    public static void UpdateArtwork(StandardLevelDetailView _instance)
    {
        var levelBar = _instance._levelBar;
        var imageView = levelBar._songArtworkImageView;
        imageView.color = new Color(0.5f, 0.5f, 0.5f, 1f);
        imageView.preserveAspect = false;
        imageView._skew = 0.0f;

        RectTransform image = imageView.rectTransform;
        image.sizeDelta = ExpandedSizeDelta;
        image.localPosition = ExpandedPosition;
        image.SetAsFirstSibling();
    }
}
