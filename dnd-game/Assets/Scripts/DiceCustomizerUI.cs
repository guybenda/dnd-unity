using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiceCustomizerUI : MonoBehaviour
{
    public GameObject Die;
    public Slider R1;
    public Slider G1;
    public Slider B1;

    public Slider R2;
    public Slider G2;
    public Slider B2;
    public Slider A2;

    public Slider R3;
    public Slider G3;
    public Slider B3;

    public Slider Smoothness;

    UserDice userDice = UserDice.Default();

    // Start is called before the first frame update
    void Start()
    {
        var mat = DiceMaterialManager.Instance.New(userDice);

        Die.GetComponent<MeshRenderer>().material = mat;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void Awake()
    {

    }

    public void OnClickSet()
    {
        userDice.MainColor = new Color(R1.value, G1.value, B1.value);
        userDice.SecondaryColor = new Color(R2.value, G2.value, B2.value, A2.value);
        userDice.NumbersColor = new Color(R3.value, G3.value, B3.value);
        userDice.Smoothness = Smoothness.value;

        DiceMaterialManager.Instance.Draw(userDice);
    }
}
