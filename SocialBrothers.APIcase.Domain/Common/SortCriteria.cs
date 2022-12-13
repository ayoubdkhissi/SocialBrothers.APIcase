using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialBrothers.APIcase.Domain.Common;
public class SortCriteria
{
    public string OrderBy { get; set; }

    public bool Desc { get; set; } = false;
}
