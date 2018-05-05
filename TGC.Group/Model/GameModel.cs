using Microsoft.DirectX.DirectInput;
using System.Drawing;
using TGC.Core.Direct3D;
using TGC.Core.Example;
using TGC.Core.Geometry;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Textures;
using TGC.Core.Sound;
using TGC.Core.Terrain;
using Microsoft.DirectX;
using TGC.Core.Collision;
using System;
using System.Collections.Generic;

namespace TGC.Group.Model
{
    /// <summary>
    ///     Ejemplo para implementar el TP.
    ///     Inicialmente puede ser renombrado o copiado para hacer más ejemplos chicos, en el caso de copiar para que se
    ///     ejecute el nuevo ejemplo deben cambiar el modelo que instancia GameForm <see cref="Form.GameForm.InitGraphics()" />
    ///     line 97.
    /// </summary>
    public class GameModel : TgcExample
    {
        /// <summary>
        ///     Constructor del juego.
        /// </summary>
        /// <param name="mediaDir">Ruta donde esta la carpeta con los assets</param>
        /// <param name="shadersDir">Ruta donde esta la carpeta con los shaders</param>
        public GameModel(string mediaDir, string shadersDir) : base(mediaDir, shadersDir)
        {
            Category = Game.Default.Category;
            Name = Game.Default.Name;
            Description = Game.Default.Description;
        }

        //Esto se debe mover a la clase de la estrella de la muerte
        private bool shouldMove = false;
        private const float velocidadDisparo = -1f;
        List<Disparo> disparos;
        //Scenes
        private NaveEspacial navePrincipal;
        private TgcScene SceneEstrellaDeLaMuerte { get; set; }
        private TgcScene LeftWallEstrellaDeLaMuerte { get; set; }
        private TgcScene RightWallEstrellaDeLaMuerte { get; set; }

        private TgcSkyBox skyBox;


        private TGCVector3 movDisparo;

        private float movimientoZ = -2f;

        private float movimientoBaseZ = -2f;

        private float movimientoMaximoZ=-20f;

        private float factorMovimientoZ = 0.25f;

        //Sounds
        //private TgcMp3Player sonidoAmbiente;
        //private TgcMp3Player sonidoRotacion;
        //private TgcMp3Player sonidoVelocidad;

        //Codigo De caja previo
        /*//Caja que se muestra en el ejemplo.
        private TGCBox Box { get; set; }

        //Mesh de TgcLogo.
        private TgcMesh Mesh { get; set; }

        //Boleano para ver si dibujamos el boundingbox
        private bool BoundingBox { get; set; }*/

        /// <summary>
        ///     Se llama una sola vez, al principio cuando se ejecuta el ejemplo.
        ///     Escribir aquí todo el código de inicialización: cargar modelos, texturas, estructuras de optimización, todo
        ///     procesamiento que podemos pre calcular para nuestro juego.
        ///     Borrar el codigo ejemplo no utilizado.
        /// </summary>
        public override void Init()
        {
            //Device de DirectX para crear primitivas.
            var d3dDevice = D3DDevice.Instance.Device;
            D3DDevice.Instance.Device.Transform.Projection =
                Matrix.PerspectiveFovLH(D3DDevice.Instance.FieldOfView,
                    D3DDevice.Instance.AspectRatio,
                    D3DDevice.Instance.ZNearPlaneDistance,
                    D3DDevice.Instance.ZFarPlaneDistance * 1.8f);

            //Crear SkyBox
            skyBox = new TgcSkyBox();
            skyBox.Center = new TGCVector3(0, 0, -2300f);
            skyBox.Size = new TGCVector3(10000, 5500, 18000);
            var texturesPath = MediaDir + "XWing\\Textures\\";
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Up, texturesPath + "space.jpg");
              skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Down, texturesPath + "space.jpg");
              skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Left, texturesPath + "space.jpg");
              skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Right, texturesPath + "space.jpg");
              skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Front, texturesPath + "space.jpg");
              skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Back, texturesPath + "space.jpg");
              
            skyBox.Init();

            this.navePrincipal = new NaveEspacial(MediaDir, "xwing-TgcScene.xml");
            this.navePrincipal.ScaleFactor = TGCMatrix.Scaling(0.5f, 0.5f, 0.5f);
            this.navePrincipal.RotationVector = new TGCVector3(0, FastMath.PI_HALF, 0);
            this.navePrincipal.MovementVector = new TGCVector3(1200, -1100f, 4000f);

            // IMPORTANTE: UBICAR LA CARPETA MEDIA EN 2018_1C_3051_Matota\TGC.Group
            SceneEstrellaDeLaMuerte = new TgcSceneLoader().loadSceneFromFile(MediaDir + "XWing/death+star-TgcScene.xml", MediaDir + "XWing/");
            LeftWallEstrellaDeLaMuerte = new TgcSceneLoader().loadSceneFromFile(MediaDir + "XWing/death+star-TgcScene.xml", MediaDir + "XWing/");
            RightWallEstrellaDeLaMuerte = new TgcSceneLoader().loadSceneFromFile(MediaDir + "XWing/m1-TgcScene.xml", MediaDir + "XWing/");

            this.ActionOnScene((mesh) => {
                mesh.AutoTransform = false;
                mesh.Transform = TGCMatrix.Scaling(new TGCVector3(50f, 200f, 80f)) * TGCMatrix.RotationY(FastMath.PI_HALF);
                mesh.setColor(Color.Gray);
                mesh.AutoUpdateBoundingBox = true;
                mesh.updateBoundingBox();
            });
            //Actualiza el bounding box!
            SceneEstrellaDeLaMuerte.BoundingBox.scaleTranslate(SceneEstrellaDeLaMuerte.Meshes[0].Position, new TGCVector3(50f, 200f, 80f));


            this.ActionOnSceneWallLeft((mesh) =>{
                mesh.AutoTransform = false;
                mesh.Transform = TGCMatrix.Scaling(new TGCVector3(50f, 200f, 80f)) * TGCMatrix.RotationY(FastMath.PI_HALF) * TGCMatrix.Translation(new TGCVector3(0,0,-8500f));
                mesh.setColor(Color.Gray);
                //es medio loco el valor que hay que ponerle al translate para que no se solapeen los mesh, no se entiende mucho de donde sale 8500, pero el tema es que
                // el mesh ya de por si tiene un tamaño que no podemos acceder entonces aunque apliquemos la escala no tenemos el valor base como para aplicar esos offsets
            });

            this.ActionOnSceneWallRight((mesh) => {
                mesh.Scale = new TGCVector3(100f, 100f, 100f);
                mesh.RotateZ(FastMath.PI_HALF);
                mesh.Position = new TGCVector3(2000f, 0, 0);
            });
            //La nave tiene mas de un Mesh, si se toma el primero hay parte que no se esta teniendo en cuenta y terminamos teniendo parte de la nave en vez de toda la nave.

            this.navePrincipal.CreateOOB();

            
            //Defino una escala en el modelo logico del mesh que es muy grande.
            /*
            Mesh.UpdateMeshTransform();
            Mesh.Scale = new TGCVector3(0.5f, 0.5f, 0.5f);

            Mesh.RotateY(1.5f);*/

            //Suelen utilizarse objetos que manejan el comportamiento de la camara.
            //Lo que en realidad necesitamos gráficamente es una matriz de View.
            //El framework maneja una cámara estática, pero debe ser inicializada.
            //Posición de la camara.
            var cameraPosition = new TGCVector3(0, 0, 0);
            //Quiero que la camara mire hacia el origen (0,0,0).
            var lookAt = TGCVector3.Empty;
            //Configuro donde esta la posicion de la camara y hacia donde mira.
            Camara.SetCamera(cameraPosition, lookAt);
            //Internamente el framework construye la matriz de view con estos dos vectores.
            //Luego en nuestro juego tendremos que crear una cámara que cambie la matriz de view con variables como movimientos o animaciones de escenas.

            Camara = new CamaraStarWars(this.navePrincipal.GetPosition(), 20, 100);

            disparos = new List<Disparo>();
        }

        /// <summary>
        ///     Se llama en cada frame.
        ///     Se debe escribir toda la lógica de computo del modelo, así como también verificar entradas del usuario y reacciones
        ///     ante ellas.
        /// </summary>
        public override void Update()
        {
            PreUpdate();

            //Coidigo Ejemplo de como capturar teclas
            /*//Capturar Input teclado
            if (base.Input.keyPressed(Key.F))
            {
                BoundingBox = !BoundingBox;
            }*/

            var movimientoNave = TGCVector3.Empty;

            //Movernos de izquierda a derecha, sobre el eje X.
             if (Input.keyDown(Key.Left) || Input.keyDown(Key.A))
                movimientoNave.X = 1;
            else if (Input.keyDown(Key.Right) || Input.keyDown(Key.D))
                movimientoNave.X = -1;

            //Movernos adelante y atras, sobre el eje Z.
            if ((Input.keyDown(Key.Up) || Input.keyDown(Key.W)) && !Input.keyDown(Key.LeftShift))
            {
                if (movimientoZ < movimientoBaseZ)
                {
                    movimientoZ += factorMovimientoZ;
                }
                movimientoNave.Z = movimientoZ;
            }
            else if (Input.keyDown(Key.Down) || Input.keyDown(Key.S))
            {
               /* if(movimientoZ<=0)
                movimientoZ -= movimientoBaseZ;
                else*/
                movimientoNave.Z = -movimientoBaseZ;
            }

            //Movimiento para elevarse con E y Control para bajar , todo sobre el eje Y.
            if (Input.keyDown(Key.E))
                movimientoNave.Y = 1;
            else if (Input.keyDown(Key.LeftControl))
                movimientoNave.Y = -1;

            //boost de velocidad con shift
            if (Input.keyDown(Key.LeftShift) && (Input.keyDown(Key.Up) || Input.keyDown(Key.W)))
            {
                if (movimientoZ > movimientoMaximoZ)
                {
                    movimientoZ -= factorMovimientoZ;
                }

                movimientoNave.Z = movimientoZ;
            }


            //Activar BarrelRoll 
            //TODO: Implementar cooldown?
            if (Input.keyDown(Key.Space))
                this.navePrincipal.DoBarrelRoll();

            if (Input.keyDown(Key.Z))
                this.navePrincipal.DoLeft90Spin();

            if (Input.keyDown(Key.X))
                this.navePrincipal.DoRight90Spin();

            //Disparar
            if (Input.keyDown(Key.F))
            {
                Disparo disparo = disparar(navePrincipal);
                disparos.Add(disparo);
            }

            if (!TgcCollisionUtils.testObbAABB(this.navePrincipal.OOB, SceneEstrellaDeLaMuerte.BoundingBox) && !shouldMove)
            {
                shouldMove = true; //Esto es a modo de ejemplo, tendria que haber un bool por cada scene de estrella y una comprobacion por c/u de ellas tambien, usar abstraccion en una clase y tener una lista de scenes!
                this.navePrincipal.OOB.setRenderColor(Color.Red);
            }
            //una vez movido setear a false para no volver a mover, cuando sea true de nuevo (no va a pasar en este caso)

            if (shouldMove)
                this.ActionOnScene((mesh) => {
                    mesh.Transform = TGCMatrix.Scaling(new TGCVector3(50f, 200f, 80f)) * TGCMatrix.RotationY(FastMath.PI_HALF) * TGCMatrix.Translation(new TGCVector3(0, 0, -17000f));
                    //cada vez que se mueve el mesh entero hay que actualizar su bounding box
                });

            //Actualiza la matrix de movimiento de la nave.
            this.navePrincipal.Move(movimientoNave * ElapsedTime);


            (this.Camara as CamaraStarWars).Target = this.navePrincipal.GetPosition();

            PostUpdate();
        }



        /// <summary>
        ///     Se llama cada vez que hay que refrescar la pantalla.
        ///     Escribir aquí todo el código referido al renderizado.
        ///     Borrar todo lo que no haga falta.
        /// </summary>
        public override void Render()
        {
            //Inicio el render de la escena, para ejemplos simples. Cuando tenemos postprocesado o shaders es mejor realizar las operaciones según nuestra conveniencia.
            PreRender();

            //DrawText.drawText("Rotacion de la nave: " + TGCVector3.PrintVector3(this.SceneNave.Meshes[0].Rotation), 0, 30, Color.White);

            skyBox.Render();

            DrawText.drawText("Posicion de la nave: " + TGCVector3.PrintVector3(this.navePrincipal.Scene.Meshes[0].Position), 0, 30, Color.White);
            DrawText.drawText("Rotacion de la nave: " + TGCVector3.PrintVector3(this.navePrincipal.Scene.Meshes[0].Rotation), 0, 45, Color.White);
            DrawText.drawText("Scale de la nave: " + TGCVector3.PrintVector3(this.navePrincipal.RotationVector), 0, 55, Color.White);
            /*Ejemplo de como renderesar
             * 
             * 
            //Siempre antes de renderizar el modelo necesitamos actualizar la matriz de transformacion.
            //Debemos recordar el orden en cual debemos multiplicar las matrices, en caso de tener modelos jerárquicos, tenemos control total.
            Box.Transform = TGCMatrix.Scaling(Box.Scale) * TGCMatrix.RotationYawPitchRoll(Box.Rotation.Y, Box.Rotation.X, Box.Rotation.Z) * TGCMatrix.Translation(Box.Position);
            //A modo ejemplo realizamos toda las multiplicaciones, pero aquí solo nos hacia falta la traslación.
            //Finalmente invocamos al render de la caja
            Box.Render();*/

            this.SceneEstrellaDeLaMuerte.RenderAll();
            this.LeftWallEstrellaDeLaMuerte.RenderAll();
            //this.RightWallEstrellaDeLaMuerte.RenderAll();
            //Cuando tenemos modelos mesh podemos utilizar un método que hace la matriz de transformación estándar.
            //Es útil cuando tenemos transformaciones simples, pero OJO cuando tenemos transformaciones jerárquicas o complicadas.
            /*Mesh.UpdateMeshTransform();
            //Render del mesh
            Mesh.Render();*/


            this.navePrincipal.TransformMatix = navePrincipal.ScaleFactor *  navePrincipal.RotationMatrix() * navePrincipal.MovementMatrix();

            this.navePrincipal.Render();

            //this.ActionOnNave((mesh) => {
            //    mesh.Transform = mesh.Transform;
            //    mesh.Transform = TGCMatrix.Scaling(0.5f, 0.5f, 0.5f) * TGCMatrix.RotationY(FastMath.PI_HALF);
            //    mesh.Render();
            //});
            //this.SceneNave.RenderAll();

            //Render de BoundingBox, muy útil para debug de colisiones.
            // if (BoundingBox)
            //{
            //Box.BoundingBox.Render();
            //Mesh.BoundingBox.Render();
            //SceneNave.BoundingBox.Render(); // El bounding box del mesh entero es extremadamente grande, y va a detectar colision cuando no la hay.
            SceneEstrellaDeLaMuerte.BoundingBox.Render();
            //LeftWallEstrellaDeLaMuerte.BoundingBox.Render();
            //RightWallEstrellaDeLaMuerte.BoundingBox.Render();



            // }
            disparos.ForEach(disparo => { disparo.modelo.MoveOrientedY(velocidadDisparo); disparo.modelo.Render();});

            //Finaliza el render y presenta en pantalla, al igual que el preRender se debe para casos puntuales es mejor utilizar a mano las operaciones de EndScene y PresentScene
            PostRender();
        }

        /// <summary>
        ///     Se llama cuando termina la ejecución del ejemplo.
        ///     Hacer Dispose() de todos los objetos creados.
        ///     Es muy importante liberar los recursos, sobretodo los gráficos ya que quedan bloqueados en el device de video.
        /// </summary>
        public override void Dispose()
        {
            //Dispose de la caja.
            //Box.Dispose();
            //Dispose del mesh.
            this.navePrincipal.Scene.DisposeAll();
            this.SceneEstrellaDeLaMuerte.DisposeAll();
            this.LeftWallEstrellaDeLaMuerte.DisposeAll();
            this.RightWallEstrellaDeLaMuerte.DisposeAll();
            skyBox.Dispose();
            for (int i = 0; i < disparos.Count; i++)
            {
                if (disparos[i].tiempoDisparo > ElapsedTime)
                {
                    disparos[i].modelo.Dispose();
                    disparos.Remove(disparos[i]);
                }

            }
        }

        private void ActionOnScene(System.Action<TgcMesh> action)
        {
            this.SceneEstrellaDeLaMuerte.Meshes.ForEach(action);
        }
        private void ActionOnSceneWallLeft(System.Action<TgcMesh> action)
        {
            this.LeftWallEstrellaDeLaMuerte.Meshes.ForEach(action);
        }
        private void ActionOnSceneWallRight(System.Action<TgcMesh> action)
        {
            this.RightWallEstrellaDeLaMuerte.Meshes.ForEach(action);
        }
        private Disparo disparar(NaveEspacial nave)
        {
            TGCBox modeloDisparo;
           // var texturaDisparo = TgcTexture.createTexture(MediaDir+"XWing\\Textures\\disparo_laser.jpg");
            var rojoDisparo = Color.Red;
            modeloDisparo= TGCBox.fromSize(new TGCVector3(0.2f, 0.1f, 8f), rojoDisparo);
            modeloDisparo.Position = nave.GetPosition();
            var disparo = new Disparo (modeloDisparo);
            return disparo;
        }
    }
}