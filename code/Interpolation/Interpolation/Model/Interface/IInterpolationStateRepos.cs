using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Interpolation.Model.Interface
{
    interface IInterpolationStateRepos
    {
        IList<InterpolationState> queryIntersect(InterpolationState IpS);

        IList<InterpolationState> queryIntersectWithParent(InterpolationState fatherIps, int sonNo);

        bool insert(InterpolationState IpS);

        bool insertList(IList<InterpolationState> list);

        bool modifyList(IList<InterpolationState> list);
    }
}
