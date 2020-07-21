"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
//export var RolePrivilegeDummyList: RolePrivilege[] = [
//  {
//    "EntityName": "Alert",
//    "DisplayName": "Alert",
//    "RolePrivilegeOperations": [
//      {
//        "Operation": "View",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Acknowledge",
//        "EntityName": "",
//        "IsEnabled": false
//      }
//    ]
//  },
//  {
//    "EntityName": "AlertRule",
//    "DisplayName": "AlertRule",
//    "RolePrivilegeOperations": [
//      {
//        "Operation": "Create",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "View",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Delete",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Edit",
//        "EntityName": "",
//        "IsEnabled": false
//      }
//    ]
//  },
//  {
//    "EntityName": "AnnealingTest",
//    "DisplayName": "Annealing Record",
//    "RolePrivilegeOperations": [
//      {
//        "Operation": "Create",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Edit",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Delete",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "View",
//        "EntityName": "",
//        "IsEnabled": false
//      }
//    ]
//  },
//  {
//    "EntityName": "AQL",
//    "DisplayName": "AQL",
//    "RolePrivilegeOperations": [
//      {
//        "Operation": "Create",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Edit",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Delete",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "View",
//        "EntityName": "",
//        "IsEnabled": false
//      }
//    ]
//  },
//  {
//    "EntityName": "BrustingTest",
//    "DisplayName": "Brusting Test",
//    "RolePrivilegeOperations": [
//      {
//        "Operation": "Create",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Edit",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Delete",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "View",
//        "EntityName": "",
//        "IsEnabled": false
//      }
//    ]
//  },
//  {
//    "EntityName": "CECoatingTest",
//    "DisplayName": "CE Coating Test",
//    "RolePrivilegeOperations": [
//      {
//        "Operation": "Create",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Edit",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Delete",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "View",
//        "EntityName": "",
//        "IsEnabled": false
//      }
//    ]
//  },
//  {
//    "EntityName": "CESetOut",
//    "DisplayName": "CE Set Outs",
//    "RolePrivilegeOperations": [
//      {
//        "Operation": "View",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Edit",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Create",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Delete",
//        "EntityName": "",
//        "IsEnabled": false
//      }
//    ]
//  },
//  {
//    "EntityName": "ChemicalLabSampling",
//    "DisplayName": "Chemical Lab Sampling",
//    "RolePrivilegeOperations": [
//      {
//        "Operation": "Create",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Edit",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Delete",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "View",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "SyncWithSAP",
//        "EntityName": "",
//        "IsEnabled": false
//      }
//    ]
//  },
//  {
//    "EntityName": "ChemicalLabSelection",
//    "DisplayName": "Chemical Lab Selection",
//    "RolePrivilegeOperations": [
//      {
//        "Operation": "Create",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Edit",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Delete",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "View",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "SyncWithSAP",
//        "EntityName": "",
//        "IsEnabled": false
//      }
//    ]
//  },
//  {
//    "EntityName": "CompareSetOutReport",
//    "DisplayName": "Compare Setouts Report",
//    "RolePrivilegeOperations": [
//      {
//        "Operation": "View",
//        "EntityName": "",
//        "IsEnabled": false
//      }
//    ]
//  },
//  {
//    "EntityName": "Configuration",
//    "DisplayName": "Configuration",
//    "RolePrivilegeOperations": [
//      {
//        "Operation": "Create",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "View",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Delete",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Edit",
//        "EntityName": "",
//        "IsEnabled": false
//      }
//    ]
//  },
//  {
//    "EntityName": "Customer",
//    "DisplayName": "Customer",
//    "RolePrivilegeOperations": [
//      {
//        "Operation": "Create",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Edit",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Delete",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "View",
//        "EntityName": "",
//        "IsEnabled": false
//      }
//    ]
//  },
//  {
//    "EntityName": "CustomerFeedback",
//    "DisplayName": "Customer Feedback",
//    "RolePrivilegeOperations": [
//      {
//        "Operation": "Create",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Edit",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Delete",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "View",
//        "EntityName": "",
//        "IsEnabled": false
//      }
//    ]
//  },
//  {
//    "EntityName": "DailyJobReport",
//    "DisplayName": "Daily Job Report",
//    "RolePrivilegeOperations": [
//      {
//        "Operation": "View",
//        "EntityName": "",
//        "IsEnabled": false
//      }
//    ]
//  },
//  {
//    "EntityName": "DailyMIS",
//    "DisplayName": "Daily MIS",
//    "RolePrivilegeOperations": [
//      {
//        "Operation": "View",
//        "EntityName": "",
//        "IsEnabled": false
//      }
//    ]
//  },
//  {
//    "EntityName": "Defect",
//    "DisplayName": "Defects",
//    "RolePrivilegeOperations": [
//      {
//        "Operation": "Create",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Edit",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Delete",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "View",
//        "EntityName": "",
//        "IsEnabled": false
//      }
//    ]
//  },
//  {
//    "EntityName": "DefectCategory",
//    "DisplayName": "Defect Category",
//    "RolePrivilegeOperations": [
//      {
//        "Operation": "Create",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Edit",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Delete",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "View",
//        "EntityName": "",
//        "IsEnabled": false
//      }
//    ]
//  },
//  {
//    "EntityName": "DefectType",
//    "DisplayName": "Defect Type",
//    "RolePrivilegeOperations": [
//      {
//        "Operation": "Create",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Edit",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "View",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Delete",
//        "EntityName": "",
//        "IsEnabled": false
//      }
//    ]
//  },
//  {
//    "EntityName": "DropTest",
//    "DisplayName": "Drop Test",
//    "RolePrivilegeOperations": [
//      {
//        "Operation": "Create",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Edit",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Delete",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "View",
//        "EntityName": "",
//        "IsEnabled": false
//      }
//    ]
//  },
//  {
//    "EntityName": "EMSAlertRule",
//    "DisplayName": "EMSAlertRule",
//    "RolePrivilegeOperations": [
//      {
//        "Operation": "Create",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "View",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Delete",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Edit",
//        "EntityName": "",
//        "IsEnabled": false
//      }
//    ]
//  },
//  {
//    "EntityName": "Forgot Password",
//    "DisplayName": "Forgot Password",
//    "RolePrivilegeOperations": [
//    ]
//  },
//  {
//    "EntityName": "FPRAction",
//    "DisplayName": "FPR Action",
//    "RolePrivilegeOperations": [
//      {
//        "Operation": "Add",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Edit",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Close",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Delete",
//        "EntityName": "",
//        "IsEnabled": false
//      }
//    ]
//  },
//  {
//    "EntityName": "GaugeEntry",
//    "DisplayName": "Gauge Entry",
//    "RolePrivilegeOperations": [
//      {
//        "Operation": "Create",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Edit",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Delete",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "View",
//        "EntityName": "",
//        "IsEnabled": false
//      }
//    ]
//  },
//  {
//    "EntityName": "GaugeParameter",
//    "DisplayName": "Gauge Parameter",
//    "RolePrivilegeOperations": [
//      {
//        "Operation": "Create",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Edit",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Delete",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "View",
//        "EntityName": "",
//        "IsEnabled": false
//      }
//    ]
//  },
//  {
//    "EntityName": "GaugeParameterSelection",
//    "DisplayName": "Gauge Parameter Selection",
//    "RolePrivilegeOperations": [
//      {
//        "Operation": "Create",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Edit",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Delete",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "View",
//        "EntityName": "",
//        "IsEnabled": false
//      }
//    ]
//  },
//  {
//    "EntityName": "HECoatingTest",
//    "DisplayName": "HE Coating Test",
//    "RolePrivilegeOperations": [
//      {
//        "Operation": "Create",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Edit",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Delete",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "View",
//        "EntityName": "",
//        "IsEnabled": false
//      }
//    ]
//  },
//  {
//    "EntityName": "HESetOut",
//    "DisplayName": "HE Set Outs",
//    "RolePrivilegeOperations": [
//      {
//        "Operation": "View",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Edit",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Create",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Delete",
//        "EntityName": "",
//        "IsEnabled": false
//      }
//    ]
//  },
//  {
//    "EntityName": "HEWeight",
//    "DisplayName": "HE Weight",
//    "RolePrivilegeOperations": [
//      {
//        "Operation": "Create",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Edit",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Delete",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "View",
//        "EntityName": "",
//        "IsEnabled": false
//      }
//    ]
//  },
//  {
//    "EntityName": "HourlyMIS",
//    "DisplayName": "Hourly MIS",
//    "RolePrivilegeOperations": [
//      {
//        "Operation": "View",
//        "EntityName": "",
//        "IsEnabled": false
//      }
//    ]
//  },
//  {
//    "EntityName": "HourlyProduction",
//    "DisplayName": "Hourly Production",
//    "RolePrivilegeOperations": [
//      {
//        "Operation": "Create",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Edit",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Delete",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "View",
//        "EntityName": "",
//        "IsEnabled": false
//      }
//    ]
//  },
//  {
//    "EntityName": "ImpactTest",
//    "DisplayName": "Impact Test",
//    "RolePrivilegeOperations": [
//      {
//        "Operation": "Create",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Edit",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Delete",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "View",
//        "EntityName": "",
//        "IsEnabled": false
//      }
//    ]
//  },
//  {
//    "EntityName": "JobAQL",
//    "DisplayName": "Set Job AQL",
//    "RolePrivilegeOperations": [
//      {
//        "Operation": "View",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Edit",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Delete",
//        "EntityName": "",
//        "IsEnabled": false
//      }
//    ]
//  },
//  {
//    "EntityName": "JobFeedback",
//    "DisplayName": "Job Feedback",
//    "RolePrivilegeOperations": [
//      {
//        "Operation": "Create",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Edit",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Delete",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "View",
//        "EntityName": "",
//        "IsEnabled": false
//      }
//    ]
//  },
//  {
//    "EntityName": "JobFeedbackRemark",
//    "DisplayName": "Job Feedback Remark",
//    "RolePrivilegeOperations": [
//      {
//        "Operation": "QC",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Production",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Furnace",
//        "EntityName": "",
//        "IsEnabled": false
//      }
//    ]
//  },
//  {
//    "EntityName": "JobPlan",
//    "DisplayName": "JobPlan",
//    "RolePrivilegeOperations": [
//      {
//        "Operation": "Create",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "View",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Delete",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Edit",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Start",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Stop",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Pause",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Resume",
//        "EntityName": "",
//        "IsEnabled": false
//      }
//    ]
//  },
//  {
//    "EntityName": "JobQCLabParameter",
//    "DisplayName": "QC Lab Parameters",
//    "RolePrivilegeOperations": [
//      {
//        "Operation": "Create",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Edit",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Delete",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "View",
//        "EntityName": "",
//        "IsEnabled": false
//      }
//    ]
//  },
//  {
//    "EntityName": "JobRunSummary",
//    "DisplayName": "Job Run Summary",
//    "RolePrivilegeOperations": [
//      {
//        "Operation": "View",
//        "EntityName": "",
//        "IsEnabled": false
//      }
//    ]
//  },
//  {
//    "EntityName": "LeakageTest",
//    "DisplayName": "Leakage Test",
//    "RolePrivilegeOperations": [
//      {
//        "Operation": "Create",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Edit",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Delete",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "View",
//        "EntityName": "",
//        "IsEnabled": false
//      }
//    ]
//  },
//  {
//    "EntityName": "Machine",
//    "DisplayName": "Machine",
//    "RolePrivilegeOperations": [
//      {
//        "Operation": "Create",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "View",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Delete",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Edit",
//        "EntityName": "",
//        "IsEnabled": false
//      }
//    ]
//  },
//  {
//    "EntityName": "Meter",
//    "DisplayName": "Meter",
//    "RolePrivilegeOperations": [
//      {
//        "Operation": "Create",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "View",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Delete",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Edit",
//        "EntityName": "",
//        "IsEnabled": false
//      }
//    ]
//  },
//  {
//    "EntityName": "Mould",
//    "DisplayName": "Mould Setup",
//    "RolePrivilegeOperations": [
//      {
//        "Operation": "Create",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Edit",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Delete",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "View",
//        "EntityName": "",
//        "IsEnabled": false
//      }
//    ]
//  },
//  {
//    "EntityName": "OperatorAction",
//    "DisplayName": "Operator Actions",
//    "RolePrivilegeOperations": [
//      {
//        "Operation": "View",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Edit",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Acknowledge",
//        "EntityName": "",
//        "IsEnabled": false
//      }
//    ]
//  },
//  {
//    "EntityName": "OrganisationUnit",
//    "DisplayName": "OrganisationUnit",
//    "RolePrivilegeOperations": [
//      {
//        "Operation": "Create",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "View",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Delete",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Edit",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "RunMultipleJob",
//        "EntityName": "",
//        "IsEnabled": false
//      }
//    ]
//  },
//  {
//    "EntityName": "Parameter",
//    "DisplayName": "Parameter",
//    "RolePrivilegeOperations": [
//      {
//        "Operation": "Create",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "View",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Delete",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Edit",
//        "EntityName": "",
//        "IsEnabled": false
//      }
//    ]
//  },
//  {
//    "EntityName": "PlanityTest",
//    "DisplayName": "Planity Test",
//    "RolePrivilegeOperations": [
//      {
//        "Operation": "Create",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Edit",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Delete",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "View",
//        "EntityName": "",
//        "IsEnabled": false
//      }
//    ]
//  },
//  {
//    "EntityName": "PlantSummaryReport",
//    "DisplayName": "Plant Summary Report",
//    "RolePrivilegeOperations": [
//      {
//        "Operation": "View",
//        "EntityName": "",
//        "IsEnabled": false
//      }
//    ]
//  },
//  {
//    "EntityName": "Product",
//    "DisplayName": "Product",
//    "RolePrivilegeOperations": [
//      {
//        "Operation": "Create",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "View",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Delete",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Edit",
//        "EntityName": "",
//        "IsEnabled": false
//      }
//    ]
//  },
//  {
//    "EntityName": "ProductQCLabParameter",
//    "DisplayName": "QC Lab Parameter Selection",
//    "RolePrivilegeOperations": [
//      {
//        "Operation": "Create",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Edit",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Delete",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "View",
//        "EntityName": "",
//        "IsEnabled": false
//      }
//    ]
//  },
//  {
//    "EntityName": "QASampling",
//    "DisplayName": "QA Sampling",
//    "RolePrivilegeOperations": [
//      {
//        "Operation": "View",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Edit",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Create",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Cancel",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "SyncWithSAP",
//        "EntityName": "",
//        "IsEnabled": false
//      }
//    ]
//  },
//  {
//    "EntityName": "QCHold",
//    "DisplayName": "QC Hold",
//    "RolePrivilegeOperations": [
//      {
//        "Operation": "Create",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Edit",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Delete",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "View",
//        "EntityName": "",
//        "IsEnabled": false
//      }
//    ]
//  },
//  {
//    "EntityName": "QCLabSampling",
//    "DisplayName": "QC Lab Sampling",
//    "RolePrivilegeOperations": [
//      {
//        "Operation": "Create",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Edit",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Delete",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "View",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "SyncWithSAP",
//        "EntityName": "",
//        "IsEnabled": false
//      }
//    ]
//  },
//  {
//    "EntityName": "RampPressureTest",
//    "DisplayName": "Ramp Pressure Test",
//    "RolePrivilegeOperations": [
//      {
//        "Operation": "Create",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Edit",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "View",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Delete",
//        "EntityName": "",
//        "IsEnabled": false
//      }
//    ]
//  },
//  {
//    "EntityName": "Resorting",
//    "DisplayName": "Resorting",
//    "RolePrivilegeOperations": [
//      {
//        "Operation": "View",
//        "EntityName": "",
//        "IsEnabled": false
//      }
//    ]
//  },
//  {
//    "EntityName": "ResortingStatusReport",
//    "DisplayName": "Resorting Status Report",
//    "RolePrivilegeOperations": [
//      {
//        "Operation": "View",
//        "EntityName": "",
//        "IsEnabled": false
//      }
//    ]
//  },
//  {
//    "EntityName": "Role",
//    "DisplayName": "Role",
//    "RolePrivilegeOperations": [
//      {
//        "Operation": "Create",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "View",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Edit",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Delete",
//        "EntityName": "",
//        "IsEnabled": false
//      }
//    ]
//  },
//  {
//    "EntityName": "RTMIAppConfiguration",
//    "DisplayName": "RTMI App Configuration",
//    "RolePrivilegeOperations": [
//      {
//        "Operation": "Edit",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "View",
//        "EntityName": "",
//        "IsEnabled": false
//      }
//    ]
//  },
//  {
//    "EntityName": "RTMISAPIntegration",
//    "DisplayName": "RTMI SAP Integration",
//    "RolePrivilegeOperations": [
//      {
//        "Operation": "View",
//        "EntityName": "",
//        "IsEnabled": false
//      }
//    ]
//  },
//  {
//    "EntityName": "SetOutComplianceReport",
//    "DisplayName": "Set Out Compliance Report",
//    "RolePrivilegeOperations": [
//      {
//        "Operation": "View",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Edit",
//        "EntityName": "",
//        "IsEnabled": false
//      }
//    ]
//  },
//  {
//    "EntityName": "ShakerTest",
//    "DisplayName": "Shaker Test",
//    "RolePrivilegeOperations": [
//      {
//        "Operation": "Create",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Edit",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Delete",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "View",
//        "EntityName": "",
//        "IsEnabled": false
//      }
//    ]
//  },
//  {
//    "EntityName": "Shift",
//    "DisplayName": "Shift",
//    "RolePrivilegeOperations": [
//      {
//        "Operation": "Create",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "View",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Delete",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Edit",
//        "EntityName": "",
//        "IsEnabled": false
//      }
//    ]
//  },
//  {
//    "EntityName": "Station",
//    "DisplayName": "Station",
//    "RolePrivilegeOperations": [
//      {
//        "Operation": "Create",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "View",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Delete",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Edit",
//        "EntityName": "",
//        "IsEnabled": false
//      }
//    ]
//  },
//  {
//    "EntityName": "TestConfiguration",
//    "DisplayName": "Test Configuration",
//    "RolePrivilegeOperations": [
//      {
//        "Operation": "Create",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Edit",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Delete",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "View",
//        "EntityName": "",
//        "IsEnabled": false
//      }
//    ]
//  },
//  {
//    "EntityName": "ThermalShock",
//    "DisplayName": "Thermal Shock Test Record",
//    "RolePrivilegeOperations": [
//      {
//        "Operation": "Create",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Edit",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Delete",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "View",
//        "EntityName": "",
//        "IsEnabled": false
//      }
//    ]
//  },
//  {
//    "EntityName": "TransferredToFG",
//    "DisplayName": "Transferred To FG",
//    "RolePrivilegeOperations": [
//      {
//        "Operation": "Create",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Edit",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "View",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Delete",
//        "EntityName": "",
//        "IsEnabled": false
//      }
//    ]
//  },
//  {
//    "EntityName": "UnitOfMeasurement",
//    "DisplayName": "Unit Of Measurement",
//    "RolePrivilegeOperations": [
//      {
//        "Operation": "View",
//        "EntityName": "",
//        "IsEnabled": false
//      }
//    ]
//  },
//  {
//    "EntityName": "User",
//    "DisplayName": "User",
//    "RolePrivilegeOperations": [
//      {
//        "Operation": "Create",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Edit",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Delete",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "View",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Lock/Unlock",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "ChangePassword",
//        "EntityName": "",
//        "IsEnabled": false
//      }
//    ]
//  },
//  {
//    "EntityName": "VerticalityTest",
//    "DisplayName": "Verticality Test",
//    "RolePrivilegeOperations": [
//      {
//        "Operation": "Create",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Edit",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Delete",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "View",
//        "EntityName": "",
//        "IsEnabled": false
//      }
//    ]
//  },
//  {
//    "EntityName": "VerticalLoadTest",
//    "DisplayName": "Vertical Load Test",
//    "RolePrivilegeOperations": [
//      {
//        "Operation": "Create",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Edit",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Delete",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "View",
//        "EntityName": "",
//        "IsEnabled": false
//      }
//    ]
//  },
//  {
//    "EntityName": "Weight",
//    "DisplayName": "Weight Record",
//    "RolePrivilegeOperations": [
//      {
//        "Operation": "Create",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Edit",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "Delete",
//        "EntityName": "",
//        "IsEnabled": false
//      },
//      {
//        "Operation": "View",
//        "EntityName": "",
//        "IsEnabled": false
//      }
//    ]
//  }
//];
//# sourceMappingURL=rolePrivilege.js.map