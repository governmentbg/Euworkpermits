namespace BlueCardPortal.Infrastructure.Constants
{
    public static class MessageConstant
    {
        /// <summary>
        /// tostr Грешка
        /// </summary>
        public const string ErrorMessage = "ErrorMessage";

        /// <summary>
        /// tostr Внимание
        /// </summary>
        public const string WarningMessage = "WarningMessage";

        /// <summary>
        /// tostr Успех
        /// </summary>
        public const string SuccessMessage = "SuccessMessage";

        /// <summary>
        /// swal Грешка
        /// </summary>
        public const string SwalErrorMessage = "SwalErrorMessage";

        /// <summary>
        /// swal Внимание
        /// </summary>
        public const string SwalWarningMessage = "SwalWarningMessage";

        /// <summary>
        /// swal Успех
        /// </summary>
        public const string SwalSuccessMessage = "SwalSuccessMessage";


        /// <summary>
        /// Не е избран (за Display Template-ите)
        /// </summary>
        public const string NotSelected = "Не е избран";

        /// <summary>
        /// Да (за Display Template-ите)
        /// </summary>
        public const string Yes = "Да";

        /// <summary>
        /// Не (за Display Template-ите)
        /// </summary>
        public const string No = "Не";

        public static class Values
        {
            public const string SaveOK = "Записът премина успешно.";
            public const string DeleteOK = "Успешно изтриване на запис.";
            public const string SaveFailed = "Проблем по време на запис.";
            public const string UpdateOK = "Обновяването премина успешно.";
            public const string UpdateFailed = "Проблем при обновяването на данните.";
            public const string FileNotFound = "Файлът не е намерен!";
            public const string Unauthorized = "Нямате права върху този ресурс!";
            public const string AccessDenied = "Достъпът отказан!";
            public const string SignPdfOK = "Успешно подписване на документ.";
            public const string SignPdfFailed = "Неуспешно подписване на документ.";
            public const string FillMantadoryFields = "Моля попълнете липсващите данни в задължителните полета.";
            public const string ErrorProcessingData = "Грешка при обработка на данните.";
            public const string ResourceNotFound = "Ресурсът не е намерен!";
            public const string FileUploadFailed = "Неуспешно прикачване на файл.";
        }
    }
}
