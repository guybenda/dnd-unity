
using Unity.Collections;
using UnityEngine;

public class DiceMaterial
{
    public bool isDirty = true;

    public UserDice userDice;
    public RenderTexture renderTexture;
    public Material material;
    public Material textureMaterial;
}