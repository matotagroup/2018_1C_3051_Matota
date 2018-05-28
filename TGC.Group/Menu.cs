using Microsoft.DirectX;
using Microsoft.DirectX.DirectInput;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Direct3D;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.Text;
using TGC.Group.Model.UtilsParaGUI;

namespace TGC.Group
{
    class Menu
    {
        public Boolean menuPrincipal { get; set; }
        public Boolean menuOpciones { get; set; }
        public Boolean menuInstrucciones { get; set; }
        public Boolean menuControles { get; set; }
        public Boolean menuAudio { get; set; }
        public Boolean playSonidoAmbiente { get; set; }
        public Boolean playSonidoMenu { get; set; }
        public Boolean playSonidoDisparos { get; set; }
        //objetos de los menúes
        private Simbolo tituloPrincipal;
        public Simbolo onSonidoMenu { get; set; }
        public Simbolo onSonidoAmbiente { get; set; }
        public Simbolo onSonidoDisparos { get; set; }
        private Boton empezar;
        private Boton opciones;
        private Boton salir;
        private Boton audio;
        private Boton instrucciones;
        private Boton controles;
        private Boton volver;
        private Boton atras;
        private Boton sonidoMenu;
        private Boton sonidoAmbiente;
        private Boton sonidoDisparos;

        private List<Simbolo> simbolos;
        private List<Boton> botonesMenuPrincipal;
        private List<Boton> botonesOpciones;
        private List<Boton> botonesSonidos;
        private List<Simbolo> simbolosSonido;
        //posiciones
        private TGCVector2 posicionTitulo = new TGCVector2(750, 50);
        private TGCVector2 posicionEmpezar = new TGCVector2(100, 400);
        private TGCVector2 posicionOpciones = new TGCVector2(100, 600);
        private TGCVector2 posicionSalir = new TGCVector2(100, 800);
        private TGCVector2 posicionVolver = new TGCVector2(100, 800);
        private TGCVector2 posicionAudio = new TGCVector2(100, 600);
        private TGCVector2 posicionInstrucciones = new TGCVector2(100,200);
        private TGCVector2 posicionControles = new TGCVector2(100, 400);
        private TGCVector2 posicionAtras = new TGCVector2(100, 800);
        private TGCVector2 posicionSonidoMenu = new TGCVector2(100, 200);
        private TGCVector2 posicionSonidoAmbiente = new TGCVector2(100, 400);
        private TGCVector2 posicionSonidoDisparos = new TGCVector2(100, 600);
        private TGCVector2 posicionOnSonidoMenu = new TGCVector2(1300, 200);
        private TGCVector2 posicionOnSonidoAmbiente = new TGCVector2(1300, 400);
        private TGCVector2 posicionOnSonidoDisparos = new TGCVector2(1300, 600);
        //textos
        private TgcText2D textoInstrucciones = new TgcText2D();
        private TgcText2D textoControles = new TgcText2D();

        public Menu(string MediaDir, TgcD3dInput input)
        {
            //instancio objetos del menú principal
            tituloPrincipal = new Simbolo(MediaDir, "titulo.png", input);
            empezar = new Boton(MediaDir, "empezar.png", input, "seleccion_empezar.png", new AccionEmpezar());
            opciones = new Boton(MediaDir, "opciones.png", input, "seleccion_opciones.png", new AccionOpciones());
            salir = new Boton(MediaDir, "salir.png", input, "seleccion_salir.png", new AccionSalir());

            tituloPrincipal.Position = posicionTitulo;
            empezar.Position = posicionEmpezar;
            opciones.Position = posicionOpciones;
            salir.Position = posicionSalir;

            menuPrincipal = true;
            menuAudio = false;
            menuControles = false;
            menuInstrucciones = false;
            menuOpciones = false;

            this.simbolos = new List<Simbolo>();
            simbolos.Add(tituloPrincipal);

            this.botonesMenuPrincipal = new List<Boton>();
            botonesMenuPrincipal.Add(empezar);
            botonesMenuPrincipal.Add(opciones);
            botonesMenuPrincipal.Add(salir);

            //instancio botones del menú opciones
            audio = new Boton(MediaDir, "audio.png", input, "seleccion_audio.png", new AccionAudio());
            instrucciones = new Boton(MediaDir, "instrucciones.png", input, "seleccion_instrucciones.png", new AccionInstrucciones());
            controles = new Boton(MediaDir, "controles.png", input, "seleccion_controles.png", new AccionControles());
            volver = new Boton(MediaDir, "volver.png", input, "seleccion_volver.png", new AccionVolver());
            atras = new Boton(MediaDir, "atras.png", input, "seleccion_atras.png", new AccionAtras());
            sonidoMenu = new Boton(MediaDir, "sonido_menu.png", input, "seleccion_sonido_menu.png", new AccionSonido(new AccionSonidoMenu(),MediaDir));
            sonidoDisparos = new Boton(MediaDir, "sonido_disparos.png", input, "seleccion_sonido_disparos.png", new AccionSonido(new AccionSonidoDisparos(),MediaDir));
            sonidoAmbiente = new Boton(MediaDir, "sonido_ambiente.png", input, "seleccion_sonido_ambiente.png", new AccionSonido(new AccionSonidoAmbiente(),MediaDir));
            onSonidoMenu = new Simbolo(MediaDir, "on.png", input);
            onSonidoDisparos = new Simbolo(MediaDir, "on.png", input);
            onSonidoAmbiente = new Simbolo(MediaDir, "on.png", input);

            audio.Position = posicionAudio;
            instrucciones.Position = posicionInstrucciones;
            controles.Position = posicionControles;
            volver.Position = posicionVolver;
            atras.Position = posicionAtras;
            sonidoAmbiente.Position = posicionSonidoAmbiente;
            sonidoDisparos.Position = posicionSonidoDisparos;
            sonidoMenu.Position = posicionSonidoMenu;
            onSonidoMenu.Position = posicionOnSonidoMenu;
            onSonidoAmbiente.Position = posicionOnSonidoAmbiente;
            onSonidoDisparos.Position = posicionOnSonidoDisparos;

            this.botonesOpciones = new List<Boton>();
            botonesOpciones.Add(audio);
            botonesOpciones.Add(instrucciones);
            botonesOpciones.Add(controles);
            botonesOpciones.Add(volver);

            this.botonesSonidos = new List<Boton>();
            botonesSonidos.Add(sonidoMenu);
            botonesSonidos.Add(sonidoDisparos);
            botonesSonidos.Add(sonidoAmbiente);

            this.simbolosSonido = new List<Simbolo>();
            simbolosSonido.Add(onSonidoDisparos);
            simbolosSonido.Add(onSonidoMenu);
            simbolosSonido.Add(onSonidoAmbiente);

            this.TextoInstrucciones();
            this.TextoControles();
            this.playSonidoAmbiente = true;
            this.playSonidoDisparos = true;
            this.playSonidoMenu = true;
        }

        public void Update(float elapsedTime)
        {
            if (menuPrincipal)
            {
                   botonesMenuPrincipal.ForEach(boton => boton.Update(elapsedTime,this));
            }
            else if (menuOpciones)
            {
                botonesOpciones.ForEach(boton => boton.Update(elapsedTime, this));
            }
            else if (menuAudio || menuControles || menuInstrucciones)
            {
                botonesSonidos.ForEach(boton => boton.Update(elapsedTime, this));
                atras.Update(elapsedTime, this);
            }
           
        }

        public void Render(float elapsedTime,Drawer2D drawer)
        {
            if (menuPrincipal)
            {
                simbolos.ForEach(simbolo => simbolo.Render(elapsedTime, drawer));
                botonesMenuPrincipal.ForEach(boton => boton.Render(elapsedTime,drawer));
            }
            else if (menuOpciones)
            {
                botonesOpciones.ForEach(boton => boton.Render(elapsedTime, drawer));
            }
            else if (menuInstrucciones)
            {
                textoInstrucciones.render();
                atras.Render(elapsedTime,drawer);
            }
            else if (menuControles)
            {
                textoControles.render();
                atras.Render(elapsedTime, drawer);
            }
            else if (menuAudio)
            {
                botonesSonidos.ForEach(boton => boton.Render(elapsedTime, drawer));
                simbolosSonido.ForEach(simbolo => simbolo.Render(elapsedTime, drawer));
                atras.Render(elapsedTime, drawer);
            }
        }

        public void Dispose()
        {
            textoInstrucciones.Dispose();
            textoControles.Dispose();
        }

        public void TextoInstrucciones()
        {
            textoInstrucciones.Text = "El juego es una copia del Star Wars donde una nave se desplaza por pasillos y es atacada, debe sobrevivir el mayor tiempo posible.";
            textoInstrucciones.Position = new Point(450, 200);
            textoInstrucciones.Color = Color.Yellow;
            textoInstrucciones.Size = new Size(1000, 100);
            textoInstrucciones.changeFont(new Font("TimesNewRoman", 30));
        }
        
        public void TextoControles()
        {
            textoControles.Text = "Derecha D Izquierda I  Disparar F  Boost Shift";
            textoControles.Position = new Point(100, 200);
            textoControles.Color = Color.Yellow;
            textoControles.Size = new Size(300, 100);
            textoControles.changeFont(new Font("TimesNewRoman", 30));
        }
    }

    class Simbolo
    {
        protected Core.Input.TgcD3dInput input;
        public CustomSprite spriteActual = new CustomSprite();
        public CustomBitmap spritePrincipal;
        public TGCVector2 Position { get { return spriteActual.Position; } set { spriteActual.Position = value; } }
        public Simbolo(string MediaDir,string texture,TgcD3dInput input)
        {
            spritePrincipal = new CustomBitmap(MediaDir + "XWing\\Textures\\" + texture, D3DDevice.Instance.Device);
            this.input = input;
            spriteActual.Bitmap = spritePrincipal;
        }
        public void Render(float elapsedTime, Drawer2D drawer)
        {
            drawer.BeginDrawSprite();
            drawer.DrawSprite(spriteActual);
            drawer.EndDrawSprite();
        }
        public void cambiarTextura(string MediaDir,string texture)
        {
            spritePrincipal = new CustomBitmap(MediaDir + "XWing\\Textures\\" + texture, D3DDevice.Instance.Device);
            spriteActual.Bitmap = spritePrincipal;
        }
        
    }


    class Boton : Simbolo
    {
        private CustomBitmap spriteSeleccionado;   //sprite si el mouse pasa sobre el boton
        private Accion accion;                     //hace una accion dependiendo del boton que sea
        public Boton(string MediaDir, string texture, TgcD3dInput input, string mouseOverTexture, Accion accion) : base(MediaDir, texture, input)
        {
            spriteSeleccionado = new CustomBitmap(MediaDir + "XWing\\Textures\\" + mouseOverTexture, D3DDevice.Instance.Device);
            this.accion = accion;
        }
        public TGCVector2 tamanio()
        {
            return new TGCVector2(this.spriteActual.Bitmap.Width, this.spriteActual.Bitmap.Height);
        }
        public TGCVector2 posicion()
        {
            return new TGCVector2(input.Xpos, input.Ypos);
        }
        public TGCVector2 maxVec()
        {
            return this.Position + this.tamanio();
        }
        public void Update(float elapsedTime,Menu menu)
        {
            var mousePos = new TGCVector2(input.Xpos, input.Ypos);
            var minVec = this.Position;
            var maxVec = this.Position + this.tamanio();

            if (minVec.X < mousePos.X && minVec.Y < mousePos.Y && mousePos.Y < maxVec.Y && mousePos.Y < maxVec.Y)
            {
                spriteActual.Bitmap = spriteSeleccionado;
                if (input.buttonPressed(TgcD3dInput.MouseButtons.BUTTON_LEFT))
                {
                    accion.realizar(menu);
                }
            }
            else
            {
                spriteActual.Bitmap = spritePrincipal;
            }
        }
    }
}
