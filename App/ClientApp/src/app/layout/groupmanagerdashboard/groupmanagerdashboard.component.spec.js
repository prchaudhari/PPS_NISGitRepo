"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var testing_1 = require("@angular/core/testing");
var groupmanagerdashboard_component_1 = require("./groupmanagerdashboard.component");
describe('GroupmanagerdashboardComponent', function () {
    var component;
    var fixture;
    beforeEach(testing_1.async(function () {
        testing_1.TestBed.configureTestingModule({
            declarations: [groupmanagerdashboard_component_1.GroupmanagerdashboardComponent]
        })
            .compileComponents();
    }));
    beforeEach(function () {
        fixture = testing_1.TestBed.createComponent(groupmanagerdashboard_component_1.GroupmanagerdashboardComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });
    it('should create', function () {
        expect(component).toBeTruthy();
    });
});
//# sourceMappingURL=groupmanagerdashboard.component.spec.js.map