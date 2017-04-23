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
        int angleDula { get; set; }
        int angleTank { get; set; }
        double weaponCD { get; set; }
        double maxAngle { get; set; }
    }

    class Tank : ITank
    {
        private double _hp = 100;
        private double _x;
        private double _y;
        private int _angleDula = 0;
        private int _angleTank = 0;
        private double _weaponCD = 2;
        private double _maxAngle = 70;

        public Tank(float xInput, float yInput, int angleTankInput)
        {
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
                _x = value;
            }
        }

        public double y
        {
            get
            {
                return _y;
            }

            set
            {
                _y = value;
            }
        }

        public int angleDula
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

        public int angleTank
        {
            get
            {
                return _angleTank;
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
    }

}
