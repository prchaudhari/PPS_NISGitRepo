import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CommonService {

  // To hold system data and configuration
  private systemPreferences: any = {
  };
  // Observable current  Page Info Object Source
  public event: any = {};
  // Observable current  Page Info Object Observable
  public eventObservable = new Subject<any>();
  public eventObservableForNotification = new Subject<any>();
  public changeEmitted: any;

  constructor() {
    this.changeEmitted = this.eventObservableForNotification.asObservable();
  }

  /**
   * @param key : Getter returns user data as passed key in argument
   */
  public get(key: string): any {
    return this.systemPreferences[key];
  }

  /**
   * @param key : Removed user data as passed key in argument
   */
  public remove(key: string, flag?: boolean): any {
    delete this.systemPreferences[key];
  }

  /**
   * @param key : Property nane sets user data with a named key as passed key in argument
   * @param data : Setter sets user data on passed key in argument
   */
  public set(key: string, data: any, flag?: boolean): void {
    this.systemPreferences[key] = data;
  }


  /**
   * @param key : Property name sets user data with a named key as passed key in argument
   * @param data : Setter sets user data on passed key in argument
   */
  public setEvent(event: string, data: any) {
    this.event = { 'event': event, 'data': data };
    this.eventObservable.next(this.event);
  }

  /**
   * @param systemPreferences : Property sets system preferences
   */
  public setUserPreferenceFromStorage(systemPreferences: any): void {
    this.systemPreferences = systemPreferences;
  }

  public notificationReadAction() {
    this.eventObservableForNotification.next();
  }
}
