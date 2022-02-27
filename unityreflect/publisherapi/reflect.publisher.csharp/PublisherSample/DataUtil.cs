using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// gltf tech info
//http://any-programming.hatenablog.com/entry/2017/08/14/221028
// obj parser source
//https://github.com/stefangordon/ObjParser

using Newtonsoft.Json;
using System.Numerics;

using ObjParser;

using Unity.Reflect;
using Unity.Reflect.Model;
using Unity.Reflect.Utils;
using Unity.Reflect.UI;
using System.Drawing;

namespace PublisherSample
{
#region GLTF
    public class Point
    {
        public Point(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
        public float x;
        public float y;
    }

    public class Point3D
    {
        public Point3D(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        public float x;
        public float y;
        public float z;
    }
    public class Vector3D
    {
        public Vector3D(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        public float x;
        public float y;
        public float z;
    }

    /// 
    public enum ComponentType
    {
        Int8 = 5120,
        UInt8 = 5121,
        Int16 = 5122,
        UInt16 = 5123,
        Float = 5126
    }
    public enum DataType
    {
        VEC3,
        VEC2,
        SCALAR
    }
    public class Attributes
    {
        public int Normal { get; set; }
        public int Position { get; set; }
        public int Texcoord_0 { get; set; }
    }
    public class Primitive
    {
        public Attributes Attributes { get; set; }
        public int Indices { get; set; }
    }
    public class Mesh
    {
        public Primitive[] Primitives { get; set; }
    }
    public class Accessor
    {
        public int BufferView { get; set; }
        public int ByteOffset { get; set; }
        public ComponentType ComponentType { get; set; }
        public int Count { get; set; }
        public DataType Type { get; set; }
    }
    public class Image
    {
        public string Uri { get; set; }
    }
    public class BufferView
    {
        public int Buffer { get; set; }
        public int ByteOffset { get; set; }
        public int ByteLength { get; set; }
    }
    public class Buffer
    {
        public string Uri { get; set; }
    }
    public class GLTF
    {
        public Mesh[] Meshes { get; set; }
        public Accessor[] Accessors { get; set; }
        public Image[] Images { get; set; }
        public BufferView[] BufferViews { get; set; }
        public Buffer[] Buffers { get; set; }

        //

    }

    public static class GLTFext
    {
        public static int Size(this ComponentType type) =>
            type == ComponentType.Float ? 4 :
            type == ComponentType.Int16 ? 2 :
            type == ComponentType.UInt16 ? 2 :
            1;
        public static int Size(this DataType type) =>
            type == DataType.SCALAR ? 1 :
            type == DataType.VEC2 ? 2 :
            3;
        //

        public static byte[][] AccessorBytes(this GLTF gltf, string directory)
        {
            var buffers =
                gltf.Buffers
                .Select(x => File.ReadAllBytes($"{directory}\\{x.Uri}"))
                .ToArray();

            var bufferViews =
                gltf.BufferViews
                .Select(x => buffers[x.Buffer]
                             .Skip(x.ByteOffset)
                             .Take(x.ByteLength))
                .ToArray();

            var accessors =
                gltf.Accessors
                .Select(x => bufferViews[x.BufferView]
                             .Skip(x.ByteOffset)
                             .Take(x.Count * x.ComponentType.Size() * x.Type.Size())
                             .ToArray())
                .ToArray();

            return accessors;
        }
    }

    class Duck
    {
        public Vector3[] Positions { get; }
        public Vector3[] Normals { get; }
        public Vector2[] Texcoords { get; }
        public int[] Indices { get; }
        //public ImageSource Texture { get; }
        public Duck(string directory, GLTF gltf)
        {
            var accessorBytes = gltf.AccessorBytes(directory);

            var primitive = gltf.Meshes[0].Primitives[0];

            Indices = ConvertTo(accessorBytes[primitive.Indices], 2, (bytes, start) =>
                (int)BitConverter.ToUInt16(bytes, start)
            );
            Positions = ConvertTo(accessorBytes[primitive.Attributes.Position], 12, (bytes, start) =>
                new Vector3(BitConverter.ToSingle(bytes, start),
                            BitConverter.ToSingle(bytes, start + 4),
                            BitConverter.ToSingle(bytes, start + 8))
            );
            Normals = ConvertTo(accessorBytes[primitive.Attributes.Normal], 12, (bytes, start) =>
                new Vector3(BitConverter.ToSingle(bytes, start),
                             BitConverter.ToSingle(bytes, start + 4),
                             BitConverter.ToSingle(bytes, start + 8))
            );
            Texcoords = ConvertTo(accessorBytes[primitive.Attributes.Texcoord_0], 8, (bytes, start) =>
                new Vector2(BitConverter.ToSingle(bytes, start),
                          BitConverter.ToSingle(bytes, start + 4))
            );
            //Texture = gltf.Images.Select(x => new BitmapImage(new Uri($"{directory}\\{x.Uri}"))).Single();
        }
        T[] ConvertTo<T>(byte[] bytes, int stepBytes, Func<byte[], int, T> convert)
        {
            T[] dst = new T[bytes.Length / stepBytes];
            for (int i = 0; i < dst.Length; ++i)
                dst[i] = convert(bytes, stepBytes * i);
            return dst;
        }
    }
#endregion

    // OBJ
    class DataUtil
    {
        public Obj obj = null;
        public Mtl mtl = null;
        public string fileName = "";

        static public void OpenGltf(string file)
        {
            string json = System.IO.File.ReadAllText(file); // ファイル内容をjson変数に格納
            GLTF gltf = JsonConvert.DeserializeObject<GLTF>(json);
            string directory = System.IO.Path.GetDirectoryName(file);
            Duck duck = new Duck(directory, gltf);

            Console.WriteLine("finish.");

        }

        public void OpenObj(string file)
        {
            string objFile =  file.Substring(0, file.Length - 3) + "obj";
            string mtlFile = file.Substring(0, file.Length - 3) + "mtl";

            obj = new Obj();
            obj.LoadObj(objFile);
            mtl = new Mtl();
            mtl.LoadMtl(mtlFile);

            Console.WriteLine("finish.");

        }

        public SyncMesh BuildMesh(Obj curObj)
        {
            SyncMesh syncmesh = new SyncMesh(new SyncId("MeshId_" + curObj.Name), "Mesh_"+curObj.Name);

            // Faceの定義に従う。無駄は多くなる
            syncmesh.SubMeshes = new List<SyncSubMesh>();
            SyncSubMesh submesh = new SyncSubMesh();
            for (int j = 0; j < curObj.FaceList.Count; j++)
            {
                    ObjParser.Types.Face face = curObj.FaceList[j];

                for (int i = 0; i < face.VertexIndexList.Length; i++)
                {
                    //idx
                    int vertexIdx = face.VertexIndexList[i] - 1;
                    int uvIdx = face.TextureVertexIndexList[i] - 1;
                    int normalIdx = face.NormalIndexList[i] - 1;
                    //Console.WriteLine(string.Format("index[{0}] vert[{1}] uv[{2}] normal[{3}]",i, vertexIdx+1, uvIdx+1, normalIdx+1));

                    //ObjParser.Types.Vertex vertex = curObj.VertexList[vertexIdx];
                    //ObjParser.Types.TextureVertex uv = curObj.TextureList[uvIdx];
                    //ObjParser.Types.VertexNormal normal = curObj.VertexNormalList[normalIdx];
                    ObjParser.Types.Vertex vertex = obj.VertexList[vertexIdx];
                    ObjParser.Types.TextureVertex uv = obj.TextureList[uvIdx];
                    ObjParser.Types.VertexNormal normal = obj.VertexNormalList[normalIdx];

                    //頂点
                    // OpenGL->DirectX変換のため、Xは反転する
                    syncmesh.Vertices.Add(new Vector3(vertex.X * -1.0f, vertex.Y, vertex.Z)); // 2021.5.26 X軸を反転
                    syncmesh.Uvs.Add(new Vector2(uv.X, uv.Y));
                    syncmesh.Normals.Add(new Vector3(normal.X * -1.0f, normal.Y, normal.Z)); // 2021.5.26 X軸を反転
                    //syncmesh.Normals.Add(new Vector3(0,0,1));
                    //Console.WriteLine(string.Format("vert={0}\t{1}\t{2}", (float)vertex.X, (float)vertex.Y, (float)vertex.Z));

                    //index
                    //submesh.Triangles.Add(i + j * face.VertexIndexList.Length);
                    // OpenGL->DirectX変換のため、逆順でセットする
                    submesh.Triangles.Add((face.VertexIndexList.Length - 1 - i) + j * face.VertexIndexList.Length);
                }
            }
            syncmesh.SubMeshes.Add(submesh);

            return syncmesh;
        }

        public SyncMesh BuildMesh_Legacy(Obj curObj)
        {
            SyncMesh syncmesh = new SyncMesh(new SyncId("MeshId_" + curObj.Name), "Mesh_" + curObj.Name);

            foreach (ObjParser.Types.Vertex vertex in curObj.VertexList)
            {
                syncmesh.Vertices.Add(new Vector3((float)vertex.X, (float)vertex.Y, (float)vertex.Z));
            }

            foreach (ObjParser.Types.VertexNormal normal in curObj.VertexNormalList)
            {
                syncmesh.Normals.Add(new Vector3((float)normal.X, (float)normal.Y, (float)normal.Z));
            }

            foreach (ObjParser.Types.TextureVertex uv in curObj.TextureList)
            {
                syncmesh.Uvs.Add(new Vector2((float)uv.X, (float)uv.Y));
            }

            syncmesh.SubMeshes = new List<SyncSubMesh>();
            SyncSubMesh submesh = new SyncSubMesh();
            foreach (ObjParser.Types.Face face in curObj.FaceList)
            {
                //string[] line = face.ToString().Split(' ');
                //foreach (string data in line)
                //{
                //    string[] facegeom = data.Split('/');
                //    int result = 0;
                //    if (int.TryParse(data, out result))
                //    {
                //        submesh.Triangles.Add(result);
                //    }
                //}
                foreach (int idx in face.VertexIndexList)
                {
                    submesh.Triangles.Add(idx);
                }
            }
            syncmesh.SubMeshes.Add(submesh);

            return syncmesh;
        }


        public List<SyncMesh> BuildMeshAry()
        {
            List<SyncMesh> meshAry = new List<SyncMesh>();

            for (int i = 0; i < obj.MeshList.Count; i++) {
                Obj curObj = obj.MeshList[i];

                // Create a hardcoded quad mesh
                SyncMesh syncmesh = new SyncMesh(new SyncId(curObj.Name), curObj.Name);

                foreach (ObjParser.Types.Vertex vertex in curObj.VertexList) 
                {
                    syncmesh.Vertices.Add(new Vector3((float)vertex.X, (float)vertex.Y, (float)vertex.Z));
                }

                foreach (ObjParser.Types.Vertex normal in curObj.VertexList)
                {
                    syncmesh.Normals.Add(new Vector3(0, 0, 1));
                }

                foreach (ObjParser.Types.TextureVertex uv in curObj.TextureList)
                {
                    syncmesh.Uvs.Add(new Vector2((float)uv.X, (float)uv.Y));
                }

                meshAry.Add(syncmesh);
            }

            return meshAry;
        }

        public SyncMaterial BuildMaterial(Obj curObj, PublisherTransaction transaction)
        {
            SyncMaterial material = null;
            ObjParser.Types.Material mat = FindMaterial(curObj.UseMtl);
            if (mat != null)
            {
                material = new SyncMaterial(new SyncId("MaterialId_" + mat.Name), mat.Name);
                Random random = new Random();
                //material.AlbedoColor = new SyncColor((float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble());
                material.AlbedoColor = SyncColor.From256(200,200,200); // test

                Console.WriteLine("\t\tColor: " 
                    + material.AlbedoColor.R.ToString() + ", "
                    + material.AlbedoColor.G.ToString() + ", "
                    + material.AlbedoColor.B.ToString() + ", " );

                // テクスチャが見つかった場合
                if (mat.map_Kd.Length > 0)
                {
                    //material.AlbedoFade = 1.0f; // 0.5f; //2021.5.26
                    SyncTexture texture = BuildTexture(mat);
                    if (texture != null)
                    {
                        transaction.Send(texture);
                        Console.WriteLine("\tTexture Id: " + texture.Id);
                        Console.WriteLine("\tTexture Name: " + texture.Name);

                        SyncMap albedoMap = new SyncMap(texture.Id, new Vector2(), new Vector2(1,1));
                        material.AlbedoMap = albedoMap;
                    }
                }
            } else
            {
                Console.WriteLine("Warning: No Material.");
                material = new SyncMaterial(new SyncId("MaterialId_" + "DEFAULT"), "DEFAULT");
                material.AlbedoColor = SyncColor.From256(200, 0, 0); 
            }
            return material;
        }

        public SyncTexture BuildTexture(ObjParser.Types.Material mat)
        {
            // ストリップパスとか相対パスの場合を考慮する
            string filePath = System.IO.Path.GetDirectoryName(mat.map_Kd);
            if (filePath.Length == 0)
            {
                filePath = System.IO.Path.GetDirectoryName(fileName) + "\\" + mat.map_Kd;
            }

            string name = System.IO.Path.GetFileNameWithoutExtension(filePath);
            string ext = System.IO.Path.GetExtension(filePath);
            SyncTexture texture = new SyncTexture(new SyncId("TextureId_" + name), name);

            Console.WriteLine("\tTexture file: " + filePath);
            try
            {
                //System.Drawing.Image img = System.Drawing.Image.FromFile(filePath);
#if false
                // 最初の案→なんかおかしい
                byte[] ary;
                using (MemoryStream ms = new MemoryStream())
                {
                    if (ext.ToLower() == ".jpg" || ext.ToLower() == ".jpeg")
                    {
                        img.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                    } else if (ext.ToLower() == ".png")
                    {
                        img.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    } else
                    {
                        Console.WriteLine("\t" + mat.map_Kd + " is unknown format");
                        return null;
                    }
                    ary = ms.ToArray();
                }
                texture.Source = ary.ToArray();
#else
                // 更に改善：解像度を2のN乗にする
                bool requireResize = false;
                Bitmap bmp = new Bitmap(filePath);
                int width = bmp.Width;
                int height = bmp.Height;
                if (!IsPow2(width))
                {
                    requireResize = true;
                    int beki = 1;
                    while(true)
                    {
                        if (width < beki)
                        {
                            width = beki / 2; // 1段階小さくする
                            break;
                        }
                        beki *= 2;
                    }
                }
                if (!IsPow2(bmp.Height))
                {
                    requireResize = true;
                    int beki = 1;
                    while (true)
                    {
                        if (height < beki)
                        {
                            height = beki / 2; // 1段階小さくする
                            break;
                        }
                        beki *= 2;
                    }
                }
                Bitmap bmpResize = new Bitmap(bmp, width, height);

                // ファイル名変更
                string resizeFileName = filePath;
                if (requireResize)
                {
                    resizeFileName.ToLower();
                    resizeFileName = resizeFileName.Replace(".jpg", "_resize.jpg"); // ファイル名差し替え
                    bmpResize.Save(resizeFileName); // リサイズして画像出力
                }
                bmpResize.Dispose();
                bmp.Dispose();

                // 2021.5.26の案 // UnityEditorで上手くいくのを確認した方法 // 
                System.IO.FileStream fileStream = new System.IO.FileStream(resizeFileName, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                System.IO.BinaryReader bin = new System.IO.BinaryReader(fileStream);
                byte[] ary = bin.ReadBytes((int)bin.BaseStream.Length);
                bin.Close();
                texture.Source = ary;

                /*
                // こっちだとどうかな // [?]という画像に置き換わる現象が出た
                List<byte> ary2 = new List<byte>();

                Bitmap bmp = new Bitmap(filePath);
                for (int x = 0; x < bmp.Width; x++)
                {
                    for (int y = 0; y < bmp.Height; y++)
                    {
                        Color color = bmp.GetPixel(x, y);
                        ary2.Add(color.R);
                        ary2.Add(color.G);
                        ary2.Add(color.B);
                        ary2.Add(color.A);
                    }
                }
                texture.Source = ary2.ToArray();
                */
#endif
            }
            catch (System.IO.FileNotFoundException e)
            {
                Console.WriteLine("FileNotFoundException: " + e.Message);
            }
            return texture;
        }

        // 2のべき乗であるかチェックする
        bool IsPow2(int x)
        {
            if (x == 0)
            {
                return false;
            }
            return (x & (x - 1)) == 0;
        }

        public List<SyncMaterial> BuildMaterialAry()
        {
            List<SyncMaterial> materialAry = new List<SyncMaterial>();

            // Create a basic colored material
            var material = new SyncMaterial(new SyncId("Material id"));
            Random random = new Random();
            material.AlbedoColor = SyncColor.From256(random.Next(0, 255),random.Next(0, 255),random.Next(0, 255));
            //mtl.MaterialList[0].;

            materialAry.Add(material);

            return materialAry;
        }

        public SyncObject BuildObject(SyncMesh mesh, SyncMaterial material, int idx)
        {
            // Create a parent object
            var parentObject = new SyncObject(new SyncId("ParentId_") + mesh.Id.ToString(), "ObjectParent_" + mesh.Name)
            {
                Transform = SyncTransform.Identity
            };
            
            // Create a child object with geometry
            var childObject = new SyncObject(new SyncId("ChildId_") + mesh.Id.ToString(), "ObjectChild_" + mesh.Name)
            {
                MeshId = mesh.Id,
                MaterialIds = new List<SyncId> { material.Id },
                Transform = SyncTransform.Identity
            };
            parentObject.Children.Add(childObject);
            
            // Fill the object with metadata
            parentObject.Metadata.Add("Key 1", new SyncParameter("Value", "Group", true));
            parentObject.Metadata.Add("Key 2", new SyncParameter("Other value", "Group", true));
            parentObject.Metadata.Add("メッシュID", new SyncParameter(mesh.Id.ToString(), "Group", true));
            parentObject.Metadata.Add("マテリアルID", new SyncParameter(material.Id.ToString(), "Group", true));

            parentObject.Metadata.Add("図形名", new SyncParameter(obj.MeshList[idx].Name, "Group", true));
            parentObject.Metadata.Add("マテリアル名", new SyncParameter(obj.MeshList[idx].UseMtl, "Group", true));
            parentObject.Metadata.Add("頂点数", new SyncParameter(obj.MeshList[idx].VertexList.Count.ToString(), "Group", true));

            parentObject.Metadata.Add("ポリゴン数", new SyncParameter(mesh.TriangleCount.ToString(), "Group", true));

            // Uncomment the following line to add the "Merge" metadata key.
            // Because of the rules.json file, this would trigger the MergeChildObjects action.
            //parentObject.Metadata.Add("Merge", new SyncParameter("", "Rules", true));

            return parentObject;
        }

        public SyncObjectInstance BuildObjectInstance(SyncObject obj)
        {
            // Create an instance out of the SyncObject
            return new SyncObjectInstance(new SyncId("InstanceId_" + obj.Id), "Instance_" + obj.Id, obj.Id)
            {
                Transform = SyncTransform.Identity,
                Metadata = obj.Metadata
            };
        }

        ObjParser.Types.Material FindMaterial(string matName)
        {
            ObjParser.Types.Material retMat = null;
            foreach(ObjParser.Types.Material mat in mtl.MaterialList)
            {
                if (mat.Name == matName)
                {
                    retMat = mat; 
                }
            }
            return retMat;
        }

    }
}
