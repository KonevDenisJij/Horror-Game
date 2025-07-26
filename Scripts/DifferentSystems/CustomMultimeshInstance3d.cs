using Godot;
using System.Security.Cryptography;

[Tool]
public partial class CustomMultimeshInstance3d : MultiMeshInstance3D
{
    [Export]
    private Texture2D _splatMap;
    [Export]
    private MeshInstance3D _groundMesh;
    [Export]
    private bool RandomizeGrassPosition { get => _randomizeGrassPosition; set
        {
            _randomizeGrassPosition = !_randomizeGrassPosition;
            if (_randomizeGrassPosition)
            {
                RandomizeGrass();
            }
        } }

    private bool _randomizeGrassPosition = false;

    private void RandomizeGrass()
    {
        /*var mdt = new MeshDataTool();
        mdt.CreateFromSurface(_groundMesh.Mesh as ArrayMesh, 0);

        /*for (var i = 0; i < mdt.GetVertexCount(); i++)
        {
            var vertexUv = mdt.GetVertexUV(i);
            var vertex = mdt.GetVertex(i);
            // In this example we extend the mesh by one unit, which results in separated faces as it is flat shaded.
            //vertex += mdt.GetVertexNormal(i);
            // Save your change.
            //mdt.SetVertex(i, vertex);
            GD.Print("-------");
            GD.Print(vertex);
            GD.Print(vertexUv);
            GD.Print((int)(vertexUv.X * _splatMap.GetSize().X));
            GD.Print((int)(vertexUv.Y * _splatMap.GetSize().Y));
            GD.Print(_splatMap.GetImage().GetPixel((int)(vertexUv.X * _splatMap.GetSize().X), (int)(vertexUv.Y * _splatMap.GetSize().Y)));
        }*/

        /*var random = new Godot.RandomNumberGenerator();

        for (int i = 0; i < Multimesh.InstanceCount; i++)
        {
            var x = random.RandfRange(-40, 30);
            var z = random.RandfRange(-20, 15);
            var uvCoords = GetUvCoords(new Vector3(x, 0, z), Vector3.Up, mdt);
            if (uvCoords == null) 
            {
                i--;
                continue;
            }

            if (_splatMap.GetImage().GetPixel((int)(uvCoords.Value.X * _splatMap.GetSize().X), (int)(uvCoords.Value.Y * _splatMap.GetSize().Y)).G > 0.2)
            {
                var r = random.RandfRange(0, 1.57f);
                Multimesh.SetInstanceTransform(i, new Transform3D(new Basis(Vector3.Up, r), new Vector3(x, 0, z)));
            }
            else
            {
                i--;
                continue;
            }
        }*/
    }

    private bool EqualsWithEpsilon(Vector3 v1, Vector3 v2, float epsilon)
    {
        if (v1.DistanceTo(v2) < epsilon)
        {
            return true;
        }

        return false;
    }

    private int? GetFace(Vector3 point, Vector3 normal, MeshDataTool mdt, float epsilon = 0.2f)
    {
        for(var i = 0; i < mdt.GetFaceCount(); i++)
        {
            if (!EqualsWithEpsilon(mdt.GetFaceNormal(i), normal, epsilon))
            {
                continue;
            }

            var v1 = mdt.GetVertex(mdt.GetFaceVertex(i, 0));

            var v2 = mdt.GetVertex(mdt.GetFaceVertex(i, 1));

            var v3 = mdt.GetVertex(mdt.GetFaceVertex(i, 2));


            if (IsPointInTriangle(point, v1, v2, v3))
            {
                return i;
            }
        }

        return null;
    }

    private Vector3 CartToBary(Vector3 p, Vector3 a, Vector3 b, Vector3 c)
    {
        var v0 = b - a;
        var v1 = c - a;
        var v2 = p - a;
        var d00 = v0.Dot(v0);
        var d01 = v0.Dot(v1);
        var d11 = v1.Dot(v1);
        var d20 = v2.Dot(v0);
        var d21 = v2.Dot(v1);
        var denom = d00 * d11 - d01 * d01;

        var v = (d11 * d20 - d01 * d21) / denom;

        var w = (d00 * d21 - d01 * d20) / denom;

        var u = 1.0f - v - w;

        return new Vector3(u, v, w);
    }

    private bool IsPointInTriangle(Vector3 point, Vector3 v1, Vector3 v2, Vector3 v3)
    {
        var bc = CartToBary(point, v1, v2, v3);

        if (bc.X < 0 || bc.X > 1)
		    return false;

        if (bc.Y < 0 || bc.Y > 1)
		    return false;

        if (bc.Z < 0 || bc.Z > 1)
		    return false;

        return true;
    }


    private Vector2? GetUvCoords(Vector3 point, Vector3 normal, MeshDataTool mdt)
    {
        var face = GetFace(point, normal, mdt);

        if (face == null)
            return null;

        var v1 = mdt.GetVertex(mdt.GetFaceVertex(face.Value, 0));

        var v2 = mdt.GetVertex(mdt.GetFaceVertex(face.Value, 1));

        var v3 = mdt.GetVertex(mdt.GetFaceVertex(face.Value, 2));

        var bc = CartToBary(point, v1, v2, v3);

        var uv1 = mdt.GetVertexUV(mdt.GetFaceVertex(face.Value, 0));

        var uv2 = mdt.GetVertexUV(mdt.GetFaceVertex(face.Value, 1));

        var uv3 = mdt.GetVertexUV(mdt.GetFaceVertex(face.Value, 2));

        return (uv1 * bc.X) + (uv2 * bc.Y) + (uv3 * bc.Z);
    }
}