import { Component, OnInit, Injector, ChangeDetectorRef } from '@angular/core';
import { Location } from '@angular/common';
import { Constants } from 'src/app/shared/constants/constants';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';
import { RoleService } from '../role.service';
import { Router, NavigationEnd } from '@angular/router';
import { NgxUiLoaderService } from 'ngx-ui-loader';
import { MessageDialogService } from 'src/app/shared/services/mesage-dialog.service';
import { DialogService } from '@tomblue/ng2-bootstrap-modal';
import { LocalStorageService } from 'src/app/shared/services/local-storage.service';
import { RolePrivilege } from '../../../shared/models/rolePrivilege';
import { RoleprivilegeMapping } from '../role';
import { UserService } from '../../users/user.service';

@Component({
  selector: 'app-add',
  templateUrl: './add.component.html',
  styleUrls: ['./add.component.scss']
})
export class AddComponent implements OnInit {

  //variable declaration here
  public isCollapsedRoles: boolean = true;
  public isCollapsedDetails: boolean = false;
  public isCollapsedUsers: boolean = true;
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
  public IsUserDetailsGet = false;
  //regex
  public onlyAlphabetsWithSpace = '[a-zA-Z ]*';
  public onlyAlphabetswithInbetweenSpaceUpto50Characters = Constants.onlyAlphabetswithInbetweenSpaceUpto50Characters;
  public rolePrivilegesList = [];
  public entityList: RolePrivilege[] = [];
  public roleAddEditResources = {}
  public ResourceLoadingFailedMsg = Constants.ResourceLoadingFailedMsg;
  public allPermisions: boolean = false
  public allPermisionsDisabled: boolean = false

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
  public usersList = [];
  public Name;
  public dependentEntityCount: any = [
    {
      "EntityName": "Dashboard",
      "Count": 0,

    },
    {
      "EntityName": "User",
      "Count": 0,

    },
    {
      "EntityName": "Role",
      "Count": 0,

    },
    {
      "EntityName": "Asset Library",
      "Count": 0,
    },
    {
      "EntityName": "Widget",
      "Count": 0,
    },
    {
      "EntityName": "Dynamic Widget",
      "Count": 0,
    },
    {
      "EntityName": "Page",
      "Count": 0,
    },
    {
      "EntityName": "Statement Definition",
      "Count": 0,
    },
    {
      "EntityName": "Schedule Management",
      "Count": 0,
    },
    {
      "EntityName": "Log",
      "Count": 0,
    },
    {
      "EntityName": "Analytics",
      "Count": 0,
    },
    {
      "EntityName": "Statement Search",
      "Count": 0,
    }
  ];

  public dependentEntityMap: RoleprivilegeMapping[] = [
    {
      "EntityName": "Dashboard",
      "Operation": "View",
      "RelatedOperation": [],
      "OtherDependentEntity": ["Page", "Schedule Management", "Log"]
    },
    {
      "EntityName": "User",
      "Operation": "Create",
      "RelatedOperation": ["Edit", "Delete", "View"],
      "OtherDependentEntity": ["Role"]
    },
    {
      "EntityName": "User",
      "Operation": "Edit",
      "RelatedOperation": ["Delete", "View"],
      "OtherDependentEntity": ["Role"]
    },
    {
      "EntityName": "User",
      "Operation": "Delete",
      "RelatedOperation": ["View"],
      "OtherDependentEntity": []
    },
    {
      "EntityName": "User",
      "Operation": "View",
      "RelatedOperation": [],
      "OtherDependentEntity": []
    },
    {
      "EntityName": "User",
      "Operation": "Reset Password",
      "RelatedOperation": ["View"],
      "OtherDependentEntity": []
    },
    {
      "EntityName": "Role",
      "Operation": "Create",
      "RelatedOperation": ["Edit", "Delete", "View"],
      "OtherDependentEntity": []
    },
    {
      "EntityName": "Role",
      "Operation": "Edit",
      "RelatedOperation": ["Delete", "View"],
      "OtherDependentEntity": []
    },
    {
      "EntityName": "Role",
      "Operation": "Delete",
      "RelatedOperation": ["View"],
      "OtherDependentEntity": []
    },
    {
      "EntityName": "Role",
      "Operation": "View",
      "RelatedOperation": [],
      "OtherDependentEntity": []
    },
    {
      "EntityName": "Asset Library",
      "Operation": "Create",
      "RelatedOperation": ["Edit", "Delete", "View"],
      "OtherDependentEntity": []
    },
    {
      "EntityName": "Asset Library",
      "Operation": "Edit",
      "RelatedOperation": ["Delete", "View"],
      "OtherDependentEntity": []
    },
    {
      "EntityName": "Asset Library",
      "Operation": "Delete",
      "RelatedOperation": ["View"],
      "OtherDependentEntity": []
    },
    {
      "EntityName": "Asset Library",
      "Operation": "View",
      "RelatedOperation": [],
      "OtherDependentEntity": []
    },
    {
      "EntityName": "Widget",
      "Operation": "View",
      "RelatedOperation": [],
      "OtherDependentEntity": []
    },
    {
      "EntityName": "Page",
      "Operation": "Create",
      "RelatedOperation": ["Edit", "Delete", "View"],
      "OtherDependentEntity": ["Asset Library", "Widget","Dynamic Widget"]
    },
    {
      "EntityName": "Page",
      "Operation": "Edit",
      "RelatedOperation": ["Delete", "View"],
      "OtherDependentEntity": ["Asset Library", "Widget", "Dynamic Widget"]
    },
    {
      "EntityName": "Page",
      "Operation": "Delete",
      "RelatedOperation": ["View"],
      "OtherDependentEntity": []
    },
    {
      "EntityName": "Page",
      "Operation": "View",
      "RelatedOperation": [],
      "OtherDependentEntity": []
    },
    {
      "EntityName": "Page",
      "Operation": "Publish",
      "RelatedOperation": ["View"],
      "OtherDependentEntity": []
    },
    {
      "EntityName": "Statement Definition",
      "Operation": "Create",
      "RelatedOperation": ["Edit", "Delete", "View"],
      "OtherDependentEntity": ["Page"]
    },
    {
      "EntityName": "Statement Definition",
      "Operation": "Edit",
      "RelatedOperation": ["Delete", "View"],
      "OtherDependentEntity": ["Page"]
    },
    {
      "EntityName": "Statement Definition",
      "Operation": "Delete",
      "RelatedOperation": ["View"],
      "OtherDependentEntity": []
    },
    {
      "EntityName": "Statement Definition",
      "Operation": "View",
      "RelatedOperation": [],
      "OtherDependentEntity": []
    },
    {
      "EntityName": "Statement Definition",
      "Operation": "Publish",
      "RelatedOperation": ["View"],
      "OtherDependentEntity": []
    },
    {
      "EntityName": "Schedule Management",
      "Operation": "Create",
      "RelatedOperation": ["Edit", "Delete", "View"],
      "OtherDependentEntity": ["Statement Definition"]
    },
    {
      "EntityName": "Schedule Management",
      "Operation": "Edit",
      "RelatedOperation": ["Delete", "View"],
      "OtherDependentEntity": ["Statement Definition"]
    },
    {
      "EntityName": "Schedule Management",
      "Operation": "Delete",
      "RelatedOperation": ["View"],
      "OtherDependentEntity": []
    },
    {
      "EntityName": "Schedule Management",
      "Operation": "View",
      "RelatedOperation": [],
      "OtherDependentEntity": []
    },
    {
      "EntityName": "Log",
      "Operation": "View",
      "RelatedOperation": [],
      "OtherDependentEntity": ["Statement Definition", "User"]
    },
    {
      "EntityName": "Analytics",
      "Operation": "View",
      "RelatedOperation": [],
      "OtherDependentEntity": ["Page", "Widget"]
    },
    {
      "EntityName": "Statement Search",
      "Operation": "View",
      "RelatedOperation": [],
      "OtherDependentEntity": ["Statement Definition"]
    },
    {
      "EntityName": "Dynamic Widget",
      "Operation": "Create",
      "RelatedOperation": ["Edit", "Delete", "View"],
      "OtherDependentEntity": ["Asset Library",]
    },
    {
      "EntityName": "Dynamic Widget",
      "Operation": "Edit",
      "RelatedOperation": ["Delete", "View"],
      "OtherDependentEntity": ["Asset Library",]
    },
    {
      "EntityName": "Dynamic Widget",
      "Operation": "Delete",
      "RelatedOperation": ["View"],
      "OtherDependentEntity": []
    },
    {
      "EntityName": "Dynamic Widget",
      "Operation": "View",
      "RelatedOperation": [],
      "OtherDependentEntity": []
    },
    {
      "EntityName": "Dynamic Widget",
      "Operation": "Publish",
      "RelatedOperation": ["View"],
      "OtherDependentEntity": []
    },
  ];

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

  constructor(private formbuilder: FormBuilder,
    private injector: Injector,
    private uiLoader: NgxUiLoaderService,
    private _messageDialogService: MessageDialogService,
    private router: Router,
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
    var response = await roleService.getRoles(searchParameter);
    this.rolRecord = response.roleList;
    this.Name = this.rolRecord[0].Name;
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
                  this.entityList[count].RolePrivilegeOperations[innercount].IsDisabled = false;
                  counter++;
                }
              }
            }
          }
        }
      }
      this.entityList.forEach(entity => {
        entity.RolePrivilegeOperations.forEach(operation => {
          var entityMapping = this.dependentEntityMap.filter(x => x.EntityName == entity.EntityName && x.Operation == operation.Operation)[0];
          if (operation.IsEnabled) {
            if (entity.EntityName == entity.EntityName) {
              //check and disable entity related operations
              entity.RolePrivilegeOperations.forEach(opt => {
                if (entityMapping.RelatedOperation != undefined) {
                  var filterOps = entityMapping.RelatedOperation.filter(op => op == opt.Operation);
                  if (filterOps != undefined && filterOps.length > 0) {
                    opt.IsDisabled = true;
                    opt.IsEnabled = true;
                  }
                }
              });
            }
            if (entityMapping.OtherDependentEntity != undefined) {
              //check and other dependent entity view operations
              var filterEntity = entityMapping.OtherDependentEntity;
              if (filterEntity != undefined && filterEntity.length > 0) {
                filterEntity.forEach(fE => {
                  this.entityList.forEach(x => {
                    if (x.EntityName == fE) {
                      x.RolePrivilegeOperations.forEach(opt => {
                        if (opt.Operation == "View") {
                          opt.IsDisabled = true;
                          opt.IsEnabled = true;
                        }
                      });
                      this.dependentEntityCount.forEach(en => {
                        if (en.EntityName == x.EntityName) {
                          en.Count = en.Count + 1;
                        }
                      })
                    }
                  })
                })
              }
            }
          }
        });
      });

      for (let count = 0; count < this.entityList.length; count++) {
        var selectOperation = this.entityList[count].RolePrivilegeOperations.filter(x => x.IsEnabled == true);
        if (selectOperation.length == this.entityList[count].RolePrivilegeOperations.length) {
          this.entityList[count].IsAllRolePrevilegeSelected = true;
        }
        else {
          this.entityList[count].IsAllRolePrevilegeSelected = false;
        }
      }
      this.entityList.forEach(entity => {
        var disbaledList = entity.RolePrivilegeOperations.filter(item => item.IsDisabled == true);
        if (disbaledList != undefined && disbaledList.length > 0) {
          entity.IsAllRolePrevilegeSelectedDisabled = true
        }
        else {
          entity.IsAllRolePrevilegeSelectedDisabled = false;

        }
      });
      var allSelectedList = this.entityList.filter(item => item.IsAllRolePrevilegeSelected == true);
      if (allSelectedList != undefined && allSelectedList.length > 0) {
        if (this.entityList.length == allSelectedList.length)
          this.allPermisions = true
      }
      else {
        this.allPermisions = false;

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
    this.entityList.forEach(entity => {
      entity.IsAllRolePrevilegeSelected = event.target.checked
      entity.RolePrivilegeOperations.forEach(previlgeOperationList => {
        previlgeOperationList.IsEnabled = event.target.checked;
        previlgeOperationList.IsDisabled = false;
      });

      entity.RolePrivilegeOperations.forEach(operation => {
        var entityMapping = this.dependentEntityMap.filter(x => x.EntityName == entity.EntityName && x.Operation == operation.Operation)[0];
        if (event.target.checked) {
          if (entity.EntityName == entity.EntityName) {
            //check and disable entity related operations
            entity.RolePrivilegeOperations.forEach(opt => {
              if (entityMapping.RelatedOperation != undefined) {
                var filterOps = entityMapping.RelatedOperation.filter(op => op == opt.Operation);
                if (filterOps != undefined && filterOps.length > 0) {
                  opt.IsDisabled = true;
                  opt.IsEnabled = true;
                }
              }
            });
          }
          if (entityMapping.OtherDependentEntity != undefined) {
            //check and other dependent entity view operations
            var filterEntity = entityMapping.OtherDependentEntity;
            if (filterEntity != undefined && filterEntity.length > 0) {
              filterEntity.forEach(fE => {
                this.entityList.forEach(x => {
                  if (x.EntityName == fE) {
                    x.RolePrivilegeOperations.forEach(opt => {
                      if (opt.Operation == "View") {
                        opt.IsDisabled = true;
                        opt.IsEnabled = true;
                      }
                    });
                    this.dependentEntityCount.forEach(en => {
                      if (en.EntityName == x.EntityName) {
                        en.Count = en.Count + 1;
                      }
                    })
                  }
                })
              })
            }
          }
        }
        else {
          if (entity.EntityName == entity.EntityName) {
            //check and disable entity related operations
            entity.RolePrivilegeOperations.forEach(opt => {
              if (entityMapping.RelatedOperation != undefined) {
                var filterOps = entityMapping.RelatedOperation.filter(op => op == opt.Operation);
                if (filterOps != undefined && filterOps.length > 0) {
                  if (opt.Operation == "Edit" && operation.Operation == "Create") {
                    opt.IsDisabled = false;
                  }
                  else if (opt.Operation == "Delete" && operation.Operation == "Edit") {
                    opt.IsDisabled = false;
                  }
                  else if (opt.Operation == "View" && operation.Operation == "Delete") {
                    opt.IsDisabled = false;
                  }
                  else if (opt.Operation == "View" && operation.Operation == "Publish") {
                    opt.IsDisabled = false;
                  }
                }
              }
            });
          }

          if (entityMapping.OtherDependentEntity != undefined) {
            //check and other dependent entity view operations

            var filterEntity = entityMapping.OtherDependentEntity.filter(en => en == entity.EntityName);
            if (filterEntity != undefined && filterEntity.length > 0) {
              var filterEntityCount = this.dependentEntityCount.filter(en => en.EntityName == entity.EntityName);
              if (filterEntityCount != undefined && filterEntityCount.length > 0) {
                if (filterEntityCount[0].Count == 1) {
                  entity.RolePrivilegeOperations.forEach(opt => {
                    if (opt.Operation == "View") {
                      opt.IsDisabled = false;
                      this.dependentEntityCount.forEach(en => {
                        if (en.EntityName == entity.EntityName) {
                          en.Count = 0;
                        }
                      })
                    }
                  });
                }
                else {
                  this.dependentEntityCount.forEach(en => {
                    if (en.EntityName == entity.EntityName) {
                      en.Count = en.Count - 1;
                    }
                  })
                }
              }
            }
          }

        }
      });
    });

    this.entityList.forEach(entity => {
      var enableList = entity.RolePrivilegeOperations.filter(item => item.IsEnabled == true);
      if (enableList.length == entity.RolePrivilegeOperations.length) {
        entity.IsAllRolePrevilegeSelected = true;
      }
      else {
        entity.IsAllRolePrevilegeSelected = false;
      }

      var disbaledList = entity.RolePrivilegeOperations.filter(item => item.IsDisabled == true);
      if (disbaledList != undefined && disbaledList.length > 0) {
        entity.IsAllRolePrevilegeSelectedDisabled = true;
      }
      else {
        entity.IsAllRolePrevilegeSelectedDisabled = false;
      }
    });
    var allSelectedList = this.entityList.filter(item => item.IsAllRolePrevilegeSelected == true);
    if (allSelectedList != undefined && allSelectedList.length > 0) {
      if (this.entityList.length == allSelectedList.length)
        this.allPermisions = true
    }
    else {
      this.allPermisions = false;
    }
  }

  selectAllEntityOperations(event, entityName) {
    this.entityList.forEach(entity => {
      if (entity.EntityName == entityName) {
        entity.RolePrivilegeOperations.forEach(previlgeOperationList => {
          previlgeOperationList.IsEnabled = event.target.checked;
        });

        entity.RolePrivilegeOperations.forEach(operation => {
          var entityMapping = this.dependentEntityMap.filter(x => x.EntityName == entityName && x.Operation == operation.Operation)[0];
          if (event.target.checked) {
            this.entityList.forEach(entity => {
              if (entity.EntityName == entityName) {
                //check and disable entity related operations
                entity.RolePrivilegeOperations.forEach(opt => {
                  if (entityMapping.RelatedOperation != undefined) {
                    var filterOps = entityMapping.RelatedOperation.filter(op => op == opt.Operation);
                    if (filterOps != undefined && filterOps.length > 0) {
                      opt.IsDisabled = true;
                      opt.IsEnabled = true;
                    }
                  }
                });
              }
              if (entityMapping.OtherDependentEntity != undefined) {
                //check and other dependent entity view operations
                var filterEntity = entityMapping.OtherDependentEntity.filter(en => en == entity.EntityName);
                if (filterEntity != undefined && filterEntity.length > 0) {
                  this.entityList.forEach(x => {
                    if (x.EntityName == filterEntity[0]) {
                      x.RolePrivilegeOperations.forEach(opt => {
                        if (opt.Operation == "View") {
                          opt.IsDisabled = true;
                          opt.IsEnabled = true;
                        }
                      });
                      this.dependentEntityCount.forEach(en => {
                        if (en.EntityName == entity.EntityName) {
                          en.Count = en.Count + 1;
                        }
                      })
                    }
                  })
                }
              }
            })
          }
          else {
            this.entityList.forEach(entity => {
              if (entity.EntityName == entityName) {
                //check and disable entity related operations
                entity.RolePrivilegeOperations.forEach(opt => {
                  if (entityMapping.RelatedOperation != undefined) {
                    var filterOps = entityMapping.RelatedOperation.filter(op => op == opt.Operation);
                    if (filterOps != undefined && filterOps.length > 0) {
                      if (opt.Operation == "Edit" && operation.Operation == "Create") {
                        opt.IsDisabled = false;
                      }
                      else if (opt.Operation == "Delete" && operation.Operation == "Edit") {
                        opt.IsDisabled = false;
                      }
                      else if (opt.Operation == "View" && operation.Operation == "Delete") {
                        opt.IsDisabled = false;
                      }
                      else if (opt.Operation == "View" && operation.Operation == "Publish") {
                        opt.IsDisabled = false;
                      }
                      else if (opt.Operation == "View" && operation.Operation == "Delete") {
                        //check if publish is selected
                        var isPublishSelected = entity.RolePrivilegeOperations.filter(e => e.Operation == "Publish" && e.IsEnabled == true);
                        if (isPublishSelected == undefined || isPublishSelected.length == 0) {
                          opt.IsDisabled = false;
                        }

                      }
                      else if (opt.Operation == "View" && operation.Operation == "Publish") {
                        var isDeleteSelected = entity.RolePrivilegeOperations.filter(e => e.Operation == "Delete" && e.IsEnabled == true);
                        if (isDeleteSelected == undefined || isDeleteSelected.length == 0) {
                          opt.IsDisabled = false;
                        }
                      }
                    }
                  }
                });
              }

              if (entityMapping.OtherDependentEntity != undefined) {
                //check and other dependent entity view operations
                var filterEntity = entityMapping.OtherDependentEntity.filter(en => en == entity.EntityName);
                if (filterEntity != undefined && filterEntity.length > 0) {
                  var filterEntityCount = this.dependentEntityCount.filter(en => en.EntityName == entity.EntityName);
                  if (filterEntityCount != undefined && filterEntityCount.length > 0) {
                    if (filterEntityCount[0].Count == 1) {
                      entity.RolePrivilegeOperations.forEach(opt => {
                        if (opt.Operation == "View") {
                          opt.IsDisabled = false;
                          this.dependentEntityCount.forEach(en => {
                            if (en.EntityName == entity.EntityName) {
                              en.Count = 0;
                            }
                          })
                        }
                      });
                    }
                    else {
                      this.dependentEntityCount.forEach(en => {
                        if (en.EntityName == entity.EntityName) {
                          en.Count = en.Count - 1;
                        }
                      })
                    }
                  }
                }
              }
            })
          }
        });

      }
    });

    this.entityList.forEach(entity => {
      var enableList = entity.RolePrivilegeOperations.filter(item => item.IsEnabled == true);
      if (enableList.length == entity.RolePrivilegeOperations.length) {
        entity.IsAllRolePrevilegeSelected = true;
      }
      else {
        entity.IsAllRolePrevilegeSelected = false;
      }

      var disbaledList = entity.RolePrivilegeOperations.filter(item => item.IsDisabled == true);
      if (disbaledList != undefined && disbaledList.length > 0) {
        entity.IsAllRolePrevilegeSelectedDisabled = true;
      }
      else {
        entity.IsAllRolePrevilegeSelectedDisabled = false;
      }
    });
    var allSelectedList = this.entityList.filter(item => item.IsAllRolePrevilegeSelected == true);
    if (allSelectedList != undefined && allSelectedList.length > 0) {
      if (this.entityList.length == allSelectedList.length)
        this.allPermisions = true
      else {
        this.allPermisions = false;
      }
    }
    else {
      this.allPermisions = false;
    }
  }

  rowWiseSelection(event, operation, entityName) {
    var entityMapping = this.dependentEntityMap.filter(x => x.EntityName == entityName && x.Operation == operation.Operation)[0];
    if (event.target.checked) {
      this.entityList.forEach(entity => {
        if (entity.EntityName == entityName) {
          //check and disable entity related operations
          entity.RolePrivilegeOperations.forEach(opt => {
            if (entityMapping.RelatedOperation != undefined) {
              var filterOps = entityMapping.RelatedOperation.filter(op => op == opt.Operation);
              if (filterOps != undefined && filterOps.length > 0) {
                opt.IsDisabled = true;
                opt.IsEnabled = true;
              }
            }
          });
        }
        if (entityMapping.OtherDependentEntity != undefined) {
          //check and other dependent entity view operations
          var filterEntity = entityMapping.OtherDependentEntity.filter(en => en == entity.EntityName);
          if (filterEntity != undefined && filterEntity.length > 0) {
            this.entityList.forEach(x => {
              if (x.EntityName == filterEntity[0]) {
                x.RolePrivilegeOperations.forEach(opt => {
                  if (opt.Operation == "View") {
                    opt.IsDisabled = true;
                    opt.IsEnabled = true;
                  }
                });
                this.dependentEntityCount.forEach(en => {
                  if (en.EntityName == x.EntityName) {
                    if (operation.Operation == "Create") {
                      en.Count = en.Count + 2;
                    }
                    else {
                      en.Count = en.Count + 1;
                    }

                  }
                })
              }
            })

          }
        }
      })
    }
    else {
      this.entityList.forEach(entity => {
        if (entity.EntityName == entityName) {
          //check and disable entity related operations
          entity.RolePrivilegeOperations.forEach(opt => {
            if (entityMapping.RelatedOperation != undefined) {
              var filterOps = entityMapping.RelatedOperation.filter(op => op == opt.Operation);
              if (filterOps != undefined && filterOps.length > 0) {
                if (opt.Operation == "Edit" && operation.Operation == "Create") {
                  opt.IsDisabled = false;
                }
                else if (opt.Operation == "Delete" && operation.Operation == "Edit") {
                  opt.IsDisabled = false;
                }
                else if (opt.Operation == "View" && operation.Operation == "Delete") {
                  //check if publish is selected
                  var isPublishSelected = entity.RolePrivilegeOperations.filter(e => e.Operation == "Publish" && e.IsEnabled == true);
                  if (isPublishSelected == undefined || isPublishSelected.length == 0) {
                    opt.IsDisabled = false;
                  }
                }
                else if (opt.Operation == "View" && operation.Operation == "Publish") {
                  var isDeleteSelected = entity.RolePrivilegeOperations.filter(e => e.Operation == "Delete" && e.IsEnabled == true);
                  if (isDeleteSelected == undefined || isDeleteSelected.length == 0) {
                    opt.IsDisabled = false;
                  }
                }
                else if (opt.Operation == "View" && operation.Operation == "Reset Password") {
                  opt.IsDisabled = false;
                }
              }
            }
          });
        }

        if (entityMapping.OtherDependentEntity != undefined) {
          //check and other dependent entity view operations

          var filterEntity = entityMapping.OtherDependentEntity.filter(en => en == entity.EntityName);
          if (filterEntity != undefined && filterEntity.length > 0) {
            var filterEntityCount = this.dependentEntityCount.filter(en => en.EntityName == entity.EntityName);
            if (filterEntityCount != undefined && filterEntityCount.length > 0) {
              if (filterEntityCount[0].Count == 1) {
                var entities = entity.RolePrivilegeOperations.filter(e => (e.Operation == "Edit" || e.Operation == "Create" || e.Operation == "Delete") && e.IsEnabled == true);
                if (entities == undefined || entities.length == 0) {
                  entity.RolePrivilegeOperations.forEach(opt => {
                    if (opt.Operation == "View") {
                      opt.IsDisabled = false;
                    }
                    this.dependentEntityCount.forEach(en => {
                      if (en.EntityName == entity.EntityName) {
                        en.Count = 0;
                      }
                    })
                  });
                }
                else {
                  this.dependentEntityCount.forEach(en => {
                    if (en.EntityName == entity.EntityName) {
                      en.Count = 0;
                    }
                  })
                }
                
              }
              else {
                this.dependentEntityCount.forEach(en => {
                  if (en.EntityName == entity.EntityName) {
                    en.Count = en.Count - 1;
                  }
                })
              }
            }
          }
        }
      })
    }

    this.entityList.forEach(entity => {
      var enableList = entity.RolePrivilegeOperations.filter(item => item.IsEnabled == true);
      if (enableList.length == entity.RolePrivilegeOperations.length) {
        entity.IsAllRolePrevilegeSelected = true;
      }
      else {
        entity.IsAllRolePrevilegeSelected = false;
      }

      var disbaledList = entity.RolePrivilegeOperations.filter(item => item.IsDisabled == true);
      if (disbaledList != undefined && disbaledList.length > 0) {
        entity.IsAllRolePrevilegeSelectedDisabled = true;
      }
      else {
        entity.IsAllRolePrevilegeSelectedDisabled = false;
      }
    });
    var allSelectedList = this.entityList.filter(item => item.IsAllRolePrevilegeSelected == true);
    if (allSelectedList != undefined && allSelectedList.length > 0) {
      if (this.entityList.length == allSelectedList.length)
        this.allPermisions = true
      else {
        this.allPermisions = false;
      }
    }
    else {
      this.allPermisions = false;
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

  async getRoleUser() {
    this.isCollapsedUsers = !this.isCollapsedUsers
    if (this.IsUserDetailsGet==false) {
      let userService = this.injector.get(UserService);
      let searchParameter: any = {};
      searchParameter.IsRolePrivilegesRequired = true;
      searchParameter.RoleIdentifier = this.RoleIdentifier;
      searchParameter.PagingParameter = {};
      searchParameter.PagingParameter.PageIndex = Constants.DefaultPageIndex;
      searchParameter.PagingParameter.PageSize = Constants.DefaultPageSize;
      searchParameter.SortParameter = {};
      searchParameter.SortParameter.SortColumn = "Id";
      searchParameter.SortParameter.SortOrder = Constants.Ascending;
      searchParameter.SearchMode = Constants.Contains;
      searchParameter.ActivationStatus = true;
      //this.spinner.start();
      this.usersList = await userService.getUser(searchParameter);
      this.IsUserDetailsGet = true;
    }
    

  }

}
