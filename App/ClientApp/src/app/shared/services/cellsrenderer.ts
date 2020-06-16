import { Injectable } from '@angular/core';


function _window():any{
    return window ;
}

@Injectable({
    providedIn: 'root'
  })

export class CellRenderService {
    constructor() { }

    get nativeWindow():any{
        
        return _window();
    }
}