using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Penta.EeWin.Cp.Base
{
    public class CredentialPair : Tuple<Credential, List<Field.Field>>
    {
        public CredentialPair(Credential item1, List<Field.Field> item2) 
            : base(item1, item2)
        {
        }
    }
}
