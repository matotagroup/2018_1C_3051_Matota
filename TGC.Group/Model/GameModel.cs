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
using System.Diagnostics;
using TGC.Core.Shaders;
using Microsoft.DirectX.Direct3D;
using TGC.Group.Model.UtilsParaGUI;
using TGC.Group.Form;

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



        //Scenes
        private NaveEspacial navePrincipal;
        private List<Escenario> escenarios;

        //private NaveEnemiga nave1;

        private List<NaveEnemiga> enemigos;

        private TgcSkyBox skyBox;


        private float movimientoZ = -2f;

        private float movimientoBaseZ = -2f;

        private float movimientoMaximoZ=-20f;

        private float factorMovimientoZ = 0.25f;

        
        public Torre torreta;
        private TGCBox sol;
        private Menu menu;
        private Drawer2D drawer;


        /// <summary>
        /// Representa el scene donde actualmente esta el jugador.
        /// </summary>
        private Escenario currentScene;

        //Sounds
        private TgcMp3Player sonidoAmbiente;
        private TgcMp3Player sonidoMenu;
        //private TgcMp3Player sonidoLaser;
        
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

            this.escenarios = new List<Escenario>();

            this.enemigos = new List<NaveEnemiga>();

            //Crear SkyBox
            skyBox = new TgcSkyBox();
            skyBox.Center = new TGCVector3(0, 0, -2300f);
            skyBox.Size = new TGCVector3(10000, 10000, 18000);
            var texturesPath = MediaDir + "XWing\\Textures\\";
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Up, texturesPath + "space.jpg");
              skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Down, texturesPath + "space.jpg");
              skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Left, texturesPath + "space.jpg");
              skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Right, texturesPath + "space.jpg");
              skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Front, texturesPath + "space.jpg");
              skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Back, texturesPath + "space.jpg");
              
            skyBox.Init();

            this.navePrincipal = new NaveEspacial(MediaDir, "xwing-TgcScene.xml",10);
            this.navePrincipal.ScaleFactor = TGCMatrix.Scaling(0.5f, 0.5f, 0.5f);
            this.navePrincipal.RotationVector = new TGCVector3(0, FastMath.PI_HALF, 0);
            this.navePrincipal.MovementVector = new TGCVector3(1200f, -1100f, 4000f);

            //this.nave1 = new NaveEnemiga(MediaDir, "X-Wing-TgcScene.xml", new TGCVector3(0,0,-200f),navePrincipal,250f);
            //nave1.ArmaPrincipal.Danio = 1;

            for(int i = 0; i < 5;i++)
                escenarios.Add(Escenario.GenerarEscenarioDefault(MediaDir, i));

            for (int i = 0; i < 3;i++)
            {
                enemigos.Add(new NaveEnemiga(MediaDir, "X-Wing-TgcScene.xml", 5, navePrincipal, 500f));
                enemigos[i].MovementVector = new TGCVector3(0,0,500000f);
                enemigos[i].CreateOOB();

            }


            //enemigos[0].Relocate(new TGCVector3(0,0,-400f));

            //escenarios.ForEach(es => es.generarTorre(MediaDir));
            currentScene = escenarios[0];

            this.navePrincipal.CreateOOB();
            //this.nave1.CreateOOB();
            //Suelen utilizarse objetos que manejan el comportamiento de la camara.
            //Lo que en realidad necesitamos gráficamente es una matriz de View.
            //El framework maneja una cámara estática, pero debe ser inicializada.
            //Posición de la camara.
            var cameraPosition = new TGCVector3(0, 0, 0);
            //Quiero que la camara mire hacia el origen (0,0,0).
            var lookAt = new TGCVector3(-50000, -1, 0);
            //Configuro donde esta la posicion de la camara y hacia donde mira.
            Camara.SetCamera(cameraPosition, lookAt);
            //Internamente el framework construye la matriz de view con estos dos vectores.
            //Luego en nuestro juego tendremos que crear una cámara que cambie la matriz de view con variables como movimientos o animaciones de escenas.

            Camara = new CamaraStarWars(this.navePrincipal.GetPosition(), 20, 100);

            //Cargar el MP3 sonido abiente
            sonidoAmbiente = new TgcMp3Player();
            sonidoAmbiente.FileName = MediaDir + "Music\\StarWarsMusic.mp3";
            sonidoMenu = new TgcMp3Player();
            sonidoMenu.FileName = MediaDir + "Music\\musica_menu.mp3";

            sol = TGCBox.fromSize(new TGCVector3(0,-500,0), new TGCVector3(5, 5, 5), Color.Yellow);
            sol.AutoTransform = true;
            /*if(menu.sonidoAmbiente())
              {
                    sonidoAmbiente.play(true);
              }
             
            if (menu.sonidoMenu)
            {
                sonidoMenu.play(true);
            }*/

            //Sonido laser
            //sonidoLaser = new TgcMp3Player();
            //sonidoLaser.FileName = MediaDir + "Music\\laserSound.mp3";
            menu = new Menu(MediaDir,Input);
            drawer = new Drawer2D();
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

            if (!menu.menuPrincipal) {

                //Movernos de izquierda a derecha, sobre el eje X.
                if (Input.keyDown(Key.Left) || Input.keyDown(Key.A))
                    if (!currentScene.CheckCollision(navePrincipal)) { movimientoNave.X = 1; }
                    else { DrawText.drawText("Tu vida: " + navePrincipal.pierdeVidas(0), 0, 150, Color.White); }


                else if (Input.keyDown(Key.Right) || Input.keyDown(Key.D))
                    if (!currentScene.CheckCollision(navePrincipal)) { movimientoNave.X = -1; }
                    else { DrawText.drawText("Tu vida: " + navePrincipal.pierdeVidas(0), 0, 150, Color.White); }


                //Movimiento para elevarse con E y Control para bajar , todo sobre el eje Y.
                if (Input.keyDown(Key.E))
                    if (!currentScene.CheckCollision(navePrincipal)) { movimientoNave.Y = 1; }
                    else { movimientoNave.Y = -3; }


                else if (Input.keyDown(Key.LeftControl))
                {
                    if (!currentScene.CheckCollision(navePrincipal)) movimientoNave.Y = -1;
                    else { movimientoNave.Y = 3; }
                }

                //boost de velocidad con shift
                if (Input.keyDown(Key.LeftShift))
                {
                    if (movimientoZ > movimientoMaximoZ)
                    {
                        movimientoZ -= factorMovimientoZ * 3;
                    }

                    movimientoNave.Z = movimientoZ;
                }


                if ((Input.keyDown(Key.Up) || Input.keyDown(Key.W)) && !Input.keyDown(Key.LeftShift))
                {
                    if (movimientoZ < movimientoBaseZ)
                    {
                        movimientoZ += factorMovimientoZ;
                    }
                    movimientoNave.Z = movimientoZ;
                }
                //Movernos adelante y atras, sobre el eje Z.

                //if (movimientoZ < movimientoBaseZ)
                //{
                //    movimientoZ += factorMovimientoZ;
                //}
                //movimientoNave.Z = movimientoZ;

                //else if (Input.keyDown(Key.Up) || Input.keyDown(Key.W))
                //{
                //        movimientoNave.Z = movimientoBaseZ;
                //}
                //else if (Input.keyDown(Key.Down) || Input.keyDown(Key.S))
                //{
                //    if (movimientoZ <= 0)
                //        movimientoZ -= movimientoBaseZ;
                //    else
                //        movimientoNave.Z = -movimientoBaseZ;
                //}

                //Activar BarrelRoll 
                //TODO: Implementar cooldown?
                if (Input.keyDown(Key.Space))
                    this.navePrincipal.DoBarrelRoll();

                if (Input.keyDown(Key.Z))
                    this.navePrincipal.DoLeft90Spin();

                if (Input.keyDown(Key.X))
                    this.navePrincipal.DoRight90Spin();

                //Disparar
                //var estadoActual = sonidoLaser.getStatus();
                var estadoSonidoAmbiente = sonidoAmbiente.getStatus();
                if (Input.buttonDown(TgcD3dInput.MouseButtons.BUTTON_LEFT))
                {
                    /*
                     if(estadoActual == TgcMp3Player.States.Open)
                     {
                         sonidoLaser.play(false);  
                     }
                     if(estadoActual == TgcMp3Player.States.Stopped)
                     {
                         sonidoLaser.closeFile(); 
                         sonidoLaser.play(false);
                     }
                     */
                    this.navePrincipal.Disparar( new TGCVector3( ( ( ( D3DDevice.Instance.Width / 2 ) - Input.Xpos ) * 10 ) + navePrincipal.MovementVector.X, navePrincipal.MovementVector.Y, -1 ) );
                }

            }

            navePrincipal.Update(ElapsedTime);

            var torretasEnRango = currentScene.TorresEnRango(navePrincipal.GetPosition());

            torretasEnRango.ForEach(torre => { torre.disparar(navePrincipal.GetPosition()); torre.Update(); });


            if (!TgcCollisionUtils.testObbAABB(this.navePrincipal.OOB, currentScene.Scene.BoundingBox))
            {
                int nextSceneIndex = escenarios.FindIndex(es => es.Equals(currentScene)) + 1;

                if (nextSceneIndex == escenarios.Count)
                    nextSceneIndex = 0;

                currentScene.MoveScene(escenarios.Count);
                currentScene.MovementVector = currentScene.GetOffsetVectorMoved();
                currentScene.UpdateBoundingBox();
                currentScene = escenarios[nextSceneIndex];

                if (enemigos.FindAll(enemigo => enemigo.EstaViva()).Count < 3)
                {
                    enemigos.FindAll(enemigo => !(enemigo.EstaViva()))[0].Relocate(new TGCVector3(0, 0, -1000f));
                }
            }
            //if (enemigos.FindAll(enemigo => enemigo.EstaViva()).Count < 3)
            //{
            //    enemigos.FindAll(enemigo => !enemigo.EstaViva()).ForEach(enemigo => enemigo.Relocate(new TGCVector3(0, 0, -500f)));
            //}

            if (currentScene.CheckCollision(navePrincipal))
                navePrincipal.OOB.setRenderColor(Color.Red);
            else
                navePrincipal.OOB.setRenderColor(Color.Green);

            //TODO: Esto tiene que cambiar, el escenario va a tener su lista de naves y ahi se tiene que manejar la colision!
            enemigos.FindAll(enemigo => enemigo.EstaViva()).ForEach(enemigo =>
              {
                  enemigo.UpdateBoundingBox();
                  if (navePrincipal.CheckIfMyShotsCollided(enemigo))
                  {
                      enemigo.Daniar(navePrincipal.ArmaPrincipal.Danio);
                      
                  }
                  if (enemigo.CheckIfMyShotsCollided(navePrincipal))
                  {
                      navePrincipal.Daniar(enemigo.ArmaPrincipal.Danio);
                  }
              }
            );


            movimientoNave -= TGCVector3.Multiply(currentScene.CheckLimits(navePrincipal, movimientoNave), 7);

            //Actualiza la matrix de movimiento de la nave.

            this.navePrincipal.Move(movimientoNave * ElapsedTime);
            this.navePrincipal.Update();

            enemigos.FindAll(enemigo => enemigo.EstaViva()).ForEach(enemigo =>
            {
                enemigo.Perseguir(ElapsedTime);
                enemigo.Update();
            }
            );

            if (Input.keyDown(Key.H))
                enemigos[0].Move(new TGCVector3(-0.1f,0,-0.1f));

            this.skyBox.Center += movimientoNave * ElapsedTime * 1000;

            //this.skyBox.Center += new TGCVector3(0, 0, movimientoNave.Z) * ElapsedTime * 1000;
            //this.sol.Move(new TGCVector3(0, 0, movimientoNave.Z) * ElapsedTime * 1000);


            (this.Camara as CamaraStarWars).Target = this.navePrincipal.GetPosition();

            menu.Update(ElapsedTime);
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

            skyBox.Render();
            sol.Render();

            //Seteo el effect del mesh de la nave.
            navePrincipal.ActionOnNave(m =>
            {
                m.Effect = TgcShaders.Instance.TgcMeshPhongShader;
                m.Effect.SetValue("lightPosition", TGCVector3.Vector3ToFloat4Array(sol.Position));
                m.Effect.SetValue("eyePosition", TGCVector3.Vector3ToFloat4Array(TGCVector3.One));
                m.Effect.SetValue("ambientColor", ColorValue.FromColor(Color.FromArgb(255, 150, 150, 150)));


                m.Effect.SetValue("diffuseColor", ColorValue.FromColor(Color.FromArgb(255, 99,72,7)));
                m.Effect.SetValue("specularColor", ColorValue.FromColor(Color.FromArgb(255, 255, 255, 255)));
                //m.Effect.SetValue("specularColor", ColorValue.FromColor(Color.Black));

                m.Effect.SetValue("specularExp", 50f);
            });

            escenarios.ForEach(e => {
                   e.ForEachMesh(m =>
                   {
                       m.Effect = TgcShaders.Instance.TgcMeshPhongShader;
                       m.Effect.SetValue("lightPosition", TGCVector3.Vector3ToFloat4Array(sol.Position));
                       m.Effect.SetValue("eyePosition", TGCVector3.Vector3ToFloat4Array(TGCVector3.One));
                       m.Effect.SetValue("ambientColor", ColorValue.FromColor(Color.FromArgb(255, 150, 150, 150)));

                       m.Effect.SetValue("diffuseColor", ColorValue.FromColor(Color.FromArgb(255, 99,72,7)));
                       m.Effect.SetValue("specularColor", ColorValue.FromColor(Color.FromArgb(255, 255, 255, 255)));
                       //m.Effect.SetValue("specularColor", ColorValue.FromColor(Color.Black));

                       m.Effect.SetValue("specularExp", 50f);
                   });
            });


            DrawText.drawText("Posicion de la nave: " + TGCVector3.PrintVector3(this.navePrincipal.Scene.Meshes[0].Position), 0, 30, Color.White);
            DrawText.drawText("Rotacion de la nave: " + TGCVector3.PrintVector3(this.navePrincipal.Scene.Meshes[0].Rotation), 0, 45, Color.White);
            DrawText.drawText("Scale de la nave: " + TGCVector3.PrintVector3(this.navePrincipal.RotationVector), 0, 55, Color.White);
            DrawText.drawText("Scale de la nave: " + TGCVector3.PrintVector3(this.navePrincipal.MovementVector), 0, 85, Color.White);
            DrawText.drawText("Scale de la nave: " + TGCVector3.PrintVector3(this.currentScene.Scene.BoundingBox.PMin), 0, 105, Color.White);
            DrawText.drawText("Scale de la nave: " + TGCVector3.PrintVector3(this.currentScene.Scene.BoundingBox.PMax), 0, 115, Color.White);

            DrawText.drawText("Tu vida: " + navePrincipal.Vida, 0, 150, Color.White);
            DrawText.drawText("Vida del enemigo 1: " + enemigos[0].Vida, 0, 190, Color.White);
            DrawText.drawText("Vida del enemigo 2: " + enemigos[1].Vida, 0, 230, Color.White);
            DrawText.drawText("Vida del enemigo 3: " + enemigos[2].Vida, 0, 270, Color.White);


            DrawText.drawText("Tu vida: " + navePrincipal.Vida, 0, 150, Color.White);
            
            this.navePrincipal.TransformMatix = navePrincipal.ScaleFactor *  navePrincipal.RotationMatrix() * navePrincipal.MovementMatrix();

            this.escenarios.ForEach((es) => {
                es.TransformMatix = es.ScaleFactor * es.RotationMatrix() * es.MovementMatrix();
                es.Render();
            });

         
            this.navePrincipal.Render();

            enemigos.FindAll(enemigo => enemigo.EstaViva()).ForEach(enemigo =>
            {
                enemigo.TransformMatix = enemigo.ScaleFactor * enemigo.RotationMatrix() * enemigo.MovementMatrix();
                enemigo.Render();
            }
            );

            menu.Render(ElapsedTime,drawer);
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
            this.navePrincipal.Scene.DisposeAll();
            this.enemigos.ForEach(enemigo => enemigo.Scene.DisposeAll());
            this.escenarios.ForEach(es => { es.Dispose(); });
            skyBox.Dispose();
            sonidoAmbiente.closeFile();
            menu.Dispose();
            //sonidoLaser.closeFile();
        }
    }
}