using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Mathematica;
using TGC.Group.Model.UtilsParaGUI;
using TGC.Core.Input;
using TGC.Core.Direct3D;

namespace TGC.Group
{
    public class Hud
    {
        private Barra barraVida;
        private Barra barraAfterBurn;
        private Simbolo corazon;
        private Simbolo afterBurn;
        private TGCVector2 vidaScale = new TGCVector2(0.3f, 0.15f);
        private TGCVector2 afterBurnScale = new TGCVector2(1f, 0.15f);
        private TGCVector2 scaleCorazon = new TGCVector2(0.32f, 0.25f);
        private TGCVector2 scaleAfterBurn = new TGCVector2(0.15f, 0.15f);
        public int W = D3DDevice.Instance.Width;
        public int H = D3DDevice.Instance.Height;
        private List<Barra> barras;
        private List<Simbolo> simbolos;
        public Hud(string MediaDir, TgcD3dInput input)
        {
            barraVida = new Barra(MediaDir, "barra_vida.png");
            barraAfterBurn = new Barra(MediaDir, "barra_afterburn.png");
            barraVida.Position = new TGCVector2(100, H / 1.5f);
            barraAfterBurn.Position = new TGCVector2(100, H / 1.25f);
            barraVida.Scaling = new TGCVector2(0.3f, 0.1f);
            barraAfterBurn.Scaling = new TGCVector2(0.15f, 0.25f);

            corazon = new Simbolo(MediaDir, "corazon.png", input);
            afterBurn = new Simbolo(MediaDir, "afterburn_simbolo.png", input);
            corazon.Position = new TGCVector2(17, H / 1.5f);
            afterBurn.Position = new TGCVector2(20, H / 1.30f);
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
            barraAfterBurn.ModificarBarra(nave.AfterBurnFuel);
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