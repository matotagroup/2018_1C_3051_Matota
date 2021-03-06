﻿using System;
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
    class MenuOpciones : TipoMenu
    {
        private Boton audio;
        private Boton instrucciones;
        private Boton controles;
        private Boton volver;

        public int W = D3DDevice.Instance.Width;
        public int H = D3DDevice.Instance.Height;
        private List<Boton> botonesOpciones;

        public MenuOpciones(string MediaDir, TgcD3dInput input)
        {
            audio = new Boton(MediaDir, "audio.png", input, "seleccion_audio.png", new AccionAudio());
            instrucciones = new Boton(MediaDir, "instrucciones.png", input, "seleccion_instrucciones.png", new AccionInstrucciones());
            controles = new Boton(MediaDir, "controles.png", input, "seleccion_controles.png", new AccionControles());
            volver = new Boton(MediaDir, "volver.png", input, "seleccion_volver.png", new AccionVolver());

            instrucciones.Position = new TGCVector2(100, H / 3.25f);
            audio.Position = new TGCVector2(100, H / 2.25f);
            controles.Position = new TGCVector2(100, H / 1.75f);
            volver.Position = new TGCVector2(100, H / 1.25f);
            instrucciones.Scaling = new TGCVector2(0.5f, 0.6f);
            audio.Scaling = new TGCVector2(0.6f, 0.6f);
            controles.Scaling = new TGCVector2(0.6f, 0.6f);
            volver.Scaling = new TGCVector2(0.4f, 0.4f);

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
