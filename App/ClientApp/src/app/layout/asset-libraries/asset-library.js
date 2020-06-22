"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.SSMLGenerator = exports.AssetSearchParameter = exports.AssetLibrarySearchParameter = exports.Asset = exports.AssetLibrary = void 0;
var commonmodel_1 = require("../../shared/models/commonmodel");
var AssetLibrary = /** @class */ (function () {
    function AssetLibrary() {
        this.Description = "";
        this.Assets = null;
    }
    return AssetLibrary;
}());
exports.AssetLibrary = AssetLibrary;
var Asset = /** @class */ (function () {
    function Asset() {
    }
    return Asset;
}());
exports.Asset = Asset;
var AssetLibrarySearchParameter = /** @class */ (function () {
    function AssetLibrarySearchParameter() {
        this.PagingParameter = new commonmodel_1.PagingParameter();
        this.SortParameter = new commonmodel_1.SortParameter();
        this.SearchMode = commonmodel_1.SearchMode.Equals;
    }
    return AssetLibrarySearchParameter;
}());
exports.AssetLibrarySearchParameter = AssetLibrarySearchParameter;
var AssetSearchParameter = /** @class */ (function () {
    function AssetSearchParameter() {
        this.PagingParameter = new commonmodel_1.PagingParameter();
        this.SortParameter = new commonmodel_1.SortParameter();
        this.SearchMode = commonmodel_1.SearchMode.Equals;
    }
    return AssetSearchParameter;
}());
exports.AssetSearchParameter = AssetSearchParameter;
var SSMLGenerator = /** @class */ (function () {
    function SSMLGenerator() {
        this.Description = "";
        this.AudioURL = "";
        this.AssetLibraries = null;
    }
    return SSMLGenerator;
}());
exports.SSMLGenerator = SSMLGenerator;
//# sourceMappingURL=asset-library.js.map