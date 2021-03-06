﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureEditor : MonoBehaviour
{
    public Texture2D baseTex;
    //public Texture2D newTex;
    public GameObject ground;
    bool started = false;

    public void SetupTextures()
    {
        ground = GameObject.Find("Ground");
        baseTex = (Texture2D)ground.GetComponent<Renderer>().material.GetTexture("_SplatTex");

        baseTex = createNewText();
        ground.GetComponent<Renderer>().material.SetTexture("_SplatTex", baseTex);
        started = true;
    }

    //Paints Circle on Texture
    public void PaintCircle(Vector2 pos, float rad)
    {
        if (!started)
        {
            Debug.LogError("AW FUCK");
        }
        Texture2D newTex = (Texture2D)ground.GetComponent<Renderer>().material.GetTexture("_SplatTex");
        if(newTex == null)
        {
            Debug.LogError("newTex is null");
        }
        rad *= 3.3f;
        //IMPLEMENT THIS INTO EVERYTHING
        pos *= newTex.width;
        if(newTex.width < 1)
        {
            Debug.LogError("newTex is empty");
        }

        for (int y = 0; y < newTex.height; y++)
        {
            for (int x = 0; x < newTex.width; x++)
            {
                if (Vector2.Distance(pos, new Vector2(x, y)) < rad)
                {
                    Color color = Color.white;
                    newTex.SetPixel(x, y, color);
                }

            }
        }
        newTex.Apply();
        ground.GetComponent<Renderer>().material.SetTexture("_SplatTex", newTex);
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
        for (int i = 0; i < temp.Length; i++)
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
