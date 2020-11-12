"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var testing_1 = require("@angular/core/testing");
var instancemanagerdashboard_component_1 = require("./instancemanagerdashboard.component");
describe('InstancemanagerdashboardComponent', function () {
    var component;
    var fixture;
    beforeEach(testing_1.async(function () {
        testing_1.TestBed.configureTestingModule({
            declarations: [instancemanagerdashboard_component_1.InstancemanagerdashboardComponent]
        })
            .compileComponents();
    }));
    beforeEach(function () {
        fixture = testing_1.TestBed.createComponent(instancemanagerdashboard_component_1.InstancemanagerdashboardComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });
    it('should create', function () {
        expect(component).toBeTruthy();
    });
});
//# sourceMappingURL=instancemanagerdashboard.component.spec.js.map