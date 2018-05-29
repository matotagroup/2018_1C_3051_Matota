using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Direct3D;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Group.Model.UtilsParaGUI;

namespace TGC.Group
{
    class Simbolo
    {
        protected Core.Input.TgcD3dInput input;
        public CustomSprite spriteActual = new CustomSprite();
        public CustomBitmap spritePrincipal;
        public TGCVector2 Position { get { return spriteActual.Position; } set { spriteActual.Position = value; } }
        public Simbolo(string MediaDir, string texture, TgcD3dInput input)
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
        public void cambiarTextura(string MediaDir, string texture)
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
        public void Update(float elapsedTime, Menu menu)
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


