using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapMgr : MonoBehaviour
{
    public Terrain[] terrains;

    public void Relocate(int dir)
    {
        Terrain[] tempTerrain = new Terrain[9];
        System.Array.Copy(terrains, tempTerrain, 9);

        switch (dir)
        {
            case 0:
                tempTerrain[6].transform.position += new Vector3(0, 0, 300);
                tempTerrain[7].transform.position += new Vector3(0, 0, 300);
                tempTerrain[8].transform.position += new Vector3(0, 0, 300);

                for (int i = 0; i < 9; i++)
                {
                    int revise = i + 3;
                    if (revise >= 9)
                    {
                        terrains[revise - 9] = tempTerrain[i];
                    }
                    else
                    {
                        terrains[revise] = tempTerrain[i];

                    }
                }
                break;
            case 1:
                tempTerrain[0].transform.position -= new Vector3(0, 0, 300);
                tempTerrain[1].transform.position -= new Vector3(0, 0, 300);
                tempTerrain[2].transform.position -= new Vector3(0, 0, 300);

                for (int i = 0; i < 9; i++)
                {
                    int revise = i - 3;
                    if (revise < 0)
                    {
                        terrains[revise + 9] = tempTerrain[i];
                    }
                    else
                    {
                        terrains[revise] = tempTerrain[i];
                    }
                }
                break;
            case 2:
                tempTerrain[2].transform.position -= new Vector3(300, 0, 0);
                tempTerrain[5].transform.position -= new Vector3(300, 0, 0);
                tempTerrain[8].transform.position -= new Vector3(300, 0, 0);

                for (int i = 0; i < 9; i++)
                {
                    if (i % 3 == 2)
                    {
                        terrains[i - 2] = tempTerrain[i];
                    }
                    else
                    {
                        terrains[i + 1] = tempTerrain[i];
                    }
                }
                break;
            case 3:
                tempTerrain[0].transform.position += new Vector3(300, 0, 0);
                tempTerrain[3].transform.position += new Vector3(300, 0, 0);
                tempTerrain[6].transform.position += new Vector3(300, 0, 0);

                for (int i = 0; i < 9; i++)
                {
                    if (i % 3 == 0)
                    {
                        terrains[i + 2] = tempTerrain[i];
                    }
                    else
                    {
                        terrains[i - 1] = tempTerrain[i];
                    }
                }
                break;
        }
    }
}
