using UnityEngine;

public class Layouts
{

    readonly static public Vector2[] roundTextPositons =
        { new Vector2(-0.5f, 1f),new Vector2(0f,1f),new Vector2(0.5f,1f),
        new Vector2(-0.45f, 0.4f),new Vector2(0.1f,0.4f),new Vector2(0.5f,0.4f),
        new Vector2(-0.45f,-0.2f),new Vector2(0.1f,-0.2f),new Vector2(0.5f,-0.2f)};

    readonly static public Vector2[] roundTextScales =
        {new Vector2(0.5f,0.5f),new Vector2(0.5f,0.5f),new Vector2(0.5f,0.5f),
        new Vector2(1.5f,0.5f),new Vector2(0.5f,0.5f),new Vector2(0.5f,0.5f),
        new Vector2(1.5f,0.5f),new Vector2(0.5f,0.5f),new Vector2(0.5f,0.5f)};

    readonly static public Vector3[] roundTextRotations =
        {new Vector3(0f,0f,0f),new Vector3(0f,0f,0f),new Vector3(0f,0f,0f),
        new Vector3(0f,0f,90f),new Vector3(0f,0f,0f),new Vector3(0f,0f,0f),
        new Vector3(0f,0f,90f),new Vector3(0f,0f,0f),new Vector3(0f,0f,0f)};


    readonly static public Vector2[] scoreTextOffsets =
        {new Vector2(-0.7f,-0.65f),new Vector2(1f,-0.2f),
       new Vector2(0.7f,1.5f),new Vector2(-1f,1.1f)};

    readonly static public Vector2[] scoreTextScales =
        {new Vector2(0.3f,0.3f),new Vector2(0.3f,0.3f),
        new Vector2(0.3f,0.3f),new Vector2(0.3f,0.3f)};

    readonly static public Vector2[] scoreTextLineupDirections =
        {new Vector2(0.21f,0f),new Vector2(0f,0.21f),
        new Vector2(-0.21f,0f),new Vector2(0f,-0.21f)};

    readonly static public Vector3[] scoreTextRotations =
        {new Vector3(0f,0f,0f),new Vector3(0f,0f,90f),
        new Vector3(0f,0f,180f),new Vector3(0f,0f,-90f)};

    readonly static public Vector2[] scoreTextCompassSpaces =
        {new Vector2(0.3f,0f),new Vector2(0f,0.3f),
        new Vector2(-0.3f,0f),new Vector2(0f,-0.3f)};


    readonly static public Vector2[] handOffsets =
        { new Vector2(-7f, -4f) ,new Vector2(7f,-2f),
        new Vector2(3.5f,4.5f),new Vector2(-7f,4.5f)};

    readonly static public Vector2[] handScales =
        {new Vector2(1.5f,1.5f),new Vector2(0.75f,0.75f),
        new Vector2(0.75f,0.75f),new Vector2(0.75f,0.75f)};

    readonly static public Vector2[] handLineupDirections =
        { new Vector2(1f, 0f), new Vector2(0f,0.5f) ,
        new Vector2(-0.5f,0f) ,new Vector2(0f,-0.5f)};

    readonly static public Vector3[] handRotations =
        { new Vector3(0f, 0f, 0f),new Vector3(0f,0f,90f),
        new Vector3(0f,0f,180f),new Vector3(0f,0f,-90f)};


    readonly static public Vector2[] tableOffsets =
        { new Vector2(-1f, -1.2f) ,new Vector2(1.55f,-0.55f),
        new Vector2(1f,2.05f),new Vector2(-1.55f,1.45f)};

    readonly static public Vector2[] tableScales =
        {new Vector2(0.6f,0.6f),new Vector2(0.6f,0.6f),
        new Vector2(0.6f,0.6f),new Vector2(0.6f,0.6f)};

    readonly static public Vector2[] tableLineupNextDirections =
        { new Vector2(0.405f, 0f),new Vector2(0f,0.405f) ,
        new Vector2(-0.405f,0f) ,new Vector2(0f,-0.405f) };

    readonly static public Vector2[] tableLineupNewLineDirections =
        { new Vector2(0f, -0.55f),new Vector2(0.55f,0f) ,
        new Vector2(0f,0.55f) ,new Vector2(-0.55f,0f) };

    readonly static public Vector3[] tableRotations =
        { new Vector3(0f, 0f, 0f),new Vector3(0f,0f,90f),
        new Vector3(0f,0f,180f),new Vector3(0f,0f,-90f)};

}