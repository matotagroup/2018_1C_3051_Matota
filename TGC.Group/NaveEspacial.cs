using Microsoft.DirectX.Direct3D;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.BoundingVolumes;
using TGC.Core.Direct3D;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Shaders;
using TGC.Core.Textures;

namespace TGC.Group
{
    public class NaveEspacial
    {

        public TgcScene Scene { get; set; }

        public TGCMatrix TransformMatix { get; set; }

        public TGCMatrix ScaleFactor { get; set; }
        public TGCVector3 RotationVector { get; set; }
        public TGCVector3 MovementVector { get; set; }
        public float speed = 1000f;
        private bool shouldBarrelRoll = false;
        private bool shouldLeft90Spin = false;
        private bool shouldRight90Spin = false;
        private bool stopSpinning = false;
        public Arma ArmaPrincipal { get; private set; }
        private TGCVector3 shipShotSize = new TGCVector3(2f, 2f, 8f);

        private Effect effect;
        private TGCVector3 g_LightDir; // direccion de la luz actual
        private TGCVector3 g_LightPos; // posicion de la luz actual (la que estoy analizando)
        private TGCMatrix g_LightView; // matriz de view del light
        private TGCMatrix g_mShadowProj; // Projection matrix for shadow map
        private Surface g_pDSShadow; // Depth-stencil buffer for rendering to shadow map
        private Texture g_pShadowMap; // Texture to which the shadow map is rendered
        private readonly int SHADOWMAP_SIZE = 1024;

        private Stopwatch coolDownMovimientos = null;


        private readonly float COOLDOWNMOVIMIENTOS = 2000;

        public int Vida { get; protected set; }  = 100;

        public float AfterBurnFuel { get; set; } = 100;
        public bool shouldSpeedBoost = false;

        public TgcBoundingOrientedBox OOB
        {
            private set; get;
        }

        public NaveEspacial(string MediaDir, string modelToUse, int danio, int cdDisparo, string defaultTechnique = null)
        {
            this.Scene = new TgcSceneLoader().loadSceneFromFile(MediaDir + "XWing/" + modelToUse, MediaDir + "XWing/");
            this.TransformMatix = TGCMatrix.Identity;
            this.ScaleFactor = TGCMatrix.Identity;
            this.RotationVector = TGCVector3.Empty;
            this.MovementVector = TGCVector3.Empty;

            this.ArmaPrincipal = new Arma(shipShotSize, Color.Red, danio, cdDisparo, this.GetPosition());
            this.coolDownMovimientos = Stopwatch.StartNew();


            this.ActionOnNave((mesh) => {
                mesh.AutoTransform = false; //Desactivar el autotransform para poder usar el mesh.transform.
                mesh.Effect = TgcShaders.Instance.TgcMeshPointLightShader;
                if (!string.IsNullOrEmpty(defaultTechnique))
                    mesh.Technique = defaultTechnique;
            });

            SetUpLighting();
        }

        //public NaveEspacial(string MediaDir, string modelToUse, int danio): this(MediaDir, modelToUse)
        //{
        //    this.ArmaPrincipal.Danio = danio;
        //}

        public void Daniar(int cantidadDanio)
        {
            if(!this.shouldBarrelRoll)
                this.Vida -= cantidadDanio;
        }

        public void GastarFuel(float cantUsada, Hud hud)
        {
            if (AfterBurnFuel < 1)
            {
                this.shouldSpeedBoost = false;
            }
            else
            {
                this.AfterBurnFuel = Math.Max(this.AfterBurnFuel - cantUsada, 0);
                hud.ReducirFuel(AfterBurnFuel);
            }
        }
        public void DoSpeedBoost()
        {
            OnCooldown(() => this.shouldSpeedBoost = true);
        }
        public void RecargarFuel(float cantUsada)
        {
            this.AfterBurnFuel = Math.Min(this.AfterBurnFuel + cantUsada, 100);
        }

        public void Disparar()
        {
            this.ArmaPrincipal.Disparar(this.MovementVector-new TGCVector3(0f,0f,1f));
        }

        public void Disparar(TGCVector3 target)
        {
            this.ArmaPrincipal.Disparar(target);
        }

        public void CreateOOB()
        {
            //Hacemos un OOB a partir de un AABB mas chico porque el original es muy grande.
            this.OOB = TgcBoundingOrientedBox.computeFromAABB(new TgcBoundingAxisAlignBox(TGCVector3.Multiply(Scene.BoundingBox.PMin, 0.5f), TGCVector3.Multiply(Scene.BoundingBox.PMax, 0.5f)));
            this.OOB.move(this.MovementVector);
            this.OOB.rotate(this.RotationVector);
        }

        public TGCMatrix RotationMatrix()
        {
            return TGCMatrix.RotationYawPitchRoll(RotationVector.Y, RotationVector.X, RotationVector.Z);
        }

        public TGCMatrix MovementMatrix()
        {
            return TGCMatrix.Translation(MovementVector);
        }
        private void PerformBarrelRoll(float ElapsedTime)
        {
            /*TGCVector3 origPosition = new TGCVector3(1, 0, 0);

            var v3 = new TGCVector3(-1, 0, 19);
            var v4 = new TGCVector3(3, 0, -5);
            var angle = FastMath.Acos(TGCVector3.Dot(TGCVector3.Normalize(v3), TGCVector3.Normalize(v4)));
            
            float mm = TGCVector3.Dot(this.RotationVector, origPosition);
            if (FastMath.Acos(mm) < FastMath.TWO_PI)*/

           
            if (this.RotationVector.X > -FastMath.TWO_PI)
                this.Rotate(new TGCVector3( FastMath.ToRad(-500 * ElapsedTime), 0, 0));
            else
            {
                this.Rotate(new TGCVector3(-this.RotationVector.X, 0, 0), false);
                this.OOB.setRotation(new TGCVector3(0,FastMath.PI_HALF,0));
                this.shouldBarrelRoll = false;
            }
            
        }

        private void SetUpLighting()
        {
            var effect = TgcShaders.Instance.TgcMeshPointLightShader;

            effect.SetValue("ambientColor", ColorValue.FromColor(Color.FromArgb(255, 85, 85, 85)));
            effect.SetValue("diffuseColor", ColorValue.FromColor(Color.WhiteSmoke)); 
            effect.SetValue("specularColor", ColorValue.FromColor(Color.FromArgb(255, 255, 255, 255)));
            effect.SetValue("specularExp", 200f);
        }

        private void LightingUpdate(TGCVector3 lightPosition, TGCVector3 lookFrom)
        {
            var effect = TgcShaders.Instance.TgcMeshPointLightShader;
            effect.SetValue("lightPosition", TGCVector3.Vector3ToFloat4Array(lightPosition));
            effect.SetValue("eyePosition", TGCVector3.Vector3ToFloat4Array(lookFrom));
        }

        public void DoBarrelRoll()
        {
            OnCooldown( () => this.shouldBarrelRoll = true );
        }

        private void OnCooldown(Action todo)
        {

            if (coolDownMovimientos.Elapsed.TotalMilliseconds >= COOLDOWNMOVIMIENTOS)
            {
                todo();
                coolDownMovimientos.Restart();
            }
        }

        private void PerformLeft90Spin(float ElapsedTime)
        {
            
            if (stopSpinning)
            {
                if (this.RotationVector.X < 0)
                    this.Rotate(new TGCVector3(FastMath.ToRad(250*ElapsedTime), 0, 0));
                else
                {
                    this.Rotate(new TGCVector3(-this.RotationVector.X, 0, 0), false);
                    this.OOB.setRotation(new TGCVector3(-this.RotationVector.X, FastMath.PI_HALF, 0));
                    this.shouldLeft90Spin = false;
                    this.stopSpinning = false;
                }
            }
            else
            {
                if (this.RotationVector.X < FastMath.PI_HALF)
                    this.Rotate(new TGCVector3(FastMath.ToRad(250 *ElapsedTime), 0, 0));
                else
                {
                    this.Rotate(new TGCVector3(-this.RotationVector.X + FastMath.PI_HALF, 0, 0), false);
                    this.OOB.setRotation(new TGCVector3(FastMath.PI_HALF, FastMath.PI_HALF, 0));
                    this.shouldLeft90Spin = false;
                    this.shouldRight90Spin = true;
                    this.stopSpinning = true;
                }
            }           
        }

        private void PerformRight90Spin(float ElapsedTime)
        {
            if (stopSpinning)
            {
                if (this.RotationVector.X > 0)
                    this.Rotate(new TGCVector3(FastMath.ToRad(-250*ElapsedTime), 0, 0));
                else
                {
                    this.Rotate(new TGCVector3(-this.RotationVector.X, 0, 0), false);
                    this.OOB.setRotation(new TGCVector3(-this.RotationVector.X, FastMath.PI_HALF, 0));
                    this.shouldRight90Spin = false;
                    this.stopSpinning = false;
                }
            }
            else
            {
                if (this.RotationVector.X > -FastMath.PI_HALF)
                {
                    this.Rotate(new TGCVector3(FastMath.ToRad(-250*ElapsedTime), 0, 0));
                }
                else
                {
                    this.Rotate(new TGCVector3(-this.RotationVector.X - FastMath.PI_HALF, 0, 0), false);
                    this.OOB.setRotation(new TGCVector3(-FastMath.PI_HALF, FastMath.PI_HALF, 0));
                    this.shouldRight90Spin = false;
                    this.shouldLeft90Spin = true;
                    this.stopSpinning = true;
                }
            }
        }

        public void DoLeft90Spin()
        {
                this.shouldLeft90Spin = true;
                this.shouldRight90Spin = false;
                this.stopSpinning = false;
        }

        public void DoRight90Spin()
        {
                this.shouldRight90Spin = true;
                this.shouldLeft90Spin = false;
                this.stopSpinning = false;
        }

        public void Rotate(TGCVector3 rotation, bool updateOOB = true)
        {
            if(updateOOB)
                this.OOB.rotate(new TGCVector3(rotation.Z, rotation.Y, -rotation.X));
            this.RotationVector = this.RotationVector + rotation;
        }

        public void MoveTo(TGCVector3 newAbsolutePosition)
        {
            this.OOB.Center = newAbsolutePosition;
            this.MovementVector = newAbsolutePosition;
            ArmaPrincipal.Move(this.GetPosition());
        }

        public void Move(TGCVector3 newOffset)
        {
            this.OOB.move(newOffset * speed);
            this.MovementVector = this.MovementVector + newOffset * speed;
            ArmaPrincipal.Move(this.GetPosition());
        }

        public TGCVector3 GetPosition()
        {
            return MovementVector;
        }


        public int PierdeVidas(int cantVidas)
        {
            Vida = cantVidas;

            return Vida;
        }

        public bool CheckIfMyShotsCollided(NaveEspacial otraNave)
        {
            return this.ArmaPrincipal.CheckShots(otraNave);
        }

        public void Update(float ElapsedTime)
        {
            if (shouldBarrelRoll)
                this.PerformBarrelRoll(ElapsedTime);
            else if (shouldLeft90Spin)
                this.PerformLeft90Spin(ElapsedTime);
            else if (shouldRight90Spin)
                this.PerformRight90Spin(ElapsedTime);

            this.ArmaPrincipal.Update(ElapsedTime);

        }

        public void Render(TGCVector3 lightPosition, TGCVector3 lookFrom, bool renderBoundingBox = false)
        {
            LightingUpdate(lightPosition, lookFrom);
            //this.SetearValoresShadowMap(lightPosition, lookFrom);
            //this.RenderShadowMap();
            if (renderBoundingBox)
                this.OOB.Render();
            
            this.ActionOnNave((mesh) => {
                mesh.Transform = TransformMatix;
                mesh.Render();
                //if (renderBoundingBox)
                //    mesh.BoundingBox.Render();
            });
            this.ArmaPrincipal.Render();
        }

        public void ActionOnNave(System.Action<TgcMesh> action)
        {
            this.Scene.Meshes.ForEach(action);
        }

        public void UpdateBoundingBox()
        {
            this.ActionOnNave((mesh) => {
                mesh.updateBoundingBox();
            });
        }

        public bool EstaViva()
        {
            return Vida > 0;
        }

        /// <summary>
        /// Mata a la nave, es decir pone su vida en 0.
        /// </summary>
        public void Morir()
        {
            Vida = 0;
        }

        public void Revivir()
        {
            Vida = 100;
            AfterBurnFuel = 100;
        }
        private void SetearValoresShadowMap(TGCVector3 lightPosition,TGCVector3 lightDir)
        {
            // Creo el shadowmap.
            // Format.R32F
            // Format.X8R8G8B8
            g_pShadowMap = new Texture(D3DDevice.Instance.Device, SHADOWMAP_SIZE, SHADOWMAP_SIZE, 1, Usage.RenderTarget, Format.R32F, Pool.Default);

            // tengo que crear un stencilbuffer para el shadowmap manualmente
            // para asegurarme que tenga la el mismo tamano que el shadowmap, y que no tenga
            // multisample, etc etc.
            g_pDSShadow = D3DDevice.Instance.Device.CreateDepthStencilSurface(SHADOWMAP_SIZE, SHADOWMAP_SIZE, DepthFormat.D24S8, MultiSampleType.None, 0, true);
            // por ultimo necesito una matriz de proyeccion para el shadowmap, ya
            // que voy a dibujar desde el pto de vista de la luz.
            // El angulo tiene que ser mayor a 45 para que la sombra no falle en los extremos del cono de luz
            // de hecho, un valor mayor a 90 todavia es mejor, porque hasta con 90 grados es muy dificil
            // lograr que los objetos del borde generen sombras
            var aspectRatio = D3DDevice.Instance.AspectRatio;
            g_mShadowProj = TGCMatrix.PerspectiveFovLH(Geometry.DegreeToRadian(80), aspectRatio, 50, 5000);
            //float far_plane = 1500f;
            // float near_plane = 2f;
        D3DDevice.Instance.Device.Transform.Projection = TGCMatrix.PerspectiveFovLH(Geometry.DegreeToRadian(45.0f), aspectRatio, 2f, 15000f).ToMatrix();
            var dir = new TGCVector3(0, -1, 0);
            dir.Normalize();
            g_LightPos = lightPosition;
            g_LightDir = dir;
            g_LightDir.Normalize();
        }
        public void setEffect(string ShadersDir)
        {
            effect = TgcShaders.loadEffect(ShadersDir + "ShadowMap.fx");

            // le asigno el efecto a las mallas
            foreach (var T in Scene.Meshes)
            {
                T.Scale = new TGCVector3(1f, 1f, 1f);
                T.Effect = effect;
            }
        }
        public void RenderShadowMap()
        {
            // Calculo la matriz de view de la luz
            effect.SetValue("g_vLightPos", new TGCVector4(g_LightPos.X, g_LightPos.Y, g_LightPos.Z, 1));
            effect.SetValue("g_vLightDir", new TGCVector4(g_LightDir.X, g_LightDir.Y, g_LightDir.Z, 1));
            g_LightView = TGCMatrix.LookAtLH(g_LightPos, g_LightPos + g_LightDir, new TGCVector3(0, 0, 1));

            // inicializacion standard:
            effect.SetValue("g_mProjLight", g_mShadowProj.ToMatrix());
            effect.SetValue("g_mViewLightProj", (g_LightView * g_mShadowProj).ToMatrix());

            // Primero genero el shadow map, para ello dibujo desde el pto de vista de luz
            // a una textura, con el VS y PS que generan un mapa de profundidades.
            var pOldRT = D3DDevice.Instance.Device.GetRenderTarget(0);
            var pShadowSurf = g_pShadowMap.GetSurfaceLevel(0);
            D3DDevice.Instance.Device.SetRenderTarget(0, pShadowSurf);
            var pOldDS = D3DDevice.Instance.Device.DepthStencilSurface;
            D3DDevice.Instance.Device.DepthStencilSurface = g_pDSShadow;

            // Hago el render de la escena pp dicha
            effect.SetValue("g_txShadow", g_pShadowMap);
            RenderScene(true);
            // restuaro el render target y el stencil
            D3DDevice.Instance.Device.DepthStencilSurface = pOldDS;
            D3DDevice.Instance.Device.SetRenderTarget(0, pOldRT);
        }
        private void RenderScene(bool shadow)
        {
            if (shadow)
            {
                this.ActionOnNave(mesh => mesh.Technique = "RenderShadow");
            }
            else
            {
                this.ActionOnNave(mesh => mesh.Technique = "RenderScene");
            }
            this.ActionOnNave(mesh => mesh.Render());
        }
    }
}
