"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.AppSettings = void 0;
var AppSettings = /** @class */ (function () {
    function AppSettings() {
    }
    AppSettings.setValue = function (env) {
        AppSettings.baseURL = env.baseURL;
        AppSettings.appVersion = env.appVersion;
    };
    AppSettings.baseURL = '';
    AppSettings.appVersion = "";
    return AppSettings;
}());
exports.AppSettings = AppSettings;
//# sourceMappingURL=appsettings.js.map