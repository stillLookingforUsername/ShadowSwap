using TMPro;
using UnityEngine;
public class FinalCookieCollection : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI finalCookieText;

    private void Update()
    {
        finalCookieText.text = "Cookies : " + GameManager.Instance.GetScore().ToString();
    }
}