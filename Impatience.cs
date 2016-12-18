using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HugsLib.DetourByAttribute;
using Verse;

namespace Fluffy_WorkshopTools
{
    public class Impatience
    {
        // get rid of the bloody interaction delay on steam uploads
        [DetourProperty( typeof(Dialog_MessageBox), "InteractionDelayExpired", DetourProperty.Getter )]
        public bool InteractionDelayExpired => true;
    }
}
