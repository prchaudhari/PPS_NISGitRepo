"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var testing_1 = require("@angular/core/testing");
var role_service_1 = require("./role.service");
describe('RoleService', function () {
    beforeEach(function () { return testing_1.TestBed.configureTestingModule({}); });
    it('should be created', function () {
        var service = testing_1.TestBed.get(role_service_1.RoleService);
        expect(service).toBeTruthy();
    });
});
//# sourceMappingURL=role.service.spec.js.map