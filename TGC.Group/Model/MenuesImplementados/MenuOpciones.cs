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
    class MenuOpciones : TipoMenu
    {
        private Boton audio;
        private Boton instrucciones;
        private Boton controles;
        private Boton volver;

        private TGCVector2 posicionVolver = new TGCVector2(100, 800);
        private TGCVector2 posicionAudio = new TGCVector2(100, 600);
        private TGCVector2 posicionInstrucciones = new TGCVector2(100, 200);
        private TGCVector2 posicionControles = new TGCVector2(100, 400);

        private List<Boton> botonesOpciones;

        public MenuOpciones(string MediaDir, TgcD3dInput input)
        {
            audio = new Boton(MediaDir, "audio.png", input, "seleccion_audio.png", new AccionAudio());
            instrucciones = new Boton(MediaDir, "instrucciones.png", input, "seleccion_instrucciones.png", new AccionInstrucciones());
            controles = new Boton(MediaDir, "controles.png", input, "seleccion_controles.png", new AccionControles());
            volver = new Boton(MediaDir, "volver.png", input, "seleccion_volver.png", new AccionVolver());

            audio.Position = posicionAudio;
            instrucciones.Position = posicionInstrucciones;
            controles.Position = posicionControles;
            volver.Position = posicionVolver;

            this.botonesOpciones = new List<Boton>();
            botonesOpciones.Add(audio);
            botonesOpciones.Add(instrucciones);
            botonesOpciones.Add(controles);
            botonesOpciones.Add(volver);
        }
        public void Render(float elapsedTime, Drawer2D drawer)
        {
            botonesOpciones.ForEach(boton => boton.Render(elapsedTime, drawer));
        }
        public void Update(float elapsedTime, Menu menu)
        {
            botonesOpciones.ForEach(boton => boton.Update(elapsedTime, menu));
        }

    }
}
