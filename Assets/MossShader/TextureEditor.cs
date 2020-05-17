using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureEditor : MonoBehaviour
{
    public Texture2D baseTex;
    //public Texture2D newTex;
    GameObject ground;

    void Awake()
    {
        ground = GameObject.Find("Ground");
        baseTex = (Texture2D) ground.GetComponent<Renderer>().material.GetTexture("_SplatTex");

        baseTex = createNewText();
        ground.GetComponent<Renderer>().material.SetTexture("_SplatTex",baseTex);
    }

    //Paints Circle on Texture
    public void PaintCircle(Vector2 pos, float rad)
    {
        rad *= 3.3f;
        //IMPLEMENT THIS INTO EVERYTHING
        pos *= baseTex.width;
        for (int y = 0; y < baseTex.height; y++)
        {
            for (int x = 0; x < baseTex.width; x++)
            {
                if (Vector2.Distance(pos, new Vector2(x, y)) < rad)
                {
                    Color color = Color.white;
                    baseTex.SetPixel(x, y, color);
                }
                
            }
        }
        baseTex.Apply();
    }

    //Reverts Texture Back to Black
    public void FillTexture()
    {
        for (int y = 0; y < baseTex.height; y++)
        {
            for (int x = 0; x < baseTex.width; x++)
            {
                    Color color = Color.black;
                    baseTex.SetPixel(x, y, color);
            }
        }
        baseTex.Apply();
    }

    Texture2D createNewText()
    {
        Color[] temp = baseTex.GetPixels();
        for(int i = 0; i < temp.Length; i++)
        {
            temp[i] = Color.black;
        }

        baseTex.SetPixels(temp);
        baseTex.Apply();
        return baseTex;
    }

    public bool IsGroundFertile(Vector2 pos)
    {
        if (baseTex.GetPixel((int)pos.x, (int)pos.y).Equals(Color.white))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
