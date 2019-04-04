using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CreateCone : ScriptableWizard
{
    public int numVerticles = 10;
    public float radiusTop = 0f;
    public float radiusBottom = 1f;
    public float length = 1f;
    public float openingAngle = 0f;//if >0, create a cone with this angle setting radiusTop to 0, and adjust radiusBottom accoding to Length
    public bool outside = true;
    public bool inside = false;
    public bool addCoolider = false;

    [MenuItem ("GameObject/Create Other/Cone")]
    static void CreateWizard()
    {
        ScriptableWizard.DisplayWizard("Create Cone", typeof(CreateCone));
    }

    private void OnWizardCreate()
    {
        GameObject newCone = new GameObject("Cone");

        if(openingAngle > 0 && openingAngle < 180)
        {
            radiusTop = 0;
            radiusBottom = length * Mathf.Tan(openingAngle * Mathf.Deg2Rad / 2);
        }

        string meshName = newCone.name + numVerticles + "v" + radiusTop + "t" + radiusBottom + "b" + length + "l" + (outside ? "o" : "") + (inside ? "i" : "");
        string meshPrefabPath = "Assets/Editor/" + meshName + ".asset";
        Mesh mesh = (Mesh)AssetDatabase.LoadAssetAtPath(meshPrefabPath, typeof(Mesh));

        if(mesh == null)
        {
            mesh = new Mesh();
            mesh.name = meshName;

            int multiplier = (outside ? 1 : 0) + (inside ? 1 : 0);
            int offset = (outside && inside ? 2 * numVerticles : 0);

            Vector3[] vertices = new Vector3[2 * multiplier * numVerticles]; // 0..n-1: top, n..2n-1: bottom
            Vector3[] normals = new Vector3[2 * multiplier * numVerticles];
            Vector2[] uvs = new Vector2[2 * multiplier * numVerticles];

            int[] tris;
            float slope = Mathf.Atan((radiusBottom - radiusTop) / length); // (rad difference)/height
            float slopeSin = Mathf.Sin(slope);
            float slopeCos = Mathf.Cos(slope);

            int i;

            for(i = 0; i < numVerticles; i++)
            {
                float angle = 2 * Mathf.PI * i / numVerticles;
                float angleSin = Mathf.Sin(angle);
                float angleCos = Mathf.Cos(angle);

                float angleHalf = 2 * Mathf.PI * (i + 0.5f) / numVerticles;
                float angleHalfSin = Mathf.Sin(angleHalf);
                float angleHalfCos = Mathf.Cos(angleHalf);

                vertices[i] = new Vector3(radiusTop * angleCos, radiusTop * angleSin, 0);
                vertices[i + numVerticles] = new Vector3(radiusBottom * angleCos, radiusBottom * angleSin, length);

                if(radiusTop == 0)
                {
                    normals[i] = new Vector3(angleHalfCos * slopeCos, angleHalfSin * slopeCos, -slopeSin);
                }
                else
                {
                    normals[i] = new Vector3(angleCos * slopeCos, angleSin * slopeCos, -slopeSin);
                }

                if (radiusBottom == 0)
                {
                    normals[i + numVerticles] = new Vector3(angleHalfCos * slopeCos, angleHalfSin * slopeCos, -slopeSin);
                }
                else
                {
                    normals[i + numVerticles] = new Vector3(angleCos * slopeCos, angleSin * slopeCos, -slopeSin);
                }

                uvs[i] = new Vector2(1.0f * i / numVerticles, 1);
                uvs[i + numVerticles] = new Vector2(1.0f * i / numVerticles, 0);

                if(outside && inside)
                {
                    //vertices and uvs are identical on inside and outside, so just copy
                    vertices[i + 2 * numVerticles] = vertices[i];
                    vertices[i + 3 * numVerticles] = vertices[i + numVerticles];
                    uvs[i + 2 * numVerticles] = vertices[i];
                    uvs[i + 3 * numVerticles] = vertices[i + numVerticles];
                }
                if(inside)
                {
                    //invert normals
                    normals[i + offset] = -normals[i];
                    normals[i + numVerticles + offset] = -normals[i + numVerticles]; 
                }
            }
            mesh.vertices = vertices;
            mesh.normals = normals;
            mesh.uv = uvs;

            //Create Triangles
            //take care of point order, depending on inside and outside

            int cnt = 0;
            if(radiusTop == 0)
            {
                //top cone
                tris = new int[numVerticles * 3 * multiplier];
                if(outside)
                {
                    for(i = 0; i < numVerticles; i++)
                    {
                        tris[cnt++] = i + numVerticles;
                        tris[cnt++] = i;
                        if(i == numVerticles-1)
                        {
                            tris[cnt++] = numVerticles + offset;
                        }
                        else
                        {
                            tris[cnt++] = i + 1 + numVerticles;
                        }
                    }
                }
                if(inside)
                {
                    for(i = offset; i < numVerticles + offset; i++)
                    {
                        tris[cnt++] = i;
                        tris[cnt++] = i + numVerticles;
                        if(i == numVerticles - 1 + offset)
                        {
                            tris[cnt++] = numVerticles + offset;
                        }
                        else
                        {
                            tris[cnt++] = i + 1 + numVerticles;
                        }
                    }
                }
            }
            else if(radiusBottom == 0)
            {
                //bottom cone
                tris = new int[numVerticles * 3 * multiplier];
                if(outside)
                {
                    for(i = 0; i < numVerticles; i++)
                    {
                        tris[cnt++] = i;
                        if(i == numVerticles - 1)
                        {
                            tris[cnt++] = 0;
                        }
                        else
                        {
                            tris[cnt++] = i + 1;
                        }
                        tris[cnt++] = i + numVerticles;
                    }
                }
                if(inside)
                {
                    for(i = offset; i < numVerticles + offset; i++)
                    {
                        if(i == numVerticles - 1 + offset)
                        {
                            tris[cnt++] = offset;
                        }
                        else
                        {
                            tris[cnt++] = i + 1; 
                        }
                        tris[cnt++] = i;
                        tris[cnt++] = i + numVerticles;
                    }
                }
            }
            else
            {
                tris = new int[numVerticles * 6 * multiplier];
                if(outside)
                {
                    for(i = 0; i < numVerticles; i++)
                    {
                        int ip1 = i + 1;
                        if(ip1 == numVerticles)
                        {
                            ip1 = 0;
                        }

                        tris[cnt++] = i;
                        tris[cnt++] = ip1;
                        tris[cnt++] = i + numVerticles;

                        tris[cnt++] = ip1 + numVerticles;
                        tris[cnt++] = i + numVerticles;
                        tris[cnt++] = ip1;
                    }
                }
                if(inside)
                {
                    for(i = offset; i < numVerticles + offset; i++)
                    {
                        int ip1 = i + 1;
                        if(ip1 == numVerticles + offset)
                        {
                            ip1 = offset;
                        }

                        tris[cnt++] = ip1;
                        tris[cnt++] = i;
                        tris[cnt++] = i + numVerticles;

                        tris[cnt++] = i + numVerticles;
                        tris[cnt++] = ip1 + numVerticles;
                        tris[cnt++] = ip1;
                    }
                }
            }
            mesh.triangles = tris;
            AssetDatabase.CreateAsset(mesh, meshPrefabPath);
            AssetDatabase.SaveAssets();
        }

        MeshFilter mf = newCone.AddComponent<MeshFilter>();
        mf.mesh = mesh;

        newCone.AddComponent<MeshRenderer>();

        if(addCoolider)
        {
            MeshCollider mc = newCone.AddComponent<MeshCollider>();
            mc.sharedMesh = mf.sharedMesh;
        }
        Selection.activeObject = newCone;
    }
}
