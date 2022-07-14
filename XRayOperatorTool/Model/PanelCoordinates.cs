using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XRayOperatorTool.Model
{
    public class PanelCoordinates
    {
        public int X { get; set; }
        public int Y { get; set; }

        public PanelCoordinates()
        {

        }
        public PanelCoordinates(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is PanelCoordinates))
            {
                return false;
            }
            var otherPanelCoordinate = obj as PanelCoordinates;
            return X.Equals(otherPanelCoordinate.X) && Y.Equals(otherPanelCoordinate.Y);
        }
    }
}
