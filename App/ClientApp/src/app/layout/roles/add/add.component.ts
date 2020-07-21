import { Component, OnInit, Injector, ChangeDetectorRef } from '@angular/core';
import { Location } from '@angular/common';
import { Constants } from 'src/app/shared/constants/constants';
import { FormGroup, FormBuilder, Validators, FormControl, SelectControlValueAccessor, FormArray, ValidatorFn } from '@angular/forms';
import { RoleService } from '../role.service';
import { ActivatedRoute, Router, NavigationEnd } from '@angular/router';
import { NgxUiLoaderService } from 'ngx-ui-loader';
import { HttpEvent, HttpEventType, HttpResponse } from '@angular/common/http';
import { MessageDialogService } from 'src/app/shared/services/mesage-dialog.service';
import { DialogService } from '@tomblue/ng2-bootstrap-modal';
import { LocalStorageService } from 'src/app/shared/services/local-storage.service';
import { ConfigConstants } from 'src/app/shared/constants/configConstants';
import { RolePrivilege } from '../../../shared/models/rolePrivilege';

@Component({
  selector: 'app-add',
  templateUrl: './add.component.html',
  styleUrls: ['./add.component.scss']
})
export class AddComponent implements OnInit {

  //variable declaration here
  public isCollapsedRoles: boolean = true;
  public isCollapsedDetails: boolean = false;
  public isCollapsedPermissions: boolean = true;
  public isLoaderActive: boolean = false;
  public isShowFilterRoles: boolean = false;
  public roleFormGroup: FormGroup;
  public permissonFilterForm: FormGroup;
  public params: any = {};
  public RoleIdentifier;
  public RoleName;
  public status;
  public EntityName: any = []
  public DisplayName: any = []
  public RolePrivilegeOperations: any = [];
  public Operation;
  public IsEnabled;
  public EntityNames
  public checked;
  public roleLists = [];
  public rolRecord = [];
  public categoriesSelected = []
  isTrade: boolean = false;
  public checkedRolePrivilages = [];
  checkAllTrades: boolean = false
  public title: string = "Add";
  public roleFormErrorObject: any = {
    showRoleNameError: false
  };
  public roleEditModeOn: boolean = false;

  //regex
  public onlyAlphabetsWithSpace = '[a-zA-Z ]*';
  public onlyAlphabetswithInbetweenSpaceUpto50Characters = Constants.onlyAlphabetswithInbetweenSpaceUpto50Characters;
  public rolePrivilegesList = [];
  public entityList: RolePrivilege[] = [];
  public roleAddEditResources = {}
  public ResourceLoadingFailedMsg = Constants.ResourceLoadingFailedMsg;
  public allPermisions: boolean = false
  public rowWisePermisions: boolean = false;
  // public IsAllRolePrevilegeSelected: boolean = false;

  public chcekBoxValue;
  public Locale;
  public trackByFn;
  public checkedRole = [];
  public data = [];
  public selectedAll: any = [];
  public IsAllRolePrevilegeSelected: any;
  public selectAll: any;
  public check: boolean = false;
  public checkedRoles = [];
  public userClaimsRolePrivilegeOperations;
  public IsAllRole: any;

  public dependentEntity: any[] = [
    {
      "EntityName": "Dashboard",
      "DependentEntity": [
        "Page",
        "Schedule Management"
      ]
    },
    {
      "EntityName": "User",
      "DependentEntity": [
        "Role",
      ]
    },
    {
      "EntityName": "Role",
    },
    {
      "EntityName": "Asset Library",

    },
    {
      "EntityName": "Widget",
    },
    {
      "EntityName": "Page",
      "DependentEntity": [
        "Asset Library",
        "Widget"
      ]
    },
    {
      "EntityName": "Statement Definition",
      "DependentEntity": [
        "Page"
      ]
    },
    {
      "EntityName": "Schedule Management",
      "DependentEntity": [
        "Statement Definition"
      ]
    },
    {
      "EntityName": "Log",
      "DependentEntity": [
        "Statement Definition",
        "User"
      ]
    },
    {
      "EntityName": "Analytics",
      "DependentEntity": [
        "Page",
        "Widget"
      ]
    }];

  public dependentEntityMap: any[] = [
    {
      "EntityName": "Dashboard",
      "DependentEntity": []
    },
    {
      "EntityName": "User",
      "DependentEntity": [
        "Log",
      ]
    },
    {
      "EntityName": "Role",
      "DependentEntity": [
        "User",
      ]
    },
    {
      "EntityName": "Asset Library",
      "DependentEntity": [
        "Page",

      ]
    },
    {
      "EntityName": "Widget",
      "DependentEntity": [
        "Page",
        "Analytics"
      ]
    },
    {
      "EntityName": "Page",
      "DependentEntity": [
        "Statement Definition",
        "Analytics"
      ]
    },
    {
      "EntityName": "Statement Definition",
      "DependentEntity": [
        "Schedule Management",
        "Log"
      ]
    },
    {
      "EntityName": "Schedule Management",
      "DependentEntity": [
      ]
    },
    {
      "EntityName": "Log",
      "DependentEntity": [
      ]
    },
    {
      "EntityName": "Analytics",
      "DependentEntity": [
      ]
    }];

  //getters of roleForm group
  get roleName() {
    return this.roleFormGroup.get('roleName');
  }

  get roleDescription() {
    return this.roleFormGroup.get('roleDescription');
  }

  //function to validate all fields
  validateAllFormFields(formGroup: FormGroup) {
    Object.keys(formGroup.controls).forEach(field => {
      const control = formGroup.get(field);
      if (control instanceof FormControl) {
        control.markAsTouched({ onlySelf: true });
      } else if (control instanceof FormGroup) {
        this.validateAllFormFields(control);
      }
    });
  }

  constructor(private _location: Location,
    private formbuilder: FormBuilder,
    private injector: Injector,
    private _dialogService: DialogService,
    private uiLoader: NgxUiLoaderService,
    private _messageDialogService: MessageDialogService,
    private router: Router,
    private localstorageservice: LocalStorageService,
    // private spinner: NgxUiLoaderService,
    private roleService: RoleService,
    private changeDetector: ChangeDetectorRef) {
    router.events.subscribe(e => {
      if (e instanceof NavigationEnd) {
        if (e.url.includes('/roles/Add')) {
          this.roleEditModeOn = false;
          this.allPermisions = false
          this.IsAllRolePrevilegeSelected = false;
          localStorage.removeItem("roleparams");
        }
      }
    });

    if (localStorage.getItem("roleparams")) {
      this.roleEditModeOn = true;
    } else {
      this.roleEditModeOn = false;
    }

    router.events.subscribe(e => {
      if (e instanceof NavigationEnd) {
        if (e.url.includes('/roles')) {
          //set passing parameters to localstorage.
          this.params = JSON.parse(localStorage.getItem('roleparams'));
          if (localStorage.getItem('roleparams')) {
            this.RoleIdentifier = this.params.Routeparams.passingparams.RoleIdentifier
            this.RoleName = this.params.Routeparams.filteredparams.RoleName
          }
        } else {
          localStorage.removeItem("roleparams");
        }
      }
    });
  }

  //function to format data
  onSubmit(): void {
    let privileges = [];
    for (var i = 0; i < this.entityList.length; i++) {
      let roleEntityObj: any = {};
      roleEntityObj.EntityName = this.entityList[i].EntityName;
      let rolePrivileges = [];
      for (var j = 0; j < this.entityList[i].RolePrivilegeOperations.length; j++) {
        if (this.entityList[i].RolePrivilegeOperations[j].IsEnabled) {
          let obj: any = {};
          obj.IsEnabled = true;
          obj.Operation = this.entityList[i].RolePrivilegeOperations[j].Operation;
          rolePrivileges.push(obj);
        }
      }
      roleEntityObj.RolePrivilegeOperations = rolePrivileges;
      privileges.push(roleEntityObj);
    }
    let roleObject: any = {
      Name: this.roleFormGroup.value.roleName,
      Description: this.roleFormGroup.value.roleDescription,
      RolePrivileges: privileges,
      //Status: true,
    }
    if (this.roleEditModeOn && localStorage.getItem('roleparams')) {
      roleObject.Identifier = this.params.Routeparams.passingparams.RoleIdentifier
    }
    this.saveRecord(roleObject);
  }

  //method written to save role
  async saveRecord(roleRecord) {
    this.uiLoader.start();
    let roleArray = [];
    roleArray.push(roleRecord);
    let roleService = this.injector.get(RoleService);
    let isRecordSaved = await roleService.saveRole(roleArray, this.roleEditModeOn);
    this.uiLoader.stop();
    if (isRecordSaved) {
      let message = Constants.recordAddedMessage;
      if (this.roleEditModeOn) {
        message = Constants.recordUpdatedMessage;
      }
      this._messageDialogService.openDialogBox('Success', message, Constants.msgBoxSuccess);
      this.navigateToListPage()
    }
  }

  //This method to fetch role privilege records
  async getRoleEntityRecords() {
    let roleService = this.injector.get(RoleService);
    let searchParameter: any = {};
    searchParameter.PagingParameter = {};
    searchParameter.PagingParameter.PageIndex = Constants.DefaultPageIndex;
    searchParameter.PagingParameter.PageSize = Constants.DefaultPageSize;
    searchParameter.SortParameter = {};
    searchParameter.SortParameter.SortColumn = Constants.EntityName;
    searchParameter.SortParameter.SortOrder = Constants.Ascending;
    searchParameter.SearchMode = Constants.Contains;
    this.isLoaderActive = true;
    this.uiLoader.start();
    this.entityList = await roleService.getRolePrivilleges();

    if (this.roleEditModeOn == true && localStorage.getItem('roleparams')) {
      this.getRoleInfoRecord();
    }
    this.uiLoader.stop();
  }

  // Function call to patch the value at the time of edititing role--
  async getRoleInfoRecord() {
    let roleService = this.injector.get(RoleService);
    let searchParameter: any = {};
    searchParameter.PagingParameter = {};
    searchParameter.PagingParameter.PageIndex = Constants.DefaultPageIndex;
    searchParameter.PagingParameter.PageSize = Constants.DefaultPageSize;
    searchParameter.SortParameter = {};
    searchParameter.SortParameter.SortColumn = Constants.Name;
    searchParameter.SortParameter.SortOrder = Constants.Ascending;
    searchParameter.SearchMode = Constants.Exact;
    searchParameter.IsRequiredRolePrivileges = true;
    if (this.RoleIdentifier != null)
      searchParameter.Identifier = this.RoleIdentifier;
    this.rolRecord = await roleService.getRoles(searchParameter);
    this.rolRecord.forEach(roleObject => {
      this.roleFormGroup.patchValue({
        roleName: roleObject.Name,
        roleDescription: roleObject.Description,
      })
      for (let i = 0; i < this.rolRecord.length; i++) {
        this.checkedRole = this.rolRecord[i].RolePrivileges
      }
      //console.log(this.checkedRole)
      for (let i = 0; i < this.checkedRole.length; i++) {
        for (let count = 0; count < this.entityList.length; count++) {
          if (this.entityList[count].EntityName == this.checkedRole[i].EntityName) {
            for (let innercount = 0; innercount < this.entityList[count].RolePrivilegeOperations.length; innercount++) {
              var counter = 0;
              for (let innercnt = 0; innercnt < this.checkedRole[i].RolePrivilegeOperations.length; innercnt++) {
                if (this.entityList[count].RolePrivilegeOperations[innercount].Operation == this.checkedRole[i].RolePrivilegeOperations[innercnt].Operation) {
                  this.entityList[count].RolePrivilegeOperations[innercount].IsEnabled = true;
                  counter++;
                }
              }
            }
            var dependentEntityObject = this.dependentEntity.filter(x => x.EntityName == this.entityList[count].EntityName);
            if (dependentEntityObject != undefined && dependentEntityObject.length > 0) {
              if (dependentEntityObject[0].DependentEntity != undefined) {
                dependentEntityObject[0].DependentEntity.forEach(dEntity => {
                  this.entityList.forEach(item => {
                    if (item.EntityName == dEntity) {
                      let operationLength = item.RolePrivilegeOperations.length;
                      let counter = 0;

                      item.RolePrivilegeOperations.forEach(previlgeOperationList => {
                        if (previlgeOperationList.Operation == "View") {
                          previlgeOperationList.IsEnabled = true;
                          previlgeOperationList.IsDisabled = true;
                        }
                      });
                      if (counter == operationLength) {
                        item.IsAllRolePrevilegeSelected = true;

                      }
                      else {
                        item.IsAllRolePrevilegeSelected = false;
                      }
                    }
                  });
                });
              }

            }
          }
        }
      }
      for (let count = 0; count < this.entityList.length; count++) {
        var selectOperation = this.entityList[count].RolePrivilegeOperations.filter(x => x.IsEnabled == true);
        if (selectOperation.length == this.entityList[count].RolePrivilegeOperations.length) {
          this.entityList[count].IsAllRolePrevilegeSelected = true;
        }
        else {
          this.entityList[count].IsAllRolePrevilegeSelected = false;
        }
      }
    });


  }

  //navigate to role list
  navigateToListPage() {
    const router = this.injector.get(Router);
    router.navigate(['roles']);
  }

  //initialization call
  ngOnInit() {
    //Validations for Role Form.
    this.roleFormGroup = this.formbuilder.group({
      roleName: [null, Validators.compose([Validators.required,
      Validators.minLength(Constants.inputMinLenth), Validators.maxLength(Constants.inputMaxLenth),
      Validators.pattern(this.onlyAlphabetswithInbetweenSpaceUpto50Characters)])
      ],
      roleDescription: [null, Validators.compose([Validators.maxLength(Constants.txtAreaMaxLenth)])]
    });
    this.permissonFilterForm = this.formbuilder.group(
      { filterPermission: [null], }
    );
    this.getRoleEntityRecords();
  }

  selectAllPermissions(event) {
    this.allPermisions = event.target.checked;
    this.entityList.forEach(rolelist => {
      rolelist.IsAllRolePrevilegeSelected = event.target.checked
      rolelist.RolePrivilegeOperations.forEach(previlgeOperationList => {
        previlgeOperationList.IsEnabled = event.target.checked;
      })
    })
  }

  selectAllEntityOperations(event, entityName) {
    this.entityList.forEach(entity => {
      if (entity.EntityName == entityName) {
        entity.RolePrivilegeOperations.forEach(previlgeOperationList => {
          previlgeOperationList.IsEnabled = event.target.checked;
        });
      }
      if (event.target.checked) {
        var dependentEntityObject = this.dependentEntity.filter(x => x.EntityName == entityName);
        if (dependentEntityObject != undefined && dependentEntityObject.length > 0) {
          dependentEntityObject[0].DependentEntity.forEach(dEntity => {

            this.entityList.forEach(item => {
              if (item.EntityName == dEntity) {
                let operationLength = item.RolePrivilegeOperations.length;
                let counter = 0;

                item.RolePrivilegeOperations.forEach(previlgeOperationList => {
                  if (previlgeOperationList.Operation == "View") {
                    previlgeOperationList.IsEnabled = true;
                    previlgeOperationList.IsDisabled = true;
                  }
                });
                if (counter == operationLength) {
                  item.IsAllRolePrevilegeSelected = true;

                }
                else {
                  item.IsAllRolePrevilegeSelected = false;
                }
              }
            });
          });
        }
      }
      if (!event.target.checked) {
        var dependentEntityObject = this.dependentEntity.filter(x => x.EntityName == entityName);
        if (dependentEntityObject != undefined && dependentEntityObject.length > 0) {
          dependentEntityObject[0].DependentEntity.forEach(dEntity => {

            this.entityList.forEach(item => {
              if (item.EntityName == dEntity) {
                let operationLength = item.RolePrivilegeOperations.length;
                let counter = 0;

                item.RolePrivilegeOperations.forEach(previlgeOperationList => {
                  if (previlgeOperationList.Operation == "View") {

                    previlgeOperationList.IsDisabled = false;
                  }
                })
              }
            });
          });
        }
      }
    })

  }

  rowWiseSelection(event, operation, entityName) {
    this.entityList.forEach(entity => {
      if (entity.EntityName == entityName) {
        let operationLength = entity.RolePrivilegeOperations.length;
        let counter = 0;
        entity.RolePrivilegeOperations.forEach(previlgeOperationList => {
          if (previlgeOperationList.IsEnabled == true) {
            counter++;
          }
          // previlgeOperationList.IsEnabled = event.target.checked;
        })
        if (counter == operationLength) {
          entity.IsAllRolePrevilegeSelected = true;

        }
        else {
          entity.IsAllRolePrevilegeSelected = false;
        }
      }
    });

    if (operation.Operation == "Create" || operation.Operation == "Edit" || entityName == "Dashboard" || entityName == "Analytics" || entityName == "Log") {
      if (operation.IsEnabled) {
        var dependentEntityObject = this.dependentEntity.filter(x => x.EntityName == entityName);
        if (dependentEntityObject != undefined && dependentEntityObject.length > 0) {
          if (dependentEntityObject[0].DependentEntity != undefined) {
            dependentEntityObject[0].DependentEntity.forEach(dEntity => {

              this.entityList.forEach(item => {
                if (item.EntityName == dEntity) {
                  let operationLength = item.RolePrivilegeOperations.length;

                  item.RolePrivilegeOperations.forEach(previlgeOperationList => {
                    if (previlgeOperationList.Operation == "View") {
                      previlgeOperationList.IsEnabled = true;
                      previlgeOperationList.IsDisabled = true;
                    }
                  });
                  var selectedItem = item.RolePrivilegeOperations.filter(i => i.IsEnabled == true);
                  if (selectedItem.length == operationLength) {
                    item.IsAllRolePrevilegeSelected = true;
                  }
                  else {
                    item.IsAllRolePrevilegeSelected = false;
                  }
                }
              });
            });
          }
        }
      }
      if (!operation.IsEnabled) {
        var dependentEntityObject = this.dependentEntity.filter(x => x.EntityName == entityName);
        if (dependentEntityObject != undefined && dependentEntityObject.length > 0) {
          if (dependentEntityObject[0].DependentEntity != undefined) {
            dependentEntityObject[0].DependentEntity.forEach(dEntity => {

              this.entityList.forEach(item => {
                if (item.EntityName == dEntity) {
                  let operationLength = item.RolePrivilegeOperations.length;
                  let counter = 0;

                  item.RolePrivilegeOperations.forEach(previlgeOperationList => {
                    if (previlgeOperationList.Operation == "View") {

                      previlgeOperationList.IsDisabled = false;
                    }
                  })
                }
              });
            });
          }
        }
      }
    }

  }




  saveBtnValidation(): boolean {
    if (this.roleFormGroup.controls.roleName.invalid) {
      return true;
    }
    if (this.roleFormGroup.controls.roleDescription.invalid) {
      return true;
    }
    return false;
  }

}
