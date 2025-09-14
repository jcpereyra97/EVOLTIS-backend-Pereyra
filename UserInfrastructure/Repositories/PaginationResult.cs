using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserInfrastructure.Repositories
{
    public sealed record PaginationResult<T>(IReadOnlyList<T> Items, int Page, int PageSize, int TotalCount);
}
