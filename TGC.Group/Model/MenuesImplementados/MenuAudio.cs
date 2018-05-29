using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Group.Model.UtilsParaGUI;

namespace TGC.Group.Model.MenuesImplementados
{
    class MenuAudio : TipoMenu
    {
        public Simbolo onSonidoMenu { get; set; }
        public Simbolo onSonidoAmbiente { get; set; }
        public Simbolo onSonidoDisparos { get; set; }


        private Boton atras;
        private Boton sonidoMenu;
        private Boton sonidoAmbiente;
        private Boton sonidoDisparos;

        private TGCVector2 posicionAtras = new TGCVector2(100, 800);
        private TGCVector2 posicionSonidoMenu = new TGCVector2(100, 200);
        private TGCVector2 posicionSonidoAmbiente = new TGCVector2(100, 400);
        private TGCVector2 posicionSonidoDisparos = new TGCVector2(100, 600);
        private TGCVector2 posicionOnSonidoMenu = new TGCVector2(1300, 200);
        private TGCVector2 posicionOnSonidoAmbiente = new TGCVector2(1300, 400);
        private TGCVector2 posicionOnSonidoDisparos = new TGCVector2(1300, 600);

        private List<Boton> botonesSonidos;
        private List<Simbolo> simbolosSonido;

        public MenuAudio(string MediaDir, TgcD3dInput input)
        {
            atras = new Boton(MediaDir, "atras.png", input, "seleccion_atras.png", new AccionAtras());
            sonidoMenu = new Boton(MediaDir, "sonido_menu.png", input, "seleccion_sonido_menu.png", new AccionSonido(new AccionSonidoMenu(), MediaDir));
            sonidoDisparos = new Boton(MediaDir, "sonido_disparos.png", input, "seleccion_sonido_disparos.png", new AccionSonido(new AccionSonidoDisparos(), MediaDir));
            sonidoAmbiente = new Boton(MediaDir, "sonido_ambiente.png", input, "seleccion_sonido_ambiente.png", new AccionSonido(new AccionSonidoAmbiente(), MediaDir));
            onSonidoMenu = new Simbolo(MediaDir, "on.png", input);
            onSonidoDisparos = new Simbolo(MediaDir, "on.png", input);
            onSonidoAmbiente = new Simbolo(MediaDir, "on.png", input);

            atras.Position = posicionAtras;
            sonidoAmbiente.Position = posicionSonidoAmbiente;
            sonidoDisparos.Position = posicionSonidoDisparos;
            sonidoMenu.Position = posicionSonidoMenu;
            onSonidoMenu.Position = posicionOnSonidoMenu;
            onSonidoAmbiente.Position = posicionOnSonidoAmbiente;
            onSonidoDisparos.Position = posicionOnSonidoDisparos;

            this.botonesSonidos = new List<Boton>();
            botonesSonidos.Add(sonidoMenu);
            botonesSonidos.Add(sonidoDisparos);
            botonesSonidos.Add(sonidoAmbiente);

            this.simbolosSonido = new List<Simbolo>();
            simbolosSonido.Add(onSonidoDisparos);
            simbolosSonido.Add(onSonidoMenu);
            simbolosSonido.Add(onSonidoAmbiente);
        }
        public void Render(float elapsedTime, Drawer2D drawer)
        {
            simbolosSonido.ForEach(simbolo => simbolo.Render(elapsedTime, drawer));
            botonesSonidos.ForEach(boton => boton.Render(elapsedTime, drawer));
            atras.Render(elapsedTime, drawer);
        }
        public void Update(float elapsedTime, Menu menu)
        {
            botonesSonidos.ForEach(boton => boton.Update(elapsedTime, menu));
            atras.Update(elapsedTime, menu);
        }

    }
}
