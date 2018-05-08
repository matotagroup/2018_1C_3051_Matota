using System;
using System.Collections.Generic;
using System.Drawing;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Group;

public class Torre
{
    public TGCVector3 ScaleFactor { get; set; } 
    public TgcScene Scene { get; set; }
    public List<Disparo> disparos;

    public TGCVector3 posicion;
    public TGCVector3 rotacion;

    public static readonly List<string> modelosDisponibles = new List<string> {
        "torreta2-TgcScene.xml", "Turbolaser-TgcScene.xml"
    };

    public Torre(string MediaDir)
    { 
        this.Scene = new TgcSceneLoader().loadSceneFromFile(MediaDir + "XWing/" + modelosDisponibles[new Random().Next(modelosDisponibles.Count)], MediaDir + "XWing/");

        ScaleFactor = new TGCVector3(5f, 5f, 5f);

        this.ActionOnTorre(mesh => {
            mesh.AutoTransform = false;
        });
    }

    public void Relocate(TGCVector4 newPosition)
    {
        this.posicion = new TGCVector3(newPosition.X, newPosition.Y, newPosition.Z);
        this.rotacion = new TGCVector3(0, newPosition.W, 0);
    }

    public void ActionOnTorre(System.Action<TgcMesh> action)
    {
        this.Scene.Meshes.ForEach(action);
    }
    public void Render()
    {
        this.ActionOnTorre(mesh => {
            mesh.Transform = TGCMatrix.Scaling(ScaleFactor) * TGCMatrix.RotationYawPitchRoll(rotacion.Y, rotacion.X, rotacion.Z) * TGCMatrix.Translation(posicion);
            mesh.Render();
        });
    }
    public void Dispose()
    {
        this.ActionOnTorre(mesh => mesh.Dispose());
    }
}
