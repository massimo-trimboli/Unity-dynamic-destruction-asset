using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class vTextureScrpt : MonoBehaviour
{
    public bool applyColorToVoxels = true;
    public Material material;

    Texture texture;
    List<GameObject> cubes;
    Dictionary<int, Color> colors = new Dictionary<int, Color>();



    // Start is called before the first frame update
    public void applyColor(List<GameObject> voxelList)
    {
        if (applyColorToVoxels)
        {
            texture = material.mainTexture;
            cubes = voxelList;

            getColors();
        }
    }

    void getColors()
    {
        for(int i = 0; i < cubes.Count; i++)
        {
            Vector3 originPos = cubes[i].transform.position;
            Vector3 point = GetComponent<MeshCollider>().ClosestPoint(cubes[i].transform.position);

            RaycastHit hit;
            Physics.Raycast(
                cubes[i].transform.position,
                point - originPos,
                out hit
                );

            print(hit.textureCoord);
        }
    }
}
