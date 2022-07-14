using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace XRayOperatorTool.Model.Emitter
{
    public class EmitterBoard : AbstractBoard
    {

        public EmitterBoard(PanelCoordinates coordinates, string positionIdentifier, BoardStatus status)
            : base(coordinates, positionIdentifier, status)
        {
        }
    }
}
