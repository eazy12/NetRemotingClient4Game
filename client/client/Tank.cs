using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace client
{
    interface ITank
    {
        double hp { get; set; }
        double x { get; set; }
        double y { get; set; }
        double angleDula { get; set; }
        double angleTank { get; set; }
        double weaponCD { get; set; }
        double maxAngle { get; set; }
    }

    class Tank : ITank
    {
        private double tankWidth;
        private double tankHeight;
        private double tankSpeed;
        private double _hp = 100;
        private double _x;
        private double _oldX;
        private double _y;
        private double _oldY;
        private double _angleDula = 0;
        private double _angleTank = 0;
        private double _weaponCD = 2;
        private double _maxAngle = 70;
        private double _terrainHeight;

        public Tank(double tankWidthInput, double tankHeightInput, double tankSpeedInput, double xInput, double yInput, int angleTankInput)
        {
            tankWidth = tankWidthInput;
            tankHeight = tankHeightInput;
            tankSpeed = tankSpeedInput;

            x = xInput;
            y = yInput;
            angleTank = angleTankInput;
        }
               

        public double hp
        {
            get
            {
                return _hp;
            }

            set
            {
                _hp = value;
            }
        }


        public double x
        {
            get
            {
                return _x;
            }

            set
            {
                _oldX = _x;
                _x = value;
            }
        }

        public double y
        {
            get
            {
                // пытался исправить проваливание гусениц через землю. Но, видимо, дело в том, что танк
                // поварачиваясь при подъеме в гору деформируется из-за того, что соотношение x:y окна не равно 1
                return _y + tankHeight; // / Math.Cos( _angleTank * 180.0 / Math.PI );
            }

            set
            {
                _oldY = _y;
                _y = value;
            }
        }

        public double angleDula
        {
            get
            {
                return _angleDula;
            }

            set
            {
                _angleDula = value;
            }
        }

        public double angleTank
        {
            get
            {
                return Math.Atan((_y - _oldY) / (_x - _oldX) ) * 180 / Math.PI;
            }

            set
            {
                _angleTank = value;
            }
        }

        public double weaponCD
        {
            get
            {
                return _weaponCD;
            }

            set
            {
                _weaponCD = value;
            }
        }

        public double maxAngle
        {
            get
            {
                return _maxAngle;
            }

            set
            {
                _maxAngle = value;
            }
        }

        public double terrainHeight
        {
            get
            {
                return _terrainHeight;
            }

            set
            {
                _terrainHeight = value;
            }
        }
    }

}
