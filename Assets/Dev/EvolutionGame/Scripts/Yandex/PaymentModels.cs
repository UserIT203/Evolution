using UnityEngine;
using YG;

namespace PaymentModels
{

    public class PaymentItem
    {
        public string ID {  get; set; }

        public bool IsBuyed
        {
            get
            {
                int status = YG2.GetState(ID);

                return status == 1;
            }
        }
    
        public PaymentItem(string id)
        {
            ID = id;
        }
    }

    public class StartKit : PaymentItem
    {
        public StartKit(string id) : base(id)
        {
        }

        public int CoinCount
        {
            get
            {
                return 8000;
            }

            private set { }
        }

        public int GemCount
        {
            get
            {
                return 550;
            }

            private set { }
        }
    }
}
