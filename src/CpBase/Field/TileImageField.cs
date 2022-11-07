using CredentialProvider.Interop;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Penta.EeWin.Cp.Base.Field
{
    public class TileImageField : Field
    {
        private Bitmap bitmapImage;

        public Bitmap BitmapImage { get { return bitmapImage; } }

        public TileImageField(in uint fieldId, in string label, in _CREDENTIAL_PROVIDER_FIELD_STATE fieldState, in _CREDENTIAL_PROVIDER_FIELD_INTERACTIVE_STATE fieldInteractiveState, Bitmap bitmapImgae)
            : base(fieldId, _CREDENTIAL_PROVIDER_FIELD_TYPE.CPFT_TILE_IMAGE, label, fieldState, fieldInteractiveState, PInvoke.CPFG_CREDENTIAL_PROVIDER_LOGO)
        {
            this.bitmapImage = bitmapImgae;
        }
    }
}
