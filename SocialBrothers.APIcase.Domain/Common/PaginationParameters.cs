using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialBrothers.APIcase.Domain.Common;
public class PaginationParameters
{
    const int MAX_PAGE_SIZE = 40;

    public int PageIndex { get; set; } = 1;

    private int _pageSize = 20;
    public int PageSize
    {
        get { return _pageSize; }
        set
        {
            _pageSize = value < MAX_PAGE_SIZE ? value : MAX_PAGE_SIZE;
        }
    }
}
