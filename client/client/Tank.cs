using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace client
{
    interface ITank
    {
        float hp { get; set; }
        float x { get; set; }
        float y { get; set; }
        int angleDula { get; set; }
        int angleTank { get; set; }
        float weaponCD { get; set; }
        float maxAngle { get; set; }
    }

    class Tank : ITank
    {
        private float _hp = 100;
        private float _x;
        private float _y;
        private int _angleDula = 90;
        private int _angleTank = 0;
        private float _weaponCD = 2;
        private float _maxAngle = 70;

        public Tank(float xInput, float yInput, int angleTankInput)
        {
            x = xInput;
            y = yInput;
            angleTank = angleTankInput;
        }

        

        public float hp
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


        public float x
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

        public float y
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

        public float weaponCD
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

        public float maxAngle
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
