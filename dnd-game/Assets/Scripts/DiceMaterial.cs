
using Unity.Collections;
using UnityEngine;

public class DiceMaterial
{
    public UserDice userDice;
    public RenderTexture renderTexture;
    public Material material;

    // public NativeArray<Color> mainTexture;
    // public NativeArray<Color> secondaryTexture;
    // public NativeArray<Color> numbersTexture;
    public NativeArray<Color32> buffer;
    public Texture2D texture;
}