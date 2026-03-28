using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UILevelCursor : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI lblLevel;
    [SerializeField] private Image imgBg;
    [SerializeField] private Sprite[] sprites;

    public void Init(int level, int currLevel)
    {
        lblLevel.text = level.ToString();
        imgBg.sprite = sprites[level == currLevel ? 0 : 1];
    }
}
