import { EnvSpecific } from "./EnvSpecific";

export class AppSettings {
    

  public static baseURL = '';
  public static appVersion = "";
   
    public static setValue(env: EnvSpecific): void {
      AppSettings.baseURL = env.baseURL;
      AppSettings.appVersion = env.appVersion;
       
      
    }
}
