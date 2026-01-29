using System;
using System.Reflection;
using HMUI;
using Lib.CrossPatcher;
using UnityEngine;


public class ArtworkManager : ICrossPatch
{
    [CrossPostfix]
    [CrossPatch(typeof(StandardLevelDetailView), "OnEnable", new Type[0])]
    public static void UpdateArtwork(StandardLevelDetailView _instance)
    {
        if (typeof(StandardLevelDetailView).GetPrivateField(_instance, "_levelBar") is not LevelBar levelBar)
        {
            Debug.LogError("Failed to get _levelBar from StandardLevelDetailView");
            return;
        }
        
        if  (typeof(LevelBar).GetPrivateField(levelBar, "_songArtworkImageView") is not ImageView imageView)
        {
            Debug.LogError("Failed to get _songArtworkImageView from StandardLevelDetailView.levelBar");
            return;
        }

        imageView.color = new Color(0.5f, 0.5f, 0.5f, 1f);
        imageView.preserveAspect = false;
        
        if (!typeof(ImageView).SetPrivateField(imageView, "_skew", 0.0f))
        {
            Debug.LogError("Failed to set _songArtworkImageView._skew to 0.0f");
            return;
        }
        
        RectTransform image = imageView.rectTransform;
        image.sizeDelta = new Vector2(70.5f, 58f);
        image.localPosition = new Vector3(-34.4f, -56f, 0f);;
        image.SetAsFirstSibling();
    }
}
