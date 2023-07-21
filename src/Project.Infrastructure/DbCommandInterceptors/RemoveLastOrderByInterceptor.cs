using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Project.Infrastructure.DbCommandInterceptors;

/// <summary>
/// 處理EF.Core自動加入Order By衍伸的資料庫效能問題
/// https://github.com/dotnet/efcore/issues/19828
/// </summary>
public class RemoveLastOrderByInterceptor : DbCommandInterceptor
{
    public const string QueryTag = "RemoveLastOrderBy";
    public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(
        DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result,
        CancellationToken cancellationToken = default)
    {
        const string orderBy = "ORDER BY";
        if (command.CommandText.Contains(QueryTag) && command.CommandText.Contains(orderBy))
        {
            int lastOrderBy = command.CommandText.LastIndexOf(orderBy, StringComparison.Ordinal);
            //beware of string manip on memory consumption
            command.CommandText = command.CommandText.Remove(lastOrderBy);
            command.CommandText += ";";
        }

        return base.ReaderExecutingAsync(command, eventData, result, cancellationToken);

    }
}
