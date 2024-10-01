using Microsoft.EntityFrameworkCore;

namespace BlueCardPortal.Infrastructure.Data.Common
{
    public interface ISoftDeletable
    {
        [Comment("Дали записа е изтрит или не")]
        bool IsDeleted { get; set; }

        [Comment("Точно време на изтриване на записа")]
        DateTime? DeletedAt { get; set; }
    }
}
