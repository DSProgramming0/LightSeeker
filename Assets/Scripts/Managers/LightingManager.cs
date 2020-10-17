using UnityEngine;
using System.Globalization;

public class LightingManager : MonoBehaviour
{
    [SerializeField] private Light mainLight;

    [SerializeField] private MeshRenderer skyDomeMesh;
    [SerializeField] private Material mat_ToSet;
    [SerializeField] private Material setMaterial;

    [SerializeField] private Color LitskyDomeTop;
    [SerializeField] private Color LitskyDomeBottom;


    void Start()
    {
        Material m = new Material(mat_ToSet);
        skyDomeMesh.material = Instantiate(m);
        setMaterial = skyDomeMesh.material;
    }


    public void toggleMainLight()
    {
        if (mainLight.enabled == false)
        {
            mainLight.enabled = true;
        }

        setMaterial.SetColor("_TopColor", LitskyDomeTop);
        setMaterial.SetColor("_BottomColor", LitskyDomeBottom);

    }
}
