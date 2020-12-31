using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCTG
{
    class Store
    {
        private string id;
        private string cardtotrade;
        private string type;
        private double minimumdamage;

        public Store() { }
        
        public string Id
        {
            get { return id; }
            set { id = value; }
        }

        public string CardToTrade
        {
            get { return cardtotrade; }
            set { cardtotrade = value; }
        }

        public string Type
        {
            get { return type; }
            set { type = value; }
        }

        public double MinimumDamage
        {
            get { return minimumdamage; }
            set { minimumdamage = value; }
        }
    }
}