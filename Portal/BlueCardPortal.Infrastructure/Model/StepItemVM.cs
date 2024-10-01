using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueCardPortal.Infrastructure.Model
{
    /// <summary>
    /// Стъпка в уизард
    /// </summary>
    public class StepItemVM
    {
        /// <summary>
        /// Иденификатор
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Номер на стъпка
        /// </summary>
        public int Number { get; set; }
      
        /// <summary>
        /// Име на контрола която се показва
        /// </summary>
        public string? CollapseName { get; set; }

        /// <summary>
        /// Име на partial view с данни за стъпката
        /// </summary>
        public string? ViewName { get; set; }

        /// <summary>
        /// Html Field Prefix
        /// </summary>
        public string? HtmlPrefix { get; set; }
        /// <summary>
        /// Наименование на стъпката
        /// </summary>
        public string? Label { get; set; }

        /// <summary>
        /// Данни
        /// </summary>
        public object Data { get; set; } = null!;
        
        /// <summary>
        /// Дали това е текущия елемент
        /// </summary>
        public bool IsCurrent { get; set; }
        /// <summary>
        /// Дали е активен елемента
        /// </summary>
        public bool IsDisabled { get; set; }

        /// <summary>
        /// Дали е видим елемента за този вид заявление
        /// </summary>
        public bool IsHidden { get; set; }

        public string IsHiddenStyle()
        {
            return IsHidden ? "display:none" : string.Empty;
        }
    }
}
