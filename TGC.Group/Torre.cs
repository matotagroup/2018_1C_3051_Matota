using System;
using System.Collections.Generic;
using System.Drawing;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Group;

public class Torre
{
    private TGCVector3 ScaleFactor { get; set; } 
    public TgcScene Scene { get; set; }

    public List<Disparo> disparos;

    public TGCVector3 posicion;
    public TGCVector3 rotacion;

    public Arma arma;
    private TGCVector3 turretShotSize = new TGCVector3(3f, 3f, 8f);
    private float rangoMaximo = 5000f;
    private TGCVector3 posicionInicialArma;

    public static readonly List<string> modelosDisponibles = new List<string> {
        "torreta2-TgcScene.xml", "Turbolaser-TgcScene.xml"
    };

    public Torre(string MediaDir)
    { 
        this.Scene = new TgcSceneLoader().loadSceneFromFile(MediaDir + "XWing/" + modelosDisponibles[new Random().Next(modelosDisponibles.Count)], MediaDir + "XWing/");

        ScaleFactor = new TGCVector3(5f, 5f, 5f);
        posicionArma.TryGetValue(new Tuple<string,float>(this.Scene.SceneName,0), out posicionInicialArma);
        arma = new Arma(turretShotSize, Color.Green, 1, 350, posicionInicialArma);

        this.ActionOnTorre(mesh => {
            mesh.AutoTransform = false;
        });
    }

    private static readonly Dictionary<Tuple<string,float>, TGCVector3> posicionArma = new Dictionary<Tuple<string,float>, TGCVector3>
    {
        { new Tuple<string, float> ("torreta2",0), new TGCVector3(150f,200f,0) },
        { new Tuple<string, float> ("torreta2",FastMath.PI), new TGCVector3(-150f,200f,0) },
        { new Tuple<string, float> ("Turbolaser",0), new TGCVector3(365f,242f,60f) },
        { new Tuple<string, float> ("Turbolaser",FastMath.PI), new TGCVector3(-358f,240f,68f) }
    };



    public void Relocate(TGCVector4 newPosition)
    {
        this.posicion = new TGCVector3(newPosition.X, newPosition.Y, newPosition.Z);
        this.rotacion = new TGCVector3(0, newPosition.W, 0);
        posicionArma.TryGetValue(new Tuple<string,float>(this.Scene.SceneName,newPosition.W), out posicionInicialArma);
        arma.Move(this.posicion+posicionInicialArma);
    }

    public float DistanciaObjetivo(TGCVector3 posicionObjetivo)
    {
        return (this.posicion - posicionObjetivo).Length();
    }

    public void Disparar(TGCVector3 targetPosition)
    {
        this.arma.Disparar(targetPosition);
    }

    public bool EnRango(TGCVector3 targetPosition)
    {
        return this.DistanciaObjetivo(targetPosition) <= rangoMaximo;
    }
    public void ActionOnTorre(System.Action<TgcMesh> action)
    {
        this.Scene.Meshes.ForEach(action);
    }
    public void Update()
    {
        this.arma.Update();
    }
    public void Render(bool renderBoundingBox = false)
    {
        this.ActionOnTorre(mesh => {
            mesh.Transform = TGCMatrix.Scaling(ScaleFactor) * TGCMatrix.RotationYawPitchRoll(rotacion.Y, rotacion.X, rotacion.Z) * TGCMatrix.Translation(posicion);
            mesh.Render();
        });

        this.Scene.BoundingBox.transform(Scene.Meshes[0].Transform);

        if(renderBoundingBox)
            this.Scene.BoundingBox.Render();
        this.arma.Render();
    }
    public bool CheckIfMyShotsCollided(NaveEspacial otraNave)
    {
        return arma.CheckShots(otraNave);
    }
    public void Dispose()
    {
        this.ActionOnTorre(mesh => mesh.Dispose());
    }
}
