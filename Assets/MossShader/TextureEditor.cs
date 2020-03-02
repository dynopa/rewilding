using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureEditor : MonoBehaviour
{
    public Texture2D baseTex;
    public Texture2D newTex;

    void Start()
    {
        newTex = createNewText();
        GetComponent<Renderer>().material.SetTexture("_SplatTex",newTex);
    }

    //Paints Circle on Texture
    public void PaintCircle(Vector2 pos, float rad)
    {
        for (int y = 0; y < newTex.height; y++)
        {
            for (int x = 0; x < newTex.width; x++)
            {
                if (Vector2.Distance(pos, new Vector2(x, y)) < 20)
                {
                    Color color = Color.white;
                    newTex.SetPixel(x, y, color);
                }

                
            }
        }
        newTex.Apply();
    }

    //Reverts Texture Back to Black
    public void FillTexture()
    {
        for (int y = 0; y < newTex.height; y++)
        {
            for (int x = 0; x < newTex.width; x++)
            {
                    Color color = Color.black;
                    newTex.SetPixel(x, y, color);
            }
        }
        newTex.Apply();
    }

    Texture2D createNewText()
    {
        Texture2D newTex = new Texture2D(baseTex.height, baseTex.height);
        Color[] temp = newTex.GetPixels();
        for(int i = 0; i < temp.Length; i++)
        {
            temp[i] = Color.black;
        }

        newTex.SetPixels(temp);
        newTex.Apply();
        return newTex;
    }

    Texture2D copyBaseText()
    {
        Texture2D newTex = new Texture2D(baseTex.height, baseTex.height);
        Color[] temp = baseTex.GetPixels();
        newTex.SetPixels(temp);
        newTex.Apply();

        return newTex;
    }
}
