using System;
using System.Reflection;
using HMUI;
using UnityEngine;


public class ArtworkManager
{

    private static object GetPrivateField(Type type, string fieldName, object instance)
    {
        var field = type.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);

        if (field is null)
            return null;

        return field.GetValue(instance);
    }
    
    private static bool SetPrivateField(Type type, string fieldName, object instance, object value)
    {
        var field = type.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);

        if (field is null)
            return false;

        field.SetValue(instance, value);
        return true;
    }
    
    public static void UpdateArtwork(LevelBar levelBar)
    {
        if (GetPrivateField(typeof(LevelBar),"_songArtworkImageView", levelBar) is not ImageView imageView)
        {
            Debug.LogError("Failed to get _songArtworkImageView from levelBar");
            return;
        }
        
        imageView.color = new Color(0.5f, 0.5f, 0.5f, 1f);
        imageView.preserveAspect = false;
        
        if (!SetPrivateField(typeof(ImageView), "_skew", imageView, 0.0f))
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
