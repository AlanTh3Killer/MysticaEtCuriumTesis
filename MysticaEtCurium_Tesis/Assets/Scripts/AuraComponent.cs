using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class AuraComponent : MonoBehaviour
{
    public enum AuraType { Vendible, Contenible, Destruir, None }
    public AuraType auraType = AuraType.None;
    public Color auraColor = Color.white;

    private Renderer rend;
    private MaterialPropertyBlock mpb;
    private static readonly int auraColorID = Shader.PropertyToID("_AuraColor");
    private static readonly int auraOnID = Shader.PropertyToID("_AuraEnabled");

    private void Awake()
    {
        rend = GetComponent<Renderer>();
        mpb = new MaterialPropertyBlock();
    }

    public void SetAuraActive(bool active)
    {
        if (rend == null) return;

        rend.GetPropertyBlock(mpb);

        if (active)
        {
            mpb.SetFloat(auraOnID, 1f);
            mpb.SetColor(auraColorID, auraColor);
        }
        else
        {
            mpb.SetFloat(auraOnID, 0f);
        }

        rend.SetPropertyBlock(mpb);
    }
}
