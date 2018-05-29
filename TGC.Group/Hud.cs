using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Mathematica;
using TGC.Group.Model.UtilsParaGUI;
using TGC.Core.Input;

namespace TGC.Group
{
    public class Hud
    {
        private Barra barraVida;
        private Barra barraAfterBurn;
        private TGCVector2 posicionVida = new TGCVector2(150, 600);
        private TGCVector2 posicionAfterBurn = new TGCVector2(150, 710);
        private Simbolo corazon;
        private Simbolo afterBurn;
        private TGCVector2 vidaScale = new TGCVector2(0.3f, 0.15f);
        private TGCVector2 afterBurnScale = new TGCVector2(1f, 0.15f);
        private TGCVector2 posicionCorazon = new TGCVector2(50, 600);
        private TGCVector2 posicionAfterBurnSimbolo = new TGCVector2(50, 680);
        private TGCVector2 scaleCorazon = new TGCVector2(0.32f, 0.25f);
        private TGCVector2 scaleAfterBurn = new TGCVector2(0.15f, 0.15f);
        private List<Barra> barras;
        private List<Simbolo> simbolos;
        public Hud(string MediaDir, TgcD3dInput input)
        {
            //barraVida.Position = posicionVida;
            barraVida = new Barra(MediaDir, "barra_vida.png");
            barraAfterBurn = new Barra(MediaDir, "barra_afterburn.png");
            barraVida.Position = posicionVida;
            barraAfterBurn.Position = posicionAfterBurn;
            barraVida.Scaling = vidaScale;
            barraAfterBurn.Scaling = afterBurnScale;

            corazon = new Simbolo(MediaDir, "corazon.png", input);
            afterBurn = new Simbolo(MediaDir, "afterburn_simbolo.png", input);
            corazon.Position = posicionCorazon;
            afterBurn.Position = posicionAfterBurnSimbolo;
            corazon.Scaling = scaleCorazon;
            afterBurn.Scaling = scaleAfterBurn;

            barras = new List<Barra>();
            barras.Add(barraVida);
            barras.Add(barraAfterBurn);

            simbolos = new List<Simbolo>();
            simbolos.Add(corazon);
            simbolos.Add(afterBurn);
        }
        public void Render(float elapsedTime, Drawer2D drawer)
        {
            barras.ForEach(barra => barra.Render(elapsedTime, drawer));
            simbolos.ForEach(simbolo => simbolo.Render(elapsedTime, drawer));
        }
        public void Update(NaveEspacial nave)
        {
            barraVida.ModificarBarra(nave.Vida*vidaScale.X);
            nave.RecargarFuel(0.1f);
            barraAfterBurn.ModificarBarra(nave.afterBurnFuel);
        }
        public void ReducirBarraVida(float vidaNave)
        {
            barraVida.ModificarBarra(vidaNave * vidaScale.X);
        }
        public void ReducirFuel(float cantUsada)
        {
            barraAfterBurn.ModificarBarra(cantUsada);
        }
    }
}