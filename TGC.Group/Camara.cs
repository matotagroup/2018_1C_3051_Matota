using TGC.Core.Camara;
using TGC.Core.Mathematica;

namespace TGC.Group
{
    public class CamaraStarWars : TgcCamera
    {
        /// <summary>
        /// Crea la camara 3D para el Star Wars Trucho.
        /// </summary>
        /// <param name="target">El objeto al cual va a apuntar, deberia ser la nave.</param>
        /// <param name="offsetHeight">Distancia desde donde va a mirar desde arriba.</param>
        /// <param name="offsetForward">Distancia desde donde va a mirar desde adelante (si es positivo va desde atras).</param>
        public CamaraStarWars(TGCVector3 target, float offsetHeight, float offsetForward)
        {
            Target = target;
            OffsetHeight = offsetHeight;
            OffsetForward = offsetForward;
        }

        /// <summary>
        ///     Desplazamiento en altura de la camara respecto del target
        /// </summary>
        public float OffsetHeight { get; set; }

        /// <summary>
        ///     Desplazamiento hacia adelante o atras de la camara repecto del target.
        ///     Para que sea hacia atras tiene que ser negativo.
        /// </summary>
        public float OffsetForward { get; set; }

        /// <summary>
        ///     Objetivo al cual la camara tiene que apuntar
        /// </summary>
        public TGCVector3 Target { get; set; }

        //El metodo es llamado desde postupdate para actualizar la camara en el update loop.
        public override void UpdateCamera(float elapsedTime)
        {
            SetCamera(CalculatePositionTarget(), Target);
        }

        /// <summary>
        ///     Genera la proxima matriz de view, sin actualizar aun los valores internos
        /// </summary>
        /// <param name="pos">Futura posicion de camara generada</param>
        public TGCVector3 CalculatePositionTarget()
        {
            //la primer matriz ubica la camara en la posicion original, se multiplica por la segunda para que siga al objeto, sino es como una camara que ve que el objeto se aleja.
            var m = TGCMatrix.Translation(0, OffsetHeight, OffsetForward) * TGCMatrix.Translation(Target);

            //Por definicion de la matriz de traslacion los valores de pos estan en la 4 fila y las primeras 3 columnas.
            return new TGCVector3(m.M41, m.M42, m.M43);
        }
    }
}
