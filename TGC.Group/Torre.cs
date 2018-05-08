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
    public TGCVector3 posicion;
    private Arma arma;
    private TGCVector3 turretShotSize = new TGCVector3(3f,3f,-8f);
    private float rangoMaximo = 5000f;

    public Torre(string MediaDir, List<TGCVector4> posicionesTorres)
    {
        this.Scene = new TgcSceneLoader().loadSceneFromFile(MediaDir + "XWing/torreta2-TgcScene.xml", MediaDir + "XWing/");
        ScaleFactor = new TGCVector3(2f, 2f, 2f);
        posicion = obtenerPosicionAlAzar(posicionesTorres);
        arma = new Arma(turretShotSize, Color.Green);
        
    }
    public TGCVector3 obtenerPosicionAlAzar(List<TGCVector4> posicionesTorres)
    {
        Random random = new Random();
        int indice = random.Next(posicionesTorres.Count);
        TGCVector4 vector = posicionesTorres[indice];
        posicion.X = vector.X;
        posicion.Y = vector.Y;
        posicion.Z = vector.Z;
        return posicion;
    }

    public float distanciaObjetivo(TGCVector3 posicionObjetivo)
    {
        return (this.posicion-posicionObjetivo).Length();
    }

    public void disparar(TGCVector3 targetPosition)
    {
        this.arma.Disparar(this.posicion,targetPosition);
    }

    public bool enRango(TGCVector3 targetPosition)
    {
        return this.distanciaObjetivo(targetPosition) <= rangoMaximo;
    }

    public void ActionOnTorre(System.Action<TgcMesh> action)
    {
        this.Scene.Meshes.ForEach(action);
    }
    public void Update()
    {
        this.arma.Update();
    }
    public void Render()
    {
        this.ActionOnTorre(mesh => { mesh.Position = posicion; mesh.Scale= ScaleFactor; mesh.Render();});
        this.arma.Render();
    }
    public void Dispose()
    {
        this.ActionOnTorre(mesh => mesh.Dispose());
    }
}
