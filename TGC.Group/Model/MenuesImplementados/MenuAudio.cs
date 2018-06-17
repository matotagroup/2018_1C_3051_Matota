using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Direct3D;
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

        public int W = D3DDevice.Instance.Width;
        public int H = D3DDevice.Instance.Height;
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

            atras.Position = new TGCVector2(75, H / 1.15f);
            sonidoAmbiente.Position = new TGCVector2(100, H / 3.5f);
            sonidoDisparos.Position = new TGCVector2(100, H / 2.5f);
            sonidoMenu.Position = new TGCVector2(100, H / 2f);
            onSonidoMenu.Position = new TGCVector2(600, H / 2f);
            onSonidoAmbiente.Position = new TGCVector2(600, H / 3.5f);
            onSonidoDisparos.Position = new TGCVector2(600, H / 2.5f);
            sonidoAmbiente.Scaling = new TGCVector2(0.4f, 0.4f);
            sonidoDisparos.Scaling = new TGCVector2(0.4f, 0.4f);
            sonidoMenu.Scaling = new TGCVector2(0.4f, 0.4f);
            onSonidoAmbiente.Scaling = new TGCVector2(0.4f, 0.4f);
            onSonidoDisparos.Scaling = new TGCVector2(0.4f, 0.4f);
            onSonidoMenu.Scaling = new TGCVector2(0.4f, 0.4f);
            atras.Scaling = new TGCVector2(0.4f, 0.4f);

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
