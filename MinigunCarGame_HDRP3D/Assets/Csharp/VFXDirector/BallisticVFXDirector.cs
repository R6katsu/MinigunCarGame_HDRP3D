using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.VFX;

public class BallisticVFXDirector : SingletonMonoBehaviour<BallisticVFXDirector>
{
    /*
    public VisualEffect effect;     // ’e“¹‚ÌVFX
    private GraphicsBuffer startPositionsBuffer;
    private GraphicsBuffer endPositionsBuffer;
    private List<Vector3> startPositions = new();
    private List<Vector3> endPositions = new();

    public void ADADA(Vector3 startPoint, Vector3 endPoint)
    {
        transform.position = startPoint;

        startPositions.Add(startPoint);
        endPositions.Add(endPoint);

        // start
        startPositionsBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, startPositions.Count, sizeof(float) * 3);
        startPositionsBuffer.SetData(startPositions);

        // end
        endPositionsBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, endPositions.Count, sizeof(float) * 3);
        endPositionsBuffer.SetData(endPositions);

        // GraphicsBuffer‚ðVFX Graph‚É“n‚·
        effect.SetGraphicsBuffer("StartPositionsBuffer", startPositionsBuffer);

        // GraphicsBuffer‚ðVFX Graph‚É“n‚·
        effect.SetGraphicsBuffer("EndPositionsBuffer", endPositionsBuffer);

        effect.SendEvent("OnBallisticPlay");
    }
    */
}
