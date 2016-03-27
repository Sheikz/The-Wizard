using UnityEngine;
using UnityEditor;
using System.Collections;

public class AnimationCreator : EditorWindow
{
    private Sprite spriteSheet;
    private string spriteName;

    [MenuItem("Window/Animation Creator")]
    public static void ShowWindow()
    {
        GetWindow(typeof(AnimationCreator), false, "Animation Creator");
    }

    void OnGUI()
    {
        GUILayout.Label("Spritesheet of character", EditorStyles.boldLabel);
        GUILayout.Label("Drag and drop the spritesheet of the character you wish to create animations for", EditorStyles.wordWrappedMiniLabel);
        spriteSheet = (Sprite)EditorGUILayout.ObjectField(spriteSheet, typeof(Sprite), false);

        if (GUILayout.Button("Create animations"))
        {
            createAnimation(spriteSheet);
        }
    }

    void createAnimation(Sprite spriteSheet)
    {
        Debug.Log("sprite name: " + spriteSheet);
        AnimationClip clip = new AnimationClip();
        AnimationEvent ev = new AnimationEvent();

    }
}
