using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    TextMeshProUGUI resultText;
    DiceContainer diceContainer;
    Camera cam;

    int diceMask;
    DiceScript highlightedDice;

    int counter = 0;


    void Start()
    {

    }

    void Awake()
    {
        resultText = GameObject.Find("ResultText").GetComponent<TextMeshProUGUI>();
        diceContainer = GameObject.Find("DiceContainer").GetComponent<DiceContainer>();
        cam = Camera.main;

        diceMask = LayerMask.GetMask("Dice");

    }

    void Update()
    {
        var mousePos = Input.mousePosition;
        var ray = cam.ScreenPointToRay(mousePos);

        var prevHighlightedDice = highlightedDice;

        Debug.DrawRay(ray.origin, ray.direction * 20f, Color.red);
        if (Physics.Raycast(ray, out var hit, 20f, diceMask))
        {
            highlightedDice = hit.collider.gameObject.GetComponent<DiceScript>();
            if (highlightedDice != null)
            {
                highlightedDice.IsHovered = true;
            }

        }
        else
        {
            highlightedDice = null;
        }

        if (prevHighlightedDice != null && prevHighlightedDice != highlightedDice)
        {
            prevHighlightedDice.IsHovered = false;
        }
    }

    void FixedUpdate()
    {
        counter = (counter + 1) % 10;
        if (counter != 0) return;

        var (total, breakdown) = diceContainer.Total();

        if (breakdown == "")
        {
            resultText.text = "";
            return;
        }

        resultText.text = $"{breakdown} = {total}";
    }
}
