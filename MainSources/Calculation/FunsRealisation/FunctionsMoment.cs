using System;
using CommonTypes;

namespace Calculation
{
    internal partial class FunctionsCalc
    {
        //5 - Мгновенные
        public IMean Aperture_rr(FunData data, DataType dataType, params IMean[] par)
        {
            var res = MFactory.NewList(dataType);
            var list = par[0]; 
            var dif = par[1];
            if (list.Count == 0) return res;
            var prev = data.ParamsValues[0];
            list.CurNum = 0;
            if (prev == null) res.AddMom(list);
            IMean m = prev ?? list.CloneMean();
            for (list.CurNum = 0; list.CurNum < list.Count; list.CurNum++)
                if (Math.Abs(list.Real - m.Real) >= dif.Real)
                    res.AddMom(m = list.CloneMean());
            data.ParamsValues[0] = m;
            return res;
        }

        private IMean Aperture_ir(FunData data, DataType dataType, params IMean[] par)
        {
            return Aperture_rr(data, dataType, par);
        }
    }
}