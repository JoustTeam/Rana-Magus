using UnityEngine;

public class WallVariantView : MonoBehaviour
{
    [SerializeField] private Transform startCap;
    [SerializeField] private Transform body;
    [SerializeField] private Transform endCap;
    [SerializeField] private Renderer bodyRenderer;

    private static readonly int TilingId = Shader.PropertyToID("_BaseMap_ST");
    private MaterialPropertyBlock block;

    public void Resize(float length, float capLength, float tilesPerMeter)
    {
        float bodyLength = Mathf.Max(0f, length - capLength * 2f);

        startCap.localPosition = Vector3.zero;
        body.localPosition = new Vector3(0f, 0f, capLength);
        body.localScale = new Vector3(1f, 1f, bodyLength);
        endCap.localPosition = new Vector3(0f, 0f, length - capLength);

        ApplyTextureTiling(bodyLength, tilesPerMeter);
    }

    private void ApplyTextureTiling(float bodyLength, float tilesPerMeter)
    {
        if (bodyRenderer == null) return;

        block ??= new MaterialPropertyBlock();
        bodyRenderer.GetPropertyBlock(block);

        block.SetVector(TilingId, new Vector4(bodyLength * tilesPerMeter, 1f, 0f, 0f));
        bodyRenderer.SetPropertyBlock(block);
    }
}