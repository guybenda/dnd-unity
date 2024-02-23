using UnityEngine;

public class DiceMaterial
{
    const int SIZE = 2048;
    public UserDice userDice;
    public CustomRenderTexture renderTexture;
    public Material material;

    public DiceMaterial()
    {
        renderTexture = new CustomRenderTexture(SIZE, SIZE, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default)
        {
            material = new Material(DiceMaterialManager.Instance.diceTextureMaterial),
            initializationMode = CustomRenderTextureUpdateMode.OnLoad,
            updateMode = CustomRenderTextureUpdateMode.OnDemand,
        };

        material = new Material(DiceMaterialManager.Instance.diceMaterial);

        material.SetTexture("_MainTex", renderTexture);
    }

    public Material Draw()
    {
        renderTexture.material.SetColor("_Color1", userDice.MainColor);
        renderTexture.material.SetColor("_Color2", userDice.SecondaryColor);
        renderTexture.material.SetColor("_Color3", userDice.NumbersColor);

        renderTexture.Update();

        material.SetFloat("_GlossMapScale", userDice.Smoothness);
        material.SetTexture("_MetallicGlossMap", userDice.Metallic ? DiceMaterialManager.Instance.metal : DiceMaterialManager.Instance.nonmetal);

        return material;
    }
}