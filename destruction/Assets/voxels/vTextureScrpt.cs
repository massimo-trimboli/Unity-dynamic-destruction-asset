using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class vTextureScrpt : MonoBehaviour
{
    public bool applyColorToVoxels = true;
    public Material material;

    Texture2D texture;
    List<GameObject> cubes;
    Dictionary<int, Color> colors = new Dictionary<int, Color>();



    public void applyColor(List<GameObject> voxelList)
    {
        if (applyColorToVoxels)
        {
            GetComponent<MeshRenderer>().material = material;
            texture = (Texture2D)material.mainTexture;
            if (texture == null)
            {
                Debug.LogError("Texture is null or not Texture2D");
                return;
            }
            if (!texture.isReadable)
                Debug.LogError("make sure your texture is readable");

            /////////////////
            cubes = voxelList;
            getColors();
            setColors();
        }
    }

    void getColors()
    {
        for(int i = 0; i < cubes.Count; i++)
        {
            Vector3 originPos = cubes[i].transform.position;
            RaycastHit hit;
            Physics.Raycast(
                cubes[i].transform.position,
                transform.position - originPos,
                out hit
                );

            Color color = texture.GetPixelBilinear(hit.textureCoord.x, hit.textureCoord.y);
            colors[i] = color;
        }
    }

    void setColors()
    {
        for (int i = 0; i < cubes.Count; i++)
        {
            Material mat = new Material(material.shader);
            mat.color = colors[i];

            cubes[i].GetComponent<MeshRenderer>().material = mat;
        }
    }
}
