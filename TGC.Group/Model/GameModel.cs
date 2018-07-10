using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX.DirectInput;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TGC.Core.Collision;
using TGC.Core.Direct3D;
using TGC.Core.Example;
using TGC.Core.Geometry;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.Shaders;
using TGC.Core.Sound;
using TGC.Core.Terrain;
using TGC.Group.Form;
using TGC.Group.Model.UtilsParaGUI;

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

        private Texture escena, propulsores, propulsoresBlurAux, propulsoresBlurAux2;
        private VertexBuffer screenQuadVB;
        private Microsoft.DirectX.Direct3D.Effect postProcessMerge, blurEffect;

        //define la cantidad de pasadas que se van al gaussian blur, mientras mas pasadas mas blur pero ojo porque consume mas.
        private int cant_pasadas = 2;

        //Scenes
        private NaveEspacial navePrincipal;
        private List<Escenario> escenarios;
        private List<NaveEnemiga> enemigos;

        private TgcSkyBox skyBox;

        //Movimiento
        private float movimientoZ = -2f;
        private float movimientoBaseZ = -2f;
        private float movimientoMaximoZ = -7.5f;
        private float factorMovimientoZ = 0.25f;

        public Torre torreta;
        private TGCBox sol;
        private Menu menu;
        private Drawer2D drawer;
        private Hud hud;
        private Surface depthStencil; // Depth-stencil buffer
        private Surface depthStencilOld;

        /// <summary>
        /// Representa el scene donde actualmente esta el jugador.
        /// </summary>
        private Escenario currentScene;

        //Sounds
        private TgcMp3Player playerAmbiente = new TgcMp3Player();
        private string pathSonidoAmbiente;
        private string pathSonidoMenu;
        private string pathSonidoDisparo;

        private int enemigosAlMismoTiempo = 3;
        private int dañoEnemigos = 5;

        private List<TGCBox> estrellasS = new List<TGCBox>
        {
            TGCBox.fromSize(new TGCVector3(-5000, 1000, -13000), new TGCVector3(15,15,15), Color.White),
            TGCBox.fromSize(new TGCVector3(500, 2000, -13000), new TGCVector3(15,15,15), Color.White),
            TGCBox.fromSize(new TGCVector3(5000, 2500, -10000), new TGCVector3(15,15,15), Color.White),
            TGCBox.fromSize(new TGCVector3(3000, 1000, -13000), new TGCVector3(15,15,15), Color.White),
            TGCBox.fromSize(new TGCVector3(-3000, 2000, -11000), new TGCVector3(15,15,15), Color.White),
            TGCBox.fromSize(new TGCVector3(-8000, 1500, -13000), new TGCVector3(15,15,15), Color.White),
            TGCBox.fromSize(new TGCVector3(-4500, 800, -9000), new TGCVector3(15,15,15), Color.White),
            TGCBox.fromSize(new TGCVector3(-9000, 300, -13000), new TGCVector3(15,15,15), Color.White),
            TGCBox.fromSize(new TGCVector3(8000, 1500, -13000), new TGCVector3(15,15,15), Color.White),
            TGCBox.fromSize(new TGCVector3(4500, 800, -9000), new TGCVector3(15,15,15), Color.White),
            TGCBox.fromSize(new TGCVector3(9000, 300, -13000), new TGCVector3(15,15,15), Color.White),
            TGCBox.fromSize(new TGCVector3(11500, 1500, -14000), new TGCVector3(15,15,15), Color.White),
            TGCBox.fromSize(new TGCVector3(10000, 100, -10000), new TGCVector3(15,15,15), Color.White),
            TGCBox.fromSize(new TGCVector3(9000, 100, -11100), new TGCVector3(15,15,15), Color.White),
            TGCBox.fromSize(new TGCVector3(8500, 2000, -9000), new TGCVector3(15,15,15), Color.White),
            TGCBox.fromSize(new TGCVector3(-9000, 300, -13000), new TGCVector3(15,15,15), Color.White),
            TGCBox.fromSize(new TGCVector3(-9000, 900, -13000), new TGCVector3(15,15,15), Color.White),
            TGCBox.fromSize(new TGCVector3(0, 300, -13000), new TGCVector3(15,15,15), Color.White),
            TGCBox.fromSize(new TGCVector3(-1000, 2100, -13000), new TGCVector3(15,15,15), Color.White),
            TGCBox.fromSize(new TGCVector3(1000, 3000, -13000), new TGCVector3(15,15,15), Color.White),
            TGCBox.fromSize(new TGCVector3(-2000, 300, -13000), new TGCVector3(15,15,15), Color.White)
        };

        // Shadow map
        //private TGCVertex3fModifier lightLookFromModifier;
        //private TGCVertex3fModifier lightLookAtModifier;

        private readonly float far_plane = 50000f;
        private readonly float near_plane = 2f;


        private readonly int SHADOWMAP_SIZE = 1024;
        private TGCVector3 g_LightDir; // direccion de la luz actual
        private TGCVector3 g_LightPos; // posicion de la luz actual (la que estoy analizando)
        private TGCMatrix g_LightView; // matriz de view del light
        private TGCMatrix g_mShadowProj; // Projection matrix for shadow map
        private Surface g_pDSShadow; // Depth-stencil buffer for rendering to shadow map

        private Texture g_pShadowMap; // Texture to which the shadow map is rendered

        private bool dibujarGlow = true, dibujarShadowMap = true, dibujarLuces = true, spawnaearEnemigos = true, habilitarDisparosTorres = true;



        /// <summary>
        ///     Se llama una sola vez, al principio cuando se ejecuta el ejemplo.
        ///     Escribir aquí todo el código de inicialización: cargar modelos, texturas, estructuras de optimización, todo
        ///     procesamiento que podemos pre calcular para nuestro juego.
        ///     Borrar el codigo ejemplo no utilizado.
        /// </summary>
        public override void Init()
        {
            estrellasS.ForEach(e => e.AutoTransform = true);
            CustomVertex.PositionTextured[] screenQuadVertices =
            {
                new CustomVertex.PositionTextured(-1, 1, 1, 0, 0),
                new CustomVertex.PositionTextured(1, 1, 1, 1, 0),
                new CustomVertex.PositionTextured(-1, -1, 1, 0, 1),
                new CustomVertex.PositionTextured(1, -1, 1, 1, 1)
            };
            //vertex buffer de los triangulos
            screenQuadVB = new VertexBuffer(typeof(CustomVertex.PositionTextured), 4, D3DDevice.Instance.Device, Usage.Dynamic | Usage.WriteOnly,
                CustomVertex.PositionTextured.Format, Pool.Default);
            screenQuadVB.SetData(screenQuadVertices, 0, LockFlags.None);

            //Creamos un DepthStencil que debe ser compatible con nuestra definicion de renderTarget2D.
            depthStencil = D3DDevice.Instance.Device.CreateDepthStencilSurface(D3DDevice.Instance.Device.PresentationParameters.BackBufferWidth,
                    D3DDevice.Instance.Device.PresentationParameters.BackBufferHeight, DepthFormat.D24S8, MultiSampleType.None, 0, true);

            depthStencilOld = D3DDevice.Instance.Device.DepthStencilSurface;


            escena = new Texture(D3DDevice.Instance.Device, D3DDevice.Instance.Device.PresentationParameters.BackBufferWidth,
                D3DDevice.Instance.Device.PresentationParameters.BackBufferHeight, 1, Usage.RenderTarget, Format.X8R8G8B8, Pool.Default);

            propulsores = new Texture(D3DDevice.Instance.Device, D3DDevice.Instance.Device.PresentationParameters.BackBufferWidth,
                D3DDevice.Instance.Device.PresentationParameters.BackBufferHeight, 1, Usage.RenderTarget, Format.X8R8G8B8, Pool.Default);

            propulsoresBlurAux = new Texture(D3DDevice.Instance.Device, D3DDevice.Instance.Device.PresentationParameters.BackBufferWidth,
                D3DDevice.Instance.Device.PresentationParameters.BackBufferHeight, 1, Usage.RenderTarget, Format.X8R8G8B8, Pool.Default);

            propulsoresBlurAux2 = new Texture(D3DDevice.Instance.Device, D3DDevice.Instance.Device.PresentationParameters.BackBufferWidth,
                D3DDevice.Instance.Device.PresentationParameters.BackBufferHeight, 1, Usage.RenderTarget, Format.X8R8G8B8, Pool.Default);

            //Device de DirectX para crear primitivas.
            var d3dDevice = D3DDevice.Instance.Device;
            D3DDevice.Instance.Device.Transform.Projection =
                Matrix.PerspectiveFovLH(D3DDevice.Instance.FieldOfView,
                    D3DDevice.Instance.AspectRatio,
                    D3DDevice.Instance.ZNearPlaneDistance,
                    D3DDevice.Instance.ZFarPlaneDistance * 1.8f);

            this.postProcessMerge = TgcShaders.loadEffect(this.ShadersDir + "PostProcess.fx");
            this.blurEffect = TgcShaders.loadEffect(this.ShadersDir + "GaussianBlur.fx");

            blurEffect.SetValue("screen_dx", d3dDevice.PresentationParameters.BackBufferWidth);
            blurEffect.SetValue("screen_dy", d3dDevice.PresentationParameters.BackBufferHeight);

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

            this.navePrincipal = new NaveEspacial(MediaDir, "xwing-TgcScene.xml", Color.DarkBlue, 10, 250, null, 250f);
            this.navePrincipal.ScaleFactor = TGCMatrix.Scaling(0.5f, 0.5f, 0.5f);
            this.navePrincipal.RotationVector = new TGCVector3(0, FastMath.PI_HALF, 0);
            this.navePrincipal.MovementVector = new TGCVector3(1200f, -1100f, 4000f);


            for (int i = 0; i < 5; i++)
                escenarios.Add(Escenario.GenerarEscenarioDefault(MediaDir, i));

            for (int i = 0; i < enemigosAlMismoTiempo; i++)
            {
                enemigos.Add(new NaveEnemiga(MediaDir, "X-Wing-TgcScene.xml", dañoEnemigos, 500, navePrincipal));
                enemigos[i].MovementVector = new TGCVector3(0, 0, 500000000000f);
                enemigos[i].CreateOOB();

            }


            //escenarios.ForEach(es => es.generarTorre(MediaDir));
            currentScene = escenarios[0];

            this.navePrincipal.CreateOOB();
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

            sol = TGCBox.fromSize(new TGCVector3(0, 5000, 4000), new TGCVector3(50, 50, 50), Color.Yellow);
            sol.AutoTransform = true;
            menu = new Menu(MediaDir, Input);

            //Cargo sonidos
            pathSonidoMenu = MediaDir + "Sound\\musica_menu.mp3";
            pathSonidoAmbiente = MediaDir + "Music\\StarWarsMusic.mp3";
            pathSonidoDisparo = MediaDir + "Music\\laserSound.wav";

            if (menu.playSonidoMenu)
            {
                playerAmbiente.closeFile();
                playerAmbiente.FileName = pathSonidoMenu;
                playerAmbiente.play(true);
            }


            drawer = new Drawer2D();
            hud = new Hud(MediaDir, Input);

            //ShadowMap

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
            g_mShadowProj = TGCMatrix.PerspectiveFovLH(Geometry.DegreeToRadian(90), aspectRatio, 50, 15000);
            D3DDevice.Instance.Device.Transform.Projection = TGCMatrix.PerspectiveFovLH(Geometry.DegreeToRadian(45.0f), aspectRatio, near_plane, far_plane).ToMatrix();
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
            if (Input.keyPressed(Key.G))
            {
                if (dibujarGlow)
                    dibujarGlow = false;
                else
                    dibujarGlow = true;
            }

            if (Input.keyPressed(Key.M))
            {
                if (dibujarShadowMap)
                    dibujarShadowMap = false;
                else
                    dibujarShadowMap = true;
            }

            if (Input.keyPressed(Key.L))
            {
                if (dibujarLuces)
                    dibujarLuces = false;
                else
                    dibujarLuces = true;
            }


            if (Input.keyPressed(Key.E))
            {
                if (spawnaearEnemigos)
                    spawnaearEnemigos = false;
                else
                    spawnaearEnemigos = true;
            }

            if (Input.keyPressed(Key.T))
            {
                if (habilitarDisparosTorres)
                    habilitarDisparosTorres = false;
                else
                    habilitarDisparosTorres = true;
            }



            if (menu.estaEnMenu)
            {
                menu.Update(ElapsedTime);
                if (playerAmbiente.FileName != pathSonidoMenu)
                {
                    playerAmbiente.closeFile();
                    playerAmbiente.FileName = pathSonidoMenu;
                    playerAmbiente.play(true);
                }
                PostUpdate();
                return;
            }
            else
            {
                hud.Update(navePrincipal);
            }

            var movimientoNave = TGCVector3.Empty;

            if (!menu.estaEnMenu)
            {
                if (playerAmbiente.FileName != pathSonidoAmbiente)
                {
                    playerAmbiente.closeFile();
                    playerAmbiente.FileName = pathSonidoAmbiente;
                    playerAmbiente.play(true);
                }
                //Movernos de izquierda a derecha, sobre el eje X.
                if (Input.keyDown(Key.Left) || Input.keyDown(Key.A))
                    movimientoNave.X = 1;
                else if (Input.keyDown(Key.Right) || Input.keyDown(Key.D))
                    movimientoNave.X = -1;

                //Movimiento para elevarse con E y Control para bajar , todo sobre el eje Y.
                if (Input.keyDown(Key.W) || Input.keyDown(Key.UpArrow))
                    movimientoNave.Y = 1;
                else if (Input.keyDown(Key.S) || Input.keyDown(Key.DownArrow))
                    movimientoNave.Y = -1;

                //boost de velocidad con shift
                if (Input.keyDown(Key.LeftShift))
                {
                    navePrincipal.DoSpeedBoost();
                    if (navePrincipal.shouldSpeedBoost)
                    {
                        navePrincipal.GastarFuel(1.5f, hud);
                        if (movimientoZ > movimientoMaximoZ)
                            movimientoZ -= factorMovimientoZ * 3;
                        movimientoNave.Z = movimientoZ;
                        cant_pasadas = 3;
                    }
                    else
                    {
                        movimientoNave.Z = movimientoBaseZ;
                    }
                }
                else
                    cant_pasadas = 2;



                if (movimientoZ < movimientoBaseZ)
                {
                    movimientoZ += factorMovimientoZ;
                }
                movimientoNave.Z = movimientoZ;

                //Activar rotaciones especiales
                if (Input.keyDown(Key.Space))
                    this.navePrincipal.DoBarrelRoll();

                if (Input.keyDown(Key.Z))
                    this.navePrincipal.DoLeft90Spin();

                if (Input.keyDown(Key.X))
                    this.navePrincipal.DoRight90Spin();

                //Disparar
                if (Input.buttonPressed(TgcD3dInput.MouseButtons.BUTTON_LEFT))
                {
                    if (navePrincipal.Disparar(new TGCVector3((((D3DDevice.Instance.Width / 2) - Input.Xpos) * 10) + navePrincipal.MovementVector.X, navePrincipal.MovementVector.Y, navePrincipal.MovementVector.Z - 5000), pathSonidoDisparo, DirectSound.DsDevice))
                    {
                        //Aca va la posicion del disparo creo

                    }
                }
                if (Input.keyDown(Key.F))
                {
                    if (navePrincipal.Disparar(pathSonidoDisparo, DirectSound.DsDevice))
                    {
                    }
                }
                if (habilitarDisparosTorres)
                {
                    var torretasEnRango = currentScene.TorresEnRango(navePrincipal.GetPosition());
                    torretasEnRango.ForEach(torre => { torre.Disparar(navePrincipal.GetPosition(), pathSonidoDisparo, DirectSound.DsDevice); torre.Update(ElapsedTime); });
                }
            }
            NaveEnemiga.resetearPosiciones();

            if (!TgcCollisionUtils.testObbAABB(this.navePrincipal.OOB, currentScene.Scene.BoundingBox))
            {
                int nextSceneIndex = escenarios.FindIndex(es => es.Equals(currentScene)) + 1;

                if (nextSceneIndex == escenarios.Count)
                    nextSceneIndex = 0;

                currentScene.MoveScene(escenarios.Count);
                currentScene.MovementVector = currentScene.GetOffsetVectorMoved();
                currentScene.UpdateBoundingBox();
                currentScene = escenarios[nextSceneIndex];

                NaveEnemiga.resetearPosiciones();

                // enemigosAlMismoTiempo pueden modificarse para aumentar o disminuir la dificultad, tambien para el modo god
                if (spawnaearEnemigos)
                {
                    if (enemigos.FindAll(enemigo => enemigo.EstaViva() && enemigo.EnemigoEstaAdelante()).Count == 0)
                    {
                        enemigos.FindAll(enemigo => !enemigo.EstaViva() || !enemigo.EnemigoEstaAdelante()).ForEach(nave => nave.Relocate());

                    }
                }
            }

            // No permitir que se salga de los limites, el salto que hace para volver es medio brusco, se podria atenuar.
            movimientoNave -= TGCVector3.Multiply(currentScene.CheckLimits(navePrincipal, movimientoNave), 10);

            //Actualiza la matrix de movimiento de la nave.

            this.navePrincipal.Move(movimientoNave * ElapsedTime);
            this.navePrincipal.Update(ElapsedTime);
            if (spawnaearEnemigos)
            {
                enemigos.FindAll(enemigo => enemigo.EstaViva() && enemigo.EnemigoEstaAdelante()).ForEach(enemigo =>
                {
                    enemigo.Perseguir(ElapsedTime, pathSonidoDisparo, DirectSound.DsDevice);
                    enemigo.Update(ElapsedTime);
                }

            );
            }
            var naves = enemigos.FindAll(enemigo => enemigo.EstaViva() && enemigo.EnemigoEstaAdelante()&&spawnaearEnemigos).Select(e => (NaveEspacial)e).ToList();
            naves.Add(navePrincipal);
            naves.ForEach(naveActual =>
            {

                //Colision de todas las naves contra el escenario.
                if (currentScene.CheckCollision(naveActual))
                    naveActual.Morir();

                naves.FindAll(n => n != naveActual).ForEach(otraNave =>
                {

                    //Colision fisica entre naves.
                    if (TgcCollisionUtils.testObbObb(naveActual.OOB, otraNave.OOB))
                    {
                        naveActual.Morir();
                        otraNave.Morir();
                    }

                    //Colision de disparos
                    if (naveActual.CheckIfMyShotsCollided(otraNave))
                        otraNave.Daniar(naveActual.ArmaPrincipal.Danio);
                });
                currentScene.TorresEnRango(navePrincipal.GetPosition()).ForEach(torre =>
                {
                    if (torre.CheckIfMyShotsCollided(naveActual))
                    {
                        naveActual.Daniar(torre.arma.Danio);
                    }
                });

            });

            if (!navePrincipal.EstaViva())
            {
                this.navePrincipal.MoveTo(new TGCVector3(1200f, -1100f, 4000f) + currentScene.GetOffsetVectorMoved());
                this.skyBox.Center = new TGCVector3(0, 0, -2300f) + currentScene.GetOffsetVectorMoved();
                this.navePrincipal.Revivir();
                this.sol.Position = new TGCVector3(0, 5000f, 4000f) + currentScene.GetOffsetVectorMoved();
                this.menu.estaEnMenu = true;
            }


            this.skyBox.Center += movimientoNave * ElapsedTime * 1000;
            this.sol.Move(new TGCVector3(0, 0, movimientoNave.Z) * ElapsedTime * 1000);
            estrellasS.ForEach(e =>
            {
                e.Position += new TGCVector3(0, 0, movimientoNave.Z) * ElapsedTime * 1000;
            });

            (this.Camara as CamaraStarWars).Target = this.navePrincipal.GetPosition();

            PostUpdate();
        }

        public void RenderScene(Microsoft.DirectX.Direct3D.Device d3dDevice)
        {

            d3dDevice.BeginScene();
            //skyBox.Render();

            estrellasS.ForEach(e => e.Render());

            // En esta pasada le aplicamos el efecto de luz metalica. con los enemigos no hace falta esto porque nunca cambian la technique.
            if (dibujarShadowMap)
            {
                navePrincipal.ActionOnNave(nave => nave.Technique = "DIFFUSE_MAP_PHONG_CON_SHADOW");
                enemigos.ForEach(e => e.ActionOnNave(m => m.Technique = "DIFFUSE_MAP_PHONG_CON_SHADOW"));
                escenarios.ForEach(es => es.Scene.Meshes.ForEach(m => m.Technique = "DIFFUSE_MAP_PHONG_CON_SHADOW"));
                escenarios.ForEach(es => es.torres.ForEach(t => t.Scene.Meshes.ForEach(m => m.Technique = "DIFFUSE_MAP_PHONG_CON_SHADOW")));
            }
            else if(dibujarLuces)
            {
                navePrincipal.ActionOnNave(nave => nave.Technique = "DIFFUSE_MAP_PHONG");
                enemigos.ForEach(e => e.ActionOnNave(m => m.Technique = "DIFFUSE_MAP_PHONG"));
                escenarios.ForEach(es => es.Scene.Meshes.ForEach(m => m.Technique = "DIFFUSE_MAP_PHONG"));
                escenarios.ForEach(es => es.torres.ForEach(t => t.Scene.Meshes.ForEach(m => m.Technique = "DIFFUSE_MAP_PHONG")));
            }
            else
            {
                navePrincipal.ActionOnNave(nave => nave.Technique = "NO_EFFECTS");
                enemigos.ForEach(e => e.ActionOnNave(m => m.Technique = "NO_EFFECTS"));
                escenarios.ForEach(es => es.Scene.Meshes.ForEach(m => m.Technique = "NO_EFFECTS"));
                escenarios.ForEach(es => es.torres.ForEach(t => t.Scene.Meshes.ForEach(m => m.Technique = "NO_EFFECTS")));
            }
            this.navePrincipal.TransformMatix = navePrincipal.ScaleFactor * navePrincipal.RotationMatrix() * navePrincipal.MovementMatrix();

            navePrincipal.Render(sol.Position, Camara.Position);

            DirectSound.Listener3d.Position = navePrincipal.GetPosition();

            this.escenarios.ForEach((es) =>
            {
                es.TransformMatix = es.ScaleFactor * es.RotationMatrix() * es.MovementMatrix();
                es.Render(sol.Position, Camara.Position, ElapsedTime);
            });
            if (spawnaearEnemigos)
            {
                enemigos.FindAll(enemigo => enemigo.EstaViva() && enemigo.EnemigoEstaAdelante()).ForEach(enemigo =>
                {
                    enemigo.TransformMatix = enemigo.ScaleFactor * enemigo.RotationMatrix() * enemigo.MovementMatrix();
                    enemigo.Render(sol.Position, Camara.Position);
                }
                );
            }
            if (!dibujarGlow)
            {
                navePrincipal.RenderDisparos();
                if (spawnaearEnemigos)
                {
                    enemigos.ForEach(enemigo => enemigo.RenderDisparos());

                    escenarios.ForEach(escenario => escenario.torres.ForEach(torre => torre.RenderDisparos()));
                }
            }
            if (menu.estaEnMenu)
            {
                menu.Render(ElapsedTime, drawer);
            }
            else
            {
                hud.Render(ElapsedTime, drawer);
            }


            d3dDevice.EndScene();
        }

        //SOLO PARA TESTING!!!!!!!!!!!!!! SI SE PONE EN TRUE SE GUARDAN LAS TEXTURAS QUE SE VAN GENERANDO EN EL MEDIA DIR.
        bool save = false;
        private TGCVector3 desfase = new TGCVector3(-10f, 75f, 275f);

        /// <summary>
        ///     Se llama cada vez que hay que refrescar la pantalla.
        ///     Escribir aquí todo el código referido al ren|rendederizado.
        ///     Borrar todo lo que no haga falta.
        /// </summary>
        public override void Render()
        {
            //Inicio el render de la escena, para ejemplos simples. Cuando tenemos postprocesado o shaders es mejor realizar las operaciones según nuestra conveniencia.

            ClearTextures();

            //Cargamos el Render Targer al cual se va a dibujar la escena 3D. Antes nos guardamos el surface original
            //En vez de dibujar a la pantalla, dibujamos a un buffer auxiliar, nuestro Render Target.


            g_LightPos = navePrincipal.GetPosition() + desfase;
            g_LightDir = navePrincipal.GetPosition() - g_LightPos;
            g_LightDir.Normalize();

            D3DDevice.Instance.Device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);

            //Genero el shadow map
            if (dibujarShadowMap)
            {
                RenderShadowMap();
            }

            // dibujo la escena pp dicha
            //D3DDevice.Instance.Device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);

            var d3dDevice = D3DDevice.Instance.Device;

            var superficieVieja = d3dDevice.GetRenderTarget(0);
            var superficieEscena = this.escena.GetSurfaceLevel(0);

            depthStencilOld = d3dDevice.DepthStencilSurface;
            d3dDevice.DepthStencilSurface = depthStencil;

            d3dDevice.SetRenderTarget(0, superficieEscena);
            // Probar de comentar esta linea, para ver como se produce el fallo en el ztest
            // por no soportar usualmente el multisampling en el render to texture (en nuevas placas de video)
            d3dDevice.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);

            RenderScene(d3dDevice);

            superficieEscena.Dispose();

            if (save)
            {
                TextureLoader.Save(this.MediaDir + "escena.bmp", ImageFileFormat.Bmp, this.escena);
            }


            var superficieGlow = this.propulsores.GetSurfaceLevel(0);
            d3dDevice.SetRenderTarget(0, superficieGlow);

            d3dDevice.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);

            d3dDevice.BeginScene();

            if (dibujarGlow)
            {
                navePrincipal.ActionOnNave(nave => nave.Technique = "CortePropulsores");

                navePrincipal.RenderDisparos();
                if (spawnaearEnemigos)
                {
                    enemigos.ForEach(enemigo => enemigo.RenderDisparos());
                }
                escenarios.ForEach(escenario => escenario.torres.ForEach(torre => torre.RenderDisparos()));

                navePrincipal.Render(TGCVector3.Empty, TGCVector3.Empty);
            }

            d3dDevice.EndScene();

            superficieGlow.Dispose();

            if (save)
            {
                TextureLoader.Save(this.MediaDir + "corte.bmp", ImageFileFormat.Bmp, this.propulsores);
            }

            if (cant_pasadas > 0)
            {
                /*
                  Si se activa el siguiente efecto mata bastante a la gpu, la realidad es que no agrega mucho porque 
                  degrada el color de la textura un poco y considerando que solo son 4 propulsores no lo vale,
                  asi que se puede dejar apagado para que no consuma. si se enciende hacer que
                  la textura de corte se grabe en propulsores en vez de propulsoresbluraux.
                  */

                superficieGlow = propulsoresBlurAux.GetSurfaceLevel(0);
                d3dDevice.SetRenderTarget(0, superficieGlow);

                d3dDevice.BeginScene();
                blurEffect.Technique = "DownFilter4";
                d3dDevice.VertexFormat = CustomVertex.PositionTextured.Format;
                d3dDevice.SetStreamSource(0, screenQuadVB, 0);
                blurEffect.SetValue("g_RenderTarget", propulsores);

                d3dDevice.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);
                blurEffect.Begin(FX.None);
                blurEffect.BeginPass(0);
                d3dDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
                blurEffect.EndPass();
                blurEffect.End();
                superficieGlow.Dispose();
                if (save)
                {
                    TextureLoader.Save(this.MediaDir + "downfilter.bmp", ImageFileFormat.Bmp, propulsoresBlurAux);
                }
                d3dDevice.EndScene();

                d3dDevice.DepthStencilSurface = depthStencilOld;


                for (var P = 0; P < cant_pasadas; ++P)
                {
                    //save = true;
                    // Gaussian blur Horizontal
                    // -----------------------------------------------------
                    //pSurf = g_pRenderTarget4Aux.GetSurfaceLevel(0);
                    //device.SetRenderTarget(0, pSurf);
                    // dibujo el quad pp dicho :

                    superficieGlow = propulsoresBlurAux2.GetSurfaceLevel(0);
                    d3dDevice.SetRenderTarget(0, superficieGlow);

                    d3dDevice.BeginScene();
                    blurEffect.Technique = "GaussianBlurSeparable";
                    d3dDevice.VertexFormat = CustomVertex.PositionTextured.Format;
                    d3dDevice.SetStreamSource(0, screenQuadVB, 0);
                    blurEffect.SetValue("g_RenderTarget", propulsoresBlurAux);

                    d3dDevice.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);
                    blurEffect.Begin(FX.None);
                    blurEffect.BeginPass(0);
                    d3dDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
                    blurEffect.EndPass();
                    blurEffect.End();
                    superficieGlow.Dispose();
                    if (save)
                    {
                        TextureLoader.Save(this.MediaDir + "rt_h_" + P + ".bmp", ImageFileFormat.Bmp, propulsoresBlurAux);
                    }
                    d3dDevice.EndScene();

                    //pSurf = g_pRenderTarget4.GetSurfaceLevel(0);
                    d3dDevice.SetRenderTarget(0, superficieGlow);
                    superficieGlow.Dispose();

                    //  Gaussian blur Vertical
                    // -----------------------------------------------------
                    //save = true;
                    superficieGlow = propulsoresBlurAux.GetSurfaceLevel(0);
                    d3dDevice.SetRenderTarget(0, superficieGlow);

                    d3dDevice.BeginScene();
                    blurEffect.Technique = "GaussianBlurSeparable";
                    d3dDevice.VertexFormat = CustomVertex.PositionTextured.Format;
                    d3dDevice.SetStreamSource(0, screenQuadVB, 0);
                    blurEffect.SetValue("g_RenderTarget", propulsoresBlurAux2);

                    d3dDevice.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);
                    blurEffect.Begin(FX.None);
                    blurEffect.BeginPass(1);
                    d3dDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
                    blurEffect.EndPass();
                    blurEffect.End();
                    if (save)
                    {
                        TextureLoader.Save(this.MediaDir + "rt_v_" + P + ".bmp", ImageFileFormat.Bmp, propulsoresBlurAux2);
                        //save = false;
                    }
                    d3dDevice.EndScene();
                }
            }
            //Si quisieramos ver que se dibujo, podemos guardar el resultado a una textura en un archivo para debugear su resultado (ojo, es lento)
            //TextureLoader.Save(this.ShadersDir + "render_target.bmp", ImageFileFormat.Bmp, renderTarget2D);

            //Ahora volvemos a restaurar el Render Target original (osea dibujar a la pantalla)
            d3dDevice.SetRenderTarget(0, superficieVieja);


            //Luego tomamos lo dibujado antes y lo combinamos con una textura con efecto de alarma
            RenderOnScreen(d3dDevice, ElapsedTime);

            //Finaliza el render y presenta en pantalla, al igual que el preRender se debe para casos puntuales es mejor utilizar a mano las operaciones de EndScene y PresentScene

        }


        public void RenderOnScreen(Microsoft.DirectX.Direct3D.Device d3dDevice, float elapsedTime)
        {
            d3dDevice.BeginScene();

            //Cargamos para renderizar el unico modelo que tenemos, un Quad que ocupa toda la pantalla, con la textura de todo lo dibujado antes
            d3dDevice.VertexFormat = CustomVertex.PositionTextured.Format;
            d3dDevice.SetStreamSource(0, screenQuadVB, 0);

            //Ver si el efecto de alarma esta activado, configurar Technique del shader segun corresponda

            //Cargamos parametros en el shader de Post-Procesado
            this.postProcessMerge.SetValue("escenaTextura", this.escena);
            if (cant_pasadas == 0)
                this.postProcessMerge.SetValue("propulsoresTextura", this.propulsores);
            else
                this.postProcessMerge.SetValue("propulsoresTextura", this.propulsoresBlurAux2);
            this.postProcessMerge.Technique = "TechniqueMerge";
            //Limiamos la pantalla y ejecutamos el render del shader
            d3dDevice.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);
            this.postProcessMerge.Begin(FX.None);
            this.postProcessMerge.BeginPass(0);
            d3dDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
            this.postProcessMerge.EndPass();
            this.postProcessMerge.End();

            //Terminamos el renderizado de la escena
            RenderFPS();
            RenderAxis();
            d3dDevice.EndScene();
            d3dDevice.Present();
        }

        public void RenderShadowMap()
        {
            var effect = TgcShaders.Instance.TgcMeshPointLightShader;
            // Calculo la matriz de view de la luz
            effect.SetValue("g_vLightPos", new Vector4(g_LightPos.X, g_LightPos.Y, g_LightPos.Z, 1));
            effect.SetValue("g_vLightDir", new Vector4(g_LightDir.X, g_LightDir.Y, g_LightDir.Z, 1));
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
            D3DDevice.Instance.Device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);
            D3DDevice.Instance.Device.BeginScene();

            // Hago el render de la escena pp dicha
            effect.SetValue("g_txShadow", g_pShadowMap);

            navePrincipal.ActionOnNave(m => m.Technique = "RenderShadow");
            navePrincipal.Render(sol.Position, Camara.Position);
            if (spawnaearEnemigos)
            {
                enemigos.FindAll(enemigo => enemigo.EstaViva() && enemigo.EnemigoEstaAdelante()).ForEach(e => e.Scene.Meshes.ForEach(m => m.Technique = "RenderShadow"));
                enemigos.FindAll(enemigo => enemigo.EstaViva() && enemigo.EnemigoEstaAdelante()).ForEach(e => e.Render(sol.Position, Camara.Position));
            }
            escenarios.ForEach(es => es.Scene.Meshes.ForEach(m => m.Technique = "RenderShadow"));
            escenarios.ForEach(es => es.torres.ForEach(t => t.Scene.Meshes.ForEach(m => m.Technique = "RenderShadow")));
            escenarios.ForEach(es => es.Render(sol.Position, Camara.Position, ElapsedTime));

            // Termino
            D3DDevice.Instance.Device.EndScene();

            if (save)
            {
                TextureLoader.Save("shadowmap.jpg", ImageFileFormat.Jpg, g_pShadowMap);
                save = false;
            }

            // restuaro el render target y el stencil
            D3DDevice.Instance.Device.DepthStencilSurface = pOldDS;
            D3DDevice.Instance.Device.SetRenderTarget(0, pOldRT);
        }

        public void RenderDebugText()
        {

            DrawText.drawText("Posicion de la nave: " + TGCVector3.PrintVector3(this.navePrincipal.Scene.Meshes[0].Position), 0, 500, Color.White);
            DrawText.drawText("Rotacion de la nave: " + TGCVector3.PrintVector3(this.navePrincipal.Scene.Meshes[0].Rotation), 0, 45, Color.White);
            DrawText.drawText("Scale de la nave: " + TGCVector3.PrintVector3(this.navePrincipal.RotationVector), 0, 55, Color.White);
            DrawText.drawText("Scale de la nave: " + TGCVector3.PrintVector3(this.navePrincipal.MovementVector), 0, 85, Color.White);
            DrawText.drawText("Scale de la nave: " + TGCVector3.PrintVector3(this.currentScene.Scene.BoundingBox.PMin), 0, 105, Color.White);
            DrawText.drawText("Scale de la nave: " + TGCVector3.PrintVector3(this.currentScene.Scene.BoundingBox.PMax), 0, 115, Color.White);

            DrawText.drawText("Tu vida: " + navePrincipal.Vida, 0, 150, Color.White);
            DrawText.drawText("Vida del enemigo 1: " + enemigos[0].Vida, 0, 190, Color.White);
            DrawText.drawText("Vida del enemigo 2: " + enemigos[1].Vida, 0, 230, Color.White);
            DrawText.drawText("Vida del enemigo 3: " + enemigos[2].Vida, 0, 270, Color.White);
            DrawText.drawText("Menu: " + menu.estaEnMenu, 0, 370, Color.White);
            DrawText.drawText("Menu: ancho " + GameForm.MousePosition, 0, 385, Color.White);
            DrawText.drawText("Menu: alto" + TGCVector2.PrintVector2(menu.GetMenuPrincipal().tamanio()), 0, 400, Color.White);
            DrawText.drawText("Menu: posicion mouse" + TGCVector2.PrintVector2(new TGCVector2(Input.Xpos, Input.Ypos)), 0, 415, Color.White);
            DrawText.drawText("Menu: hitbox" + TGCVector2.PrintVector2(menu.GetMenuPrincipal().tamanio() + menu.GetMenuPrincipal().posicion()), 0, 430, Color.White);
            DrawText.drawText("Tu vida: " + navePrincipal.Vida, 0, 150, Color.White);
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
            playerAmbiente.closeFile();
            g_pShadowMap.Dispose();
            g_pDSShadow.Dispose();
        }
    }
}