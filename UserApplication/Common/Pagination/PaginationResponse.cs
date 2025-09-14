using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserApplication.Common.Pagination
{
    public record PaginationResponse<T>(
        IReadOnlyList<T> Items,
        int Page,
        int PageSize,
        int TotalCount)
    {
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public bool HasPrevious => Page > 1;
        public bool HasNext => Page < TotalPages;
    }
}

