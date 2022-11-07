using System.Collections.Generic;

namespace Penta.EeWin.ServerApi.Data.Response
{
    public class AuthList : Response
    {
        public Apc apc;
        public Policy policy;
    }
    public class Apc
    {
        public string apcToken;
        public bool passedAll;
        public bool sso;
        public string tgt;
        public string validUntilDate;
    }

    public class Policy
    {
        public string priority;
                
        public List<FactorList>[] factorList;
    }
    
    public class FactorList
    {
        public int id;
        public string type;
        public string name;
        public string config;
    }
}