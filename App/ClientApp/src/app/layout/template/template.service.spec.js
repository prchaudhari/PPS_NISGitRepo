"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var testing_1 = require("@angular/core/testing");
var template_service_1 = require("./template.service");
describe('TemplateService', function () {
    beforeEach(function () { return testing_1.TestBed.configureTestingModule({}); });
    it('should be created', function () {
        var service = testing_1.TestBed.get(template_service_1.TemplateService);
        expect(service).toBeTruthy();
    });
});
//# sourceMappingURL=template.service.spec.js.map