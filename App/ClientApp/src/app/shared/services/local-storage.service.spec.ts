import { TestBed, async } from '@angular/core/testing';

import { LocalStorageService } from './local-storage.service';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { RouterTestingModule } from '@angular/router/testing';

describe('LocalStorageService', () => {
    let service : LocalStorageService
    let store = {};
    const mockLocalStorage = {
      getItem: (key: string): string => {
        return key in store ? store[key] : null;
      },
      setItem: (key: string, value: string) => {
        store[key] = `${value}`;
      },
    };
   
    beforeEach(async(() => {
      TestBed.configureTestingModule({
        imports: [
          HttpClientTestingModule,
          RouterTestingModule
        ],
      })
      .compileComponents();
    }));

    beforeEach(() => {
        spyOn(localStorage, 'getItem')
        .and.callFake(mockLocalStorage.getItem);
        spyOn(localStorage, 'setItem')
        .and.callFake(mockLocalStorage.setItem);
      
      });

    it('should be created', () => {
        const service: LocalStorageService = TestBed.get(LocalStorageService);
        expect(service).toBeTruthy();
      });
    
    it('should return stored value of TexfabClients',() => {
        let localStorageRefServiceSpy = jasmine.createSpyObj('LocalStorageRef', ['clientCustomer']);
        let getLocalStorageSpy = jasmine.createSpyObj('LocalStorageRef.clientCustomer', ['getItem', 'setItem']);
        localStorageRefServiceSpy.clientCustomer.and.returnValue(getLocalStorageSpy);
        getLocalStorageSpy.getItem.and.callFake(mockLocalStorage.getItem);
        getLocalStorageSpy.setItem.and.callFake(mockLocalStorage.setItem);
        });
  
  });
