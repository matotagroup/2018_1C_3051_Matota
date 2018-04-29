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
        private TgcScene SceneEstrellaDeLaMuerte { get; set; }
        private TgcScene LeftWallEstrellaDeLaMuerte { get; set; }
        private TgcScene RightWallEstrellaDeLaMuerte { get; set; }

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

            this.navePrincipal = new NaveEspacial(MediaDir, "xwing-TgcScene.xml");
            this.navePrincipal.ScaleFactor = TGCMatrix.Scaling(0.5f, 0.5f, 0.5f);
            this.navePrincipal.RotationVector = new TGCVector3(0, FastMath.PI_HALF, 0);

            //Codigo ejemplo donde se muestra como configurar y cargar mesh
            /*//Textura de la carperta Media. Game.Default es un archivo de configuracion (Game.settings) util para poner cosas.
            //Pueden abrir el Game.settings que se ubica dentro de nuestro proyecto para configurar.
            var pathTexturaCaja = MediaDir + Game.Default.TexturaCaja;

            //Cargamos una textura, tener en cuenta que cargar una textura significa crear una copia en memoria.
            //Es importante cargar texturas en Init, si se hace en el render loop podemos tener grandes problemas si instanciamos muchas.
            var texture = TgcTexture.createTexture(pathTexturaCaja);

            //Creamos una caja 3D ubicada de dimensiones (5, 10, 5) y la textura como color.
            var size = new TGCVector3(5, 10, 5);
            //Construimos una caja según los parámetros, por defecto la misma se crea con centro en el origen y se recomienda así para facilitar las transformaciones.
            Box = TGCBox.fromSize(size, texture);
            //Posición donde quiero que este la caja, es común que se utilicen estructuras internas para las transformaciones.
            //Entonces actualizamos la posición lógica, luego podemos utilizar esto en render para posicionar donde corresponda con transformaciones.
            Box.Position = new TGCVector3(-25, 0, 0);*/

            // IMPORTANTE: UBICAR LA CARPETA MEDIA EN 2018_1C_3051_Matota\TGC.Group
            SceneEstrellaDeLaMuerte = new TgcSceneLoader().loadSceneFromFile(MediaDir + "XWing/death+star-TgcScene.xml", MediaDir + "XWing/");
            LeftWallEstrellaDeLaMuerte = new TgcSceneLoader().loadSceneFromFile(MediaDir + "XWing/death+star-TgcScene.xml", MediaDir + "XWing/");
            RightWallEstrellaDeLaMuerte = new TgcSceneLoader().loadSceneFromFile(MediaDir + "XWing/m1-TgcScene.xml", MediaDir + "XWing/");

            this.ActionOnScene((mesh) => {
                mesh.AutoTransform = false;
                mesh.Transform = TGCMatrix.Scaling(new TGCVector3(50f, 200f, 80f)) * TGCMatrix.RotationY(FastMath.PI_HALF);
            });
            
            
            this.ActionOnSceneWallLeft((mesh) =>{
                mesh.AutoTransform = false;
                mesh.Transform = TGCMatrix.Scaling(new TGCVector3(50f, 200f, 80f)) * TGCMatrix.RotationY(FastMath.PI_HALF) * TGCMatrix.Translation(new TGCVector3(0,0,-8500f));
            });

            this.ActionOnSceneWallRight((mesh) => {
                mesh.Scale = new TGCVector3(100f, 100f, 100f);
                mesh.RotateZ(FastMath.PI_HALF);
                mesh.Position = new TGCVector3(2000f, 0, 0);
            });
            //La nave tiene mas de un Mesh, si se toma el primero hay parte que no se esta teniendo en cuenta y terminamos teniendo parte de la nave en vez de toda la nave.


            
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
            if (Input.keyDown(Key.Up) || Input.keyDown(Key.W))
                movimientoNave.Z = -1;
            else if (Input.keyDown(Key.Down) || Input.keyDown(Key.S))
                movimientoNave.Z = 5;
            //Movimiento para elevarse con E y Control para bajar , todo sobre el eje Y.
            if (Input.keyDown(Key.E))
                movimientoNave.Y = 1;
            else if (Input.keyDown(Key.LeftControl))
                movimientoNave.Y = -1;
            //boost de velocidad con shift
            if (Input.keyDown(Key.LeftShift))
                movimientoNave.Z = -5;
            //Activar BarrelRoll 
            //TODO: Implementar cooldown?
            if (Input.keyDown(Key.Space))
                this.navePrincipal.DoBarrelRoll();

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
        LeftWallEstrellaDeLaMuerte.BoundingBox.Render();
        RightWallEstrellaDeLaMuerte.BoundingBox.Render();



           // }

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
    }
}