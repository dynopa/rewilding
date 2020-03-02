using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureGetter : MonoBehaviour
{
    public GameObject Canvas;
    public TextureEditor texE;
    public Vector2 textCoord;
    public Texture2D alphaTex;

    void Start()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -Vector3.up, out hit))
        {
            Canvas = hit.transform.gameObject;
            textCoord = hit.textureCoord;
        }
        else
        {
            print("FUCK");
        }
        texE = Canvas.GetComponent<TextureEditor>();
        alphaTex = (Texture2D)Canvas.GetComponent<Renderer>().material.GetTexture("_SplatTex");

        textCoord = textCoord * alphaTex.width;

        texE.FillTexture();
        texE.PaintCircle(textCoord, 50);
    }
    
    public bool IsGroundFertile(Vector2 pos)
    {
        if(alphaTex.GetPixel((int) pos.x, (int) pos.y).Equals(Color.white))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
 
}
