import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject } from 'rxjs/Rx';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';
import 'rxjs/add/operator/toPromise';
import { EnvSpecific } from '../../../envspecific';
import { AppSettings } from '../../../appsettings';

@Injectable()
export class EnvironmentSpecificService {

  public envSpecific: EnvSpecific;
  public envSpecificNull: EnvSpecific;
  private envSpecificSubject: BehaviorSubject<EnvSpecific> = new BehaviorSubject<EnvSpecific>(this.envSpecific);
  products ;
    //: any = (data as any).default;

  constructor(private http: HttpClient) {
    //this.products = a.primaryMain;
  }

  public loadEnvironment():any {
    if (this.envSpecific === null || this.envSpecific === undefined) {
      
      return this.http.get('assets/env-specific.json')
        //.map((data) => )
        .toPromise<any>();
    }

    return Promise.resolve(this.envSpecificNull);
  }

  public setEnvSpecific(es: EnvSpecific) {
    if (es === null || es === undefined) {
      return;
    }

    this.envSpecific = es;

    AppSettings.setValue(this.envSpecific);

    if (this.envSpecificSubject) {
      this.envSpecificSubject.next(this.envSpecific);
    }
  }

  public subscribe(caller: any, callback: (caller: any, es: EnvSpecific) => void) {
    this.envSpecificSubject
      .subscribe((es) => {
        if (es === null) {
          return;
        }
        callback(caller, es);
      });
  }
}
