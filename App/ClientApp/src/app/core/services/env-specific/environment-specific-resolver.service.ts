import { Injectable } from '@angular/core';
import {
    Resolve, RouterStateSnapshot,
    ActivatedRouteSnapshot
} from '@angular/router';

import { EnvSpecific } from '../../../envspecific';
import { EnvironmentSpecificService } from './environment-specific.service';

@Injectable()
export class EnvironmentSpecificResolver implements Resolve<EnvSpecific> {
    constructor(private envSpecificSvc: EnvironmentSpecificService) { }

    resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Promise<EnvSpecific> {
        return this.envSpecificSvc.loadEnvironment()
            .then(es => {
                this.envSpecificSvc.setEnvSpecific(es);
                return this.envSpecificSvc.envSpecific;
            }, error => {
                console.log(error);
                return null;
            });
    }
}
