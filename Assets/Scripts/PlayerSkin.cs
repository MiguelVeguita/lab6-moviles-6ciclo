using UnityEngine;

public class PlayerSkin : MonoBehaviour
{
    
    public GameObject[] heads;
    public GameObject[] torsos;
    public GameObject[] arm;
    public GameObject[] eyes;
    public GameObject[] legs;

    public void UpdateSkin(int headIndex, int eyesIndex, int torsoIndex, int armIndex, int legIndex)
    {
        ChangePart(heads, headIndex);
        ChangePart(eyes, eyesIndex);
        ChangePart(torsos, torsoIndex);
        ChangePart(arm, armIndex);
        ChangePart(legs, legIndex);
    }

    private void ChangePart(GameObject[] partsArray, int indexToShow)
    {
        if (partsArray == null || partsArray.Length == 0)return;
        

        if (indexToShow < 0 || indexToShow >= partsArray.Length)
        {
            Debug.LogError($"Índice de skin '{indexToShow}' fuera de rango para este array de partes.");
            return;
        }
        for (int i = 0; i < partsArray.Length; i++)
        {
            if (partsArray[i] != null)
            {
                partsArray[i].SetActive(i == indexToShow);
            }
        }
    }
}