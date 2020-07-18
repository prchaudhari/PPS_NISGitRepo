import { Component, OnInit, Injector } from '@angular/core';
import { Constants } from 'src/app/shared/constants/constants';
import { RenderengineService } from '../renderengine.service';
import { ActivatedRoute, Router, NavigationEnd } from '@angular/router';
import { NgxUiLoaderService } from 'ngx-ui-loader';
import { MessageDialogService } from 'src/app/shared/services/mesage-dialog.service';
import { LocalStorageService } from 'src/app/shared/services/local-storage.service';

@Component({
  selector: 'app-view',
  templateUrl: './view.component.html',
  styles: []
})
export class ViewComponent implements OnInit {

  public params: any = {};
  public RenderEngineIdentifier: number = 0;
  public renderEngine: any = {};

  constructor(private injector: Injector,
    private uiLoader: NgxUiLoaderService,
    private _messageDialogService: MessageDialogService,
    private router: Router,
    private localstorageservice: LocalStorageService,
    private renderEngineService: RenderengineService) {

      router.events.subscribe(e => {
        if (e instanceof NavigationEnd) {
            if (e.url.includes('/renderengines')) {
                //set passing parameters to localstorage.
                if (localStorage.getItem('renderEngineViewRouteparams')) {
                  this.params = JSON.parse(localStorage.getItem('renderEngineViewRouteparams'));
                  this.RenderEngineIdentifier = this.params.Routeparams.passingparams.RenderEngineIdentifier
                }
            } else {
                localStorage.removeItem("renderEngineViewRouteparams");
            }
        }
      });
     }

  ngOnInit() {
    this.getRenderEngineDetails();
  }

  async getRenderEngineDetails() {
    let searchParameter: any = {};
      searchParameter.PagingParameter = {};
      searchParameter.PagingParameter.PageIndex = Constants.DefaultPageIndex;
      searchParameter.PagingParameter.PageSize = Constants.DefaultPageSize;
      searchParameter.SortParameter = {};
      searchParameter.SortParameter.SortColumn = Constants.Name;
      searchParameter.SortParameter.SortOrder = Constants.Ascending;
      searchParameter.SearchMode = Constants.Exact;
      if (this.RenderEngineIdentifier != null) {
        searchParameter.Identifier = this.RenderEngineIdentifier;
      }
      let renderEngineRecords = await this.renderEngineService.getRenderEngines(searchParameter);
      if(renderEngineRecords.length != 0) {
        this.renderEngine = renderEngineRecords[0];
      }
  };

  // method written to navigate to render engine list
  navigateToRenderEngineList() {
    this.router.navigate(['renderengines']);
  }

  navigateToEditRenderEngine() {
    let queryParams = {
      Routeparams: {
        passingparams: {
          "RenderEngineIdentifier": this.renderEngine.Identifier,
        }
      }
    }
    localStorage.setItem("renderEngineEditRouteparams", JSON.stringify(queryParams));
    this.router.navigate(['renderengines', 'Edit']);
    localStorage.removeItem("renderEngineViewRouteparams");
  }

  async deleteRenderEngineRecord() {
    let message = 'Are you sure, you want to delete this record?';
    this._messageDialogService.openConfirmationDialogBox('Confirm', message, Constants.msgBoxWarning).subscribe(async (isConfirmed) => {
      if (isConfirmed) {
        let renderEngineData = [{
          "Identifier": this.renderEngine.Identifier,
        }];
        let isDeleted = await this.renderEngineService.deleteRenderEngine(renderEngineData);
          if (isDeleted) {
            let messageString = Constants.recordDeletedMessage;
            this._messageDialogService.openDialogBox('Success', messageString, Constants.msgBoxSuccess);
            this.navigateToRenderEngineList();
            localStorage.removeItem("renderEngineViewRouteparams");
          }
      }
    });
  }
}
