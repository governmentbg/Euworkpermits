/*!
 * Tempus Dominus v6.2.7 (https://getdatepicker.com/)
 * Copyright 2013-2022 Jonathan Peterson
 * Licensed under MIT (https://github.com/Eonasdan/tempus-dominus/blob/master/LICENSE)
 */
(function (g, f) {
    typeof exports === "object" && typeof module !== "undefined" ? f(exports) : typeof define === "function" && define.amd ? define(["exports"], f) : ((g = typeof globalThis !== "undefined" ? globalThis : g || self), f(((g.tempusDominus = g.tempusDominus || {}), (g.tempusDominus.locales = g.tempusDominus.locales || {}), (g.tempusDominus.locales.bg = {}))));
})(this, function (exports) {
    "use strict";
    const name = "bg";
    const localization = {
        today: "Днес",
        clear: "Изчистете",
        close: "Затворете",
        selectMonth: "Изберете месец",
        previousMonth: "Предходен месец",
        nextMonth: "С;ледващ месец",
        selectYear: "Изберете година",
        previousYear: "Предходна година",
        nextYear: "Следваща година",
        selectDecade: "Изберете десетилетие",
        previousDecade: "Предходно десетилетие",
        nextDecade: "Следващо десетилетие",
        previousCentury: "Предходен век",
        nextCentury: "Следващ век",
        pickHour: "Изберете час",
        incrementHour: "Увеличете времето",
        decrementHour: "Намалете времето",
        pickMinute: "Изберете минута",
        incrementMinute: "Увеличете минута",
        decrementMinute: "Намалете минута",
        pickSecond: "Изберете второ",
        incrementSecond: "Увеличете секунди",
        decrementSecond: "Намалете секунди",
        toggleMeridiem: "Превключете период",
        selectTime: "Изберете време",
        selectDate: "Изберете дата",
        dayViewHeaderFormat: { month: "long", year: "2-digit" },
        locale: "bg",
        startOfTheWeek: 1,
        dateFormats: {
            LTS: 'HH:mm:ss',
            LT: 'HH:mm',
            L: 'dd.MM.yyyy',
            LL: 'd. MMMM yyyy',
            LLL: 'd. MMMM yyyy HH:mm',
            LLLL: 'dddd, d. MMMM yyyy HH:mm',
        },
        ordinal: n => n,
        format: "L"
    };
    exports.localization = localization;
    exports.name = name;
    Object.defineProperty(exports, "__esModule", { value: true });
});
