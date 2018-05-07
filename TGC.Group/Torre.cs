using System;
using System.Collections.Generic;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Group;

public class Torre
{
    public TGCVector3 ScaleFactor { get; set; } 
    public TgcScene Scene { get; set; }
    public List<Disparo> disparos;
    public TGCVector3 posicion;

    public Torre(string MediaDir,List<TGCVector4> torres)
    {
        this.Scene = new TgcSceneLoader().loadSceneFromFile(MediaDir + "XWing/torreta-TgcScene.xml", MediaDir + "XWing/");
        Random random = new Random();
        int indice = random.Next(torres.Count);
        TGCVector4 vector = torres[indice];
        posicion.X = vector.X;
        posicion.Y = vector.Y;
        posicion.Z = vector.Z;
        ScaleFactor = new TGCVector3(2f, 2f, 2f);
        
    }
    public void ActionOnTorre(System.Action<TgcMesh> action)
    {
        this.Scene.Meshes.ForEach(action);
    }
    public void Render()
    {
        this.ActionOnTorre(mesh => { mesh.Position = posicion; mesh.Scale= ScaleFactor; mesh.Render(); });
    }
    public void Dispose()
    {
        this.ActionOnTorre(mesh => mesh.Dispose());
    }
}
