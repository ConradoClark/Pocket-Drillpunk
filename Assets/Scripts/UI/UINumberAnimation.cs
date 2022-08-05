using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Licht.Unity.Objects;

public abstract class UINumberAnimation : BaseUIObject
{
    public abstract IEnumerable<IEnumerable<Action>> AnimateOldNumbers(UINumberRenderer numberRenderer,int currentNumber, int newNumber);
    public abstract IEnumerable<IEnumerable<Action>> AnimateNewNumbers(UINumberRenderer numberRenderer);
}
