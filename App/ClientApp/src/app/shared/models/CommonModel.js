"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.SearchMode = exports.SortOrder = exports.SortParameter = exports.PagingParameter = void 0;
var PagingParameter = /** @class */ (function () {
    function PagingParameter() {
        this.PageIndex = 0;
        this.PageSize = 0;
    }
    return PagingParameter;
}());
exports.PagingParameter = PagingParameter;
var SortParameter = /** @class */ (function () {
    function SortParameter() {
    }
    return SortParameter;
}());
exports.SortParameter = SortParameter;
var SortOrder;
(function (SortOrder) {
    SortOrder[SortOrder["Ascending"] = 1] = "Ascending";
    SortOrder[SortOrder["Descending"] = 2] = "Descending";
})(SortOrder = exports.SortOrder || (exports.SortOrder = {}));
var SearchMode;
(function (SearchMode) {
    SearchMode[SearchMode["Contains"] = 1] = "Contains";
    SearchMode[SearchMode["Equals"] = 2] = "Equals";
})(SearchMode = exports.SearchMode || (exports.SearchMode = {}));
//# sourceMappingURL=CommonModel.js.map