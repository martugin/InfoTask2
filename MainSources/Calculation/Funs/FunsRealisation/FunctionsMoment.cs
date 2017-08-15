using System;
using CommonTypes;

namespace Calculation
{
    internal partial class CalcFunctions
    {
        //5 - Мгновенные
        public IReadMean Aperture_rr(FunData data, DataType dataType, params IReadMean[] par)
        {
            var res = MFactory.NewList(dataType);
            var list = par[0]; 
            var dif = par[1];
            if (list.Count == 0) return res;
            var prev = data.ParamsValues[0];
            list.CurNum = 0;
            if (prev == null) res.AddMom(list);
            IReadMean m = prev ?? list.ToMean();
            for (list.CurNum = 0; list.CurNum < list.Count; list.CurNum++)
                if (Math.Abs(list.Real - m.Real) >= dif.Real)
                    res.AddMom(m = list.ToMean());
            data.ParamsValues[0] = m;
            return res;
        }

        private IReadMean Aperture_ir(FunData data, DataType dataType, params IReadMean[] par)
        {
            return Aperture_rr(data, dataType, par);
        }
    }
}