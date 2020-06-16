export interface Entity
{
    "EntityName": string;
    "DisplayName": string;
    "Keys": string;
    "AssemblyName":string;
    "NamespaceName": string;
    "Operations": string[];
    "EntityOperations": EntityOperations[],
    "ComponentCode": string;
    "IsActive": boolean;
    "IsImportEnabled": boolean;
    "ServiceURL": string;
    "IsDefaultEntity": boolean;
  };

  export interface EntityOperations {
    "EntityName": string;
    "Operation": string;
    "DependentOperations": EntityOperations[];
  }

  export var EntityDummyList:Entity[]=[
    {
      "EntityName": "Alert",
      "DisplayName": "Alert",
      "Keys": null,
      "AssemblyName": null,
      "NamespaceName": null,
      "Operations": [
        "View",
        "Acknowledge"
      ],
      "EntityOperations": [
        {
          "EntityName": "Alert",
          "Operation": "View",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "Alert",
          "Operation": "Acknowledge",
          "DependentOperations": [
            
          ]
        }
      ],
      "ComponentCode": "PMS",
      "IsActive": true,
      "IsImportEnabled": false,
      "ServiceURL": null,
      "IsDefaultEntity": false
    },
    {
      "EntityName": "AlertRule",
      "DisplayName": "AlertRule",
      "Keys": null,
      "AssemblyName": null,
      "NamespaceName": null,
      "Operations": [
        "Create",
        "View",
        "Delete",
        "Edit"
      ],
      "EntityOperations": [
        {
          "EntityName": "AlertRule",
          "Operation": "Create",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "AlertRule",
          "Operation": "View",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "AlertRule",
          "Operation": "Delete",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "AlertRule",
          "Operation": "Edit",
          "DependentOperations": [
            
          ]
        }
      ],
      "ComponentCode": "PMS",
      "IsActive": true,
      "IsImportEnabled": false,
      "ServiceURL": null,
      "IsDefaultEntity": false
    },
    {
      "EntityName": "AnnealingTest",
      "DisplayName": "Annealing Record",
      "Keys": null,
      "AssemblyName": null,
      "NamespaceName": null,
      "Operations": [
        "Create",
        "Edit",
        "Delete",
        "View"
      ],
      "EntityOperations": [
        {
          "EntityName": "AnnealingTest",
          "Operation": "Create",
          "DependentOperations": [
            {
              "EntityName": "JobPlan",
              "Operation": "View",
              "DependentOperations": [
                {
                  "EntityName": "Product",
                  "Operation": "View",
                  "DependentOperations": [
                    {
                      "EntityName": "Machine",
                      "Operation": "View",
                      "DependentOperations": [
                        {
                          "EntityName": "OrganisationUnit",
                          "Operation": "View",
                          "DependentOperations": [
                            
                          ]
                        }
                      ]
                    },
                    {
                      "EntityName": "Configuration",
                      "Operation": "View",
                      "DependentOperations": [
                        
                      ]
                    }
                  ]
                }
              ]
            }
          ]
        },
        {
          "EntityName": "AnnealingTest",
          "Operation": "Edit",
          "DependentOperations": [
            {
              "EntityName": "JobPlan",
              "Operation": "View",
              "DependentOperations": [
                {
                  "EntityName": "Product",
                  "Operation": "View",
                  "DependentOperations": [
                    {
                      "EntityName": "Machine",
                      "Operation": "View",
                      "DependentOperations": [
                        {
                          "EntityName": "OrganisationUnit",
                          "Operation": "View",
                          "DependentOperations": [
                            
                          ]
                        }
                      ]
                    },
                    {
                      "EntityName": "Configuration",
                      "Operation": "View",
                      "DependentOperations": [
                        
                      ]
                    }
                  ]
                }
              ]
            }
          ]
        },
        {
          "EntityName": "AnnealingTest",
          "Operation": "Delete",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "AnnealingTest",
          "Operation": "View",
          "DependentOperations": [
            {
              "EntityName": "JobPlan",
              "Operation": "View",
              "DependentOperations": [
                {
                  "EntityName": "Product",
                  "Operation": "View",
                  "DependentOperations": [
                    {
                      "EntityName": "Machine",
                      "Operation": "View",
                      "DependentOperations": [
                        {
                          "EntityName": "OrganisationUnit",
                          "Operation": "View",
                          "DependentOperations": [
                            
                          ]
                        }
                      ]
                    },
                    {
                      "EntityName": "Configuration",
                      "Operation": "View",
                      "DependentOperations": [
                        
                      ]
                    }
                  ]
                }
              ]
            }
          ]
        }
      ],
      "ComponentCode": "PMS",
      "IsActive": true,
      "IsImportEnabled": false,
      "ServiceURL": null,
      "IsDefaultEntity": false
    },
    {
      "EntityName": "AQL",
      "DisplayName": "AQL",
      "Keys": null,
      "AssemblyName": null,
      "NamespaceName": null,
      "Operations": [
        "Create",
        "Edit",
        "Delete",
        "View"
      ],
      "EntityOperations": [
        {
          "EntityName": "AQL",
          "Operation": "Create",
          "DependentOperations": [
            {
              "EntityName": "DefectType",
              "Operation": "View",
              "DependentOperations": [
                
              ]
            },
            {
              "EntityName": "Defect",
              "Operation": "View",
              "DependentOperations": [
                
              ]
            }
          ]
        },
        {
          "EntityName": "AQL",
          "Operation": "Edit",
          "DependentOperations": [
            {
              "EntityName": "DefectType",
              "Operation": "View",
              "DependentOperations": [
                
              ]
            },
            {
              "EntityName": "Defect",
              "Operation": "View",
              "DependentOperations": [
                
              ]
            }
          ]
        },
        {
          "EntityName": "AQL",
          "Operation": "Delete",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "AQL",
          "Operation": "View",
          "DependentOperations": [
            
          ]
        }
      ],
      "ComponentCode": "PMS",
      "IsActive": true,
      "IsImportEnabled": false,
      "ServiceURL": null,
      "IsDefaultEntity": false
    },
    {
      "EntityName": "BrustingTest",
      "DisplayName": "Brusting Test",
      "Keys": null,
      "AssemblyName": null,
      "NamespaceName": null,
      "Operations": [
        "Create",
        "Edit",
        "Delete",
        "View"
      ],
      "EntityOperations": [
        {
          "EntityName": "BrustingTest",
          "Operation": "Create",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "BrustingTest",
          "Operation": "Edit",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "BrustingTest",
          "Operation": "Delete",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "BrustingTest",
          "Operation": "View",
          "DependentOperations": [
            
          ]
        }
      ],
      "ComponentCode": "PMS",
      "IsActive": true,
      "IsImportEnabled": false,
      "ServiceURL": null,
      "IsDefaultEntity": false
    },
    {
      "EntityName": "CECoatingTest",
      "DisplayName": "CE Coating Test",
      "Keys": null,
      "AssemblyName": null,
      "NamespaceName": null,
      "Operations": [
        "Create",
        "Edit",
        "Delete",
        "View"
      ],
      "EntityOperations": [
        {
          "EntityName": "CECoatingTest",
          "Operation": "Create",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "CECoatingTest",
          "Operation": "Edit",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "CECoatingTest",
          "Operation": "Delete",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "CECoatingTest",
          "Operation": "View",
          "DependentOperations": [
            
          ]
        }
      ],
      "ComponentCode": "PMS",
      "IsActive": true,
      "IsImportEnabled": false,
      "ServiceURL": null,
      "IsDefaultEntity": false
    },
    {
      "EntityName": "CESetOut",
      "DisplayName": "CE Set Outs",
      "Keys": null,
      "AssemblyName": null,
      "NamespaceName": null,
      "Operations": [
        "View",
        "Edit",
        "Create",
        "Delete"
      ],
      "EntityOperations": [
        {
          "EntityName": "CESetOut",
          "Operation": "View",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "CESetOut",
          "Operation": "Edit",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "CESetOut",
          "Operation": "Create",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "CESetOut",
          "Operation": "Delete",
          "DependentOperations": [
            
          ]
        }
      ],
      "ComponentCode": "PMS",
      "IsActive": true,
      "IsImportEnabled": false,
      "ServiceURL": null,
      "IsDefaultEntity": false
    },
    {
      "EntityName": "ChemicalLabSampling",
      "DisplayName": "Chemical Lab Sampling",
      "Keys": null,
      "AssemblyName": null,
      "NamespaceName": null,
      "Operations": [
        "Create",
        "Edit",
        "Delete",
        "View",
        "SyncWithSAP"
      ],
      "EntityOperations": [
        {
          "EntityName": "ChemicalLabSampling",
          "Operation": "Create",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "ChemicalLabSampling",
          "Operation": "Edit",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "ChemicalLabSampling",
          "Operation": "Delete",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "ChemicalLabSampling",
          "Operation": "View",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "ChemicalLabSampling",
          "Operation": "SyncWithSAP",
          "DependentOperations": [
            
          ]
        }
      ],
      "ComponentCode": "PMS",
      "IsActive": true,
      "IsImportEnabled": false,
      "ServiceURL": null,
      "IsDefaultEntity": false
    },
    {
      "EntityName": "ChemicalLabSelection",
      "DisplayName": "Chemical Lab Selection",
      "Keys": null,
      "AssemblyName": null,
      "NamespaceName": null,
      "Operations": [
        "Create",
        "Edit",
        "Delete",
        "View",
        "SyncWithSAP"
      ],
      "EntityOperations": [
        {
          "EntityName": "ChemicalLabSelection",
          "Operation": "Create",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "ChemicalLabSelection",
          "Operation": "Edit",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "ChemicalLabSelection",
          "Operation": "Delete",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "ChemicalLabSelection",
          "Operation": "View",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "ChemicalLabSelection",
          "Operation": "SyncWithSAP",
          "DependentOperations": [
            
          ]
        }
      ],
      "ComponentCode": "PMS",
      "IsActive": true,
      "IsImportEnabled": false,
      "ServiceURL": null,
      "IsDefaultEntity": false
    },
    {
      "EntityName": "CompareSetOutReport",
      "DisplayName": "Compare Setouts Report",
      "Keys": null,
      "AssemblyName": null,
      "NamespaceName": null,
      "Operations": [
        "View"
      ],
      "EntityOperations": [
        {
          "EntityName": "CompareSetOutReport",
          "Operation": "View",
          "DependentOperations": [
            
          ]
        }
      ],
      "ComponentCode": "PMS",
      "IsActive": true,
      "IsImportEnabled": false,
      "ServiceURL": null,
      "IsDefaultEntity": false
    },
    {
      "EntityName": "Configuration",
      "DisplayName": "Configuration",
      "Keys": null,
      "AssemblyName": null,
      "NamespaceName": null,
      "Operations": [
        "Create",
        "View",
        "Delete",
        "Edit"
      ],
      "EntityOperations": [
        {
          "EntityName": "Configuration",
          "Operation": "Create",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "Configuration",
          "Operation": "View",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "Configuration",
          "Operation": "Delete",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "Configuration",
          "Operation": "Edit",
          "DependentOperations": [
            
          ]
        }
      ],
      "ComponentCode": "PMS",
      "IsActive": true,
      "IsImportEnabled": false,
      "ServiceURL": null,
      "IsDefaultEntity": false
    },
    {
      "EntityName": "Customer",
      "DisplayName": "Customer",
      "Keys": null,
      "AssemblyName": null,
      "NamespaceName": null,
      "Operations": [
        "Create",
        "Edit",
        "Delete",
        "View"
      ],
      "EntityOperations": [
        {
          "EntityName": "Customer",
          "Operation": "Create",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "Customer",
          "Operation": "Edit",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "Customer",
          "Operation": "Delete",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "Customer",
          "Operation": "View",
          "DependentOperations": [
            
          ]
        }
      ],
      "ComponentCode": "PMS",
      "IsActive": true,
      "IsImportEnabled": false,
      "ServiceURL": null,
      "IsDefaultEntity": false
    },
    {
      "EntityName": "CustomerFeedback",
      "DisplayName": "Customer Feedback",
      "Keys": null,
      "AssemblyName": null,
      "NamespaceName": null,
      "Operations": [
        "Create",
        "Edit",
        "Delete",
        "View"
      ],
      "EntityOperations": [
        {
          "EntityName": "CustomerFeedback",
          "Operation": "Create",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "CustomerFeedback",
          "Operation": "Edit",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "CustomerFeedback",
          "Operation": "Delete",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "CustomerFeedback",
          "Operation": "View",
          "DependentOperations": [
            
          ]
        }
      ],
      "ComponentCode": "PMS",
      "IsActive": true,
      "IsImportEnabled": false,
      "ServiceURL": null,
      "IsDefaultEntity": false
    },
    {
      "EntityName": "DailyJobReport",
      "DisplayName": "Daily Job Report",
      "Keys": null,
      "AssemblyName": null,
      "NamespaceName": null,
      "Operations": [
        "View"
      ],
      "EntityOperations": [
        {
          "EntityName": "DailyJobReport",
          "Operation": "View",
          "DependentOperations": [
            
          ]
        }
      ],
      "ComponentCode": "PMS",
      "IsActive": true,
      "IsImportEnabled": false,
      "ServiceURL": null,
      "IsDefaultEntity": false
    },
    {
      "EntityName": "DailyMIS",
      "DisplayName": "Daily MIS",
      "Keys": null,
      "AssemblyName": null,
      "NamespaceName": null,
      "Operations": [
        "View"
      ],
      "EntityOperations": [
        {
          "EntityName": "DailyMIS",
          "Operation": "View",
          "DependentOperations": [
            
          ]
        }
      ],
      "ComponentCode": "PMS",
      "IsActive": true,
      "IsImportEnabled": false,
      "ServiceURL": null,
      "IsDefaultEntity": false
    },
    {
      "EntityName": "Defect",
      "DisplayName": "Defects",
      "Keys": null,
      "AssemblyName": null,
      "NamespaceName": null,
      "Operations": [
        "Create",
        "Edit",
        "Delete",
        "View"
      ],
      "EntityOperations": [
        {
          "EntityName": "Defect",
          "Operation": "Create",
          "DependentOperations": [
            {
              "EntityName": "DefectType",
              "Operation": "View",
              "DependentOperations": [
                
              ]
            },
            {
              "EntityName": "DefectCategory",
              "Operation": "View",
              "DependentOperations": [
                
              ]
            }
          ]
        },
        {
          "EntityName": "Defect",
          "Operation": "Edit",
          "DependentOperations": [
            {
              "EntityName": "DefectType",
              "Operation": "View",
              "DependentOperations": [
                
              ]
            },
            {
              "EntityName": "DefectCategory",
              "Operation": "View",
              "DependentOperations": [
                
              ]
            }
          ]
        },
        {
          "EntityName": "Defect",
          "Operation": "Delete",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "Defect",
          "Operation": "View",
          "DependentOperations": [
            
          ]
        }
      ],
      "ComponentCode": "PMS",
      "IsActive": true,
      "IsImportEnabled": false,
      "ServiceURL": null,
      "IsDefaultEntity": false
    },
    {
      "EntityName": "DefectCategory",
      "DisplayName": "Defect Category",
      "Keys": null,
      "AssemblyName": null,
      "NamespaceName": null,
      "Operations": [
        "Create",
        "Edit",
        "Delete",
        "View"
      ],
      "EntityOperations": [
        {
          "EntityName": "DefectCategory",
          "Operation": "Create",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "DefectCategory",
          "Operation": "Edit",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "DefectCategory",
          "Operation": "Delete",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "DefectCategory",
          "Operation": "View",
          "DependentOperations": [
            
          ]
        }
      ],
      "ComponentCode": "PMS",
      "IsActive": true,
      "IsImportEnabled": false,
      "ServiceURL": null,
      "IsDefaultEntity": false
    },
    {
      "EntityName": "DefectType",
      "DisplayName": "Defect Type",
      "Keys": null,
      "AssemblyName": null,
      "NamespaceName": null,
      "Operations": [
        "Create",
        "Edit",
        "View",
        "Delete"
      ],
      "EntityOperations": [
        {
          "EntityName": "DefectType",
          "Operation": "Create",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "DefectType",
          "Operation": "Edit",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "DefectType",
          "Operation": "View",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "DefectType",
          "Operation": "Delete",
          "DependentOperations": [
            
          ]
        }
      ],
      "ComponentCode": "PMS",
      "IsActive": true,
      "IsImportEnabled": false,
      "ServiceURL": null,
      "IsDefaultEntity": false
    },
    {
      "EntityName": "DropTest",
      "DisplayName": "Drop Test",
      "Keys": null,
      "AssemblyName": null,
      "NamespaceName": null,
      "Operations": [
        "Create",
        "Edit",
        "Delete",
        "View"
      ],
      "EntityOperations": [
        {
          "EntityName": "DropTest",
          "Operation": "Create",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "DropTest",
          "Operation": "Edit",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "DropTest",
          "Operation": "Delete",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "DropTest",
          "Operation": "View",
          "DependentOperations": [
            
          ]
        }
      ],
      "ComponentCode": "PMS",
      "IsActive": true,
      "IsImportEnabled": false,
      "ServiceURL": null,
      "IsDefaultEntity": false
    },
    {
      "EntityName": "EMSAlertRule",
      "DisplayName": "EMSAlertRule",
      "Keys": null,
      "AssemblyName": null,
      "NamespaceName": null,
      "Operations": [
        "Create",
        "View",
        "Delete",
        "Edit"
      ],
      "EntityOperations": [
        {
          "EntityName": "EMSAlertRule",
          "Operation": "Create",
          "DependentOperations": [
            {
              "EntityName": "Parameter",
              "Operation": "View",
              "DependentOperations": [
                
              ]
            },
            {
              "EntityName": "Station",
              "Operation": "View",
              "DependentOperations": [
                
              ]
            }
          ]
        },
        {
          "EntityName": "EMSAlertRule",
          "Operation": "View",
          "DependentOperations": [
            {
              "EntityName": "Meter",
              "Operation": "View",
              "DependentOperations": [
                
              ]
            },
            {
              "EntityName": "Parameter",
              "Operation": "View",
              "DependentOperations": [
                
              ]
            },
            {
              "EntityName": "Station",
              "Operation": "View",
              "DependentOperations": [
                
              ]
            }
          ]
        },
        {
          "EntityName": "EMSAlertRule",
          "Operation": "Delete",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "EMSAlertRule",
          "Operation": "Edit",
          "DependentOperations": [
            {
              "EntityName": "Parameter",
              "Operation": "View",
              "DependentOperations": [
                
              ]
            },
            {
              "EntityName": "Station",
              "Operation": "View",
              "DependentOperations": [
                
              ]
            }
          ]
        }
      ],
      "ComponentCode": "PMS",
      "IsActive": true,
      "IsImportEnabled": false,
      "ServiceURL": null,
      "IsDefaultEntity": false
    },
    {
      "EntityName": "Forgot Password",
      "DisplayName": "Forgot Password",
      "Keys": null,
      "AssemblyName": null,
      "NamespaceName": null,
      "Operations": [
        
      ],
      "EntityOperations": [
        
      ],
      "ComponentCode": "PMS",
      "IsActive": true,
      "IsImportEnabled": false,
      "ServiceURL": null,
      "IsDefaultEntity": false
    },
    {
      "EntityName": "FPRAction",
      "DisplayName": "FPR Action",
      "Keys": null,
      "AssemblyName": null,
      "NamespaceName": null,
      "Operations": [
        "Add",
        "Edit",
        "Close",
        "Delete"
      ],
      "EntityOperations": [
        {
          "EntityName": "FPRAction",
          "Operation": "Add",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "FPRAction",
          "Operation": "Edit",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "FPRAction",
          "Operation": "Close",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "FPRAction",
          "Operation": "Delete",
          "DependentOperations": [
            
          ]
        }
      ],
      "ComponentCode": "PMS",
      "IsActive": true,
      "IsImportEnabled": false,
      "ServiceURL": null,
      "IsDefaultEntity": false
    },
    {
      "EntityName": "GaugeEntry",
      "DisplayName": "Gauge Entry",
      "Keys": null,
      "AssemblyName": null,
      "NamespaceName": null,
      "Operations": [
        "Create",
        "Edit",
        "Delete",
        "View"
      ],
      "EntityOperations": [
        {
          "EntityName": "GaugeEntry",
          "Operation": "Create",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "GaugeEntry",
          "Operation": "Edit",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "GaugeEntry",
          "Operation": "Delete",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "GaugeEntry",
          "Operation": "View",
          "DependentOperations": [
            
          ]
        }
      ],
      "ComponentCode": "PMS",
      "IsActive": true,
      "IsImportEnabled": false,
      "ServiceURL": null,
      "IsDefaultEntity": false
    },
    {
      "EntityName": "GaugeParameter",
      "DisplayName": "Gauge Parameter",
      "Keys": null,
      "AssemblyName": null,
      "NamespaceName": null,
      "Operations": [
        "Create",
        "Edit",
        "Delete",
        "View"
      ],
      "EntityOperations": [
        {
          "EntityName": "GaugeParameter",
          "Operation": "Create",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "GaugeParameter",
          "Operation": "Edit",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "GaugeParameter",
          "Operation": "Delete",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "GaugeParameter",
          "Operation": "View",
          "DependentOperations": [
            
          ]
        }
      ],
      "ComponentCode": "PMS",
      "IsActive": true,
      "IsImportEnabled": false,
      "ServiceURL": null,
      "IsDefaultEntity": false
    },
    {
      "EntityName": "GaugeParameterSelection",
      "DisplayName": "Gauge Parameter Selection",
      "Keys": null,
      "AssemblyName": null,
      "NamespaceName": null,
      "Operations": [
        "Create",
        "Edit",
        "Delete",
        "View"
      ],
      "EntityOperations": [
        {
          "EntityName": "GaugeParameterSelection",
          "Operation": "Create",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "GaugeParameterSelection",
          "Operation": "Edit",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "GaugeParameterSelection",
          "Operation": "Delete",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "GaugeParameterSelection",
          "Operation": "View",
          "DependentOperations": [
            
          ]
        }
      ],
      "ComponentCode": "PMS",
      "IsActive": true,
      "IsImportEnabled": false,
      "ServiceURL": null,
      "IsDefaultEntity": false
    },
    {
      "EntityName": "HECoatingTest",
      "DisplayName": "HE Coating Test",
      "Keys": null,
      "AssemblyName": null,
      "NamespaceName": null,
      "Operations": [
        "Create",
        "Edit",
        "Delete",
        "View"
      ],
      "EntityOperations": [
        {
          "EntityName": "HECoatingTest",
          "Operation": "Create",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "HECoatingTest",
          "Operation": "Edit",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "HECoatingTest",
          "Operation": "Delete",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "HECoatingTest",
          "Operation": "View",
          "DependentOperations": [
            
          ]
        }
      ],
      "ComponentCode": "PMS",
      "IsActive": true,
      "IsImportEnabled": false,
      "ServiceURL": null,
      "IsDefaultEntity": false
    },
    {
      "EntityName": "HESetOut",
      "DisplayName": "HE Set Outs",
      "Keys": null,
      "AssemblyName": null,
      "NamespaceName": null,
      "Operations": [
        "View",
        "Edit",
        "Create",
        "Delete"
      ],
      "EntityOperations": [
        {
          "EntityName": "HESetOut",
          "Operation": "View",
          "DependentOperations": [
            {
              "EntityName": "JobPlan",
              "Operation": "View",
              "DependentOperations": [
                {
                  "EntityName": "Product",
                  "Operation": "View",
                  "DependentOperations": [
                    {
                      "EntityName": "Machine",
                      "Operation": "View",
                      "DependentOperations": [
                        {
                          "EntityName": "OrganisationUnit",
                          "Operation": "View",
                          "DependentOperations": [
                            
                          ]
                        }
                      ]
                    },
                    {
                      "EntityName": "Configuration",
                      "Operation": "View",
                      "DependentOperations": [
                        
                      ]
                    }
                  ]
                }
              ]
            }
          ]
        },
        {
          "EntityName": "HESetOut",
          "Operation": "Edit",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "HESetOut",
          "Operation": "Create",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "HESetOut",
          "Operation": "Delete",
          "DependentOperations": [
            
          ]
        }
      ],
      "ComponentCode": "PMS",
      "IsActive": true,
      "IsImportEnabled": false,
      "ServiceURL": null,
      "IsDefaultEntity": false
    },
    {
      "EntityName": "HEWeight",
      "DisplayName": "HE Weight",
      "Keys": null,
      "AssemblyName": null,
      "NamespaceName": null,
      "Operations": [
        "Create",
        "Edit",
        "Delete",
        "View"
      ],
      "EntityOperations": [
        {
          "EntityName": "HEWeight",
          "Operation": "Create",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "HEWeight",
          "Operation": "Edit",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "HEWeight",
          "Operation": "Delete",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "HEWeight",
          "Operation": "View",
          "DependentOperations": [
            
          ]
        }
      ],
      "ComponentCode": "PMS",
      "IsActive": true,
      "IsImportEnabled": false,
      "ServiceURL": null,
      "IsDefaultEntity": false
    },
    {
      "EntityName": "HourlyMIS",
      "DisplayName": "Hourly MIS",
      "Keys": null,
      "AssemblyName": null,
      "NamespaceName": null,
      "Operations": [
        "View"
      ],
      "EntityOperations": [
        {
          "EntityName": "HourlyMIS",
          "Operation": "View",
          "DependentOperations": [
            
          ]
        }
      ],
      "ComponentCode": "PMS",
      "IsActive": true,
      "IsImportEnabled": false,
      "ServiceURL": null,
      "IsDefaultEntity": false
    },
    {
      "EntityName": "HourlyProduction",
      "DisplayName": "Hourly Production",
      "Keys": null,
      "AssemblyName": null,
      "NamespaceName": null,
      "Operations": [
        "Create",
        "Edit",
        "Delete",
        "View"
      ],
      "EntityOperations": [
        {
          "EntityName": "HourlyProduction",
          "Operation": "Create",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "HourlyProduction",
          "Operation": "Edit",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "HourlyProduction",
          "Operation": "Delete",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "HourlyProduction",
          "Operation": "View",
          "DependentOperations": [
            
          ]
        }
      ],
      "ComponentCode": "PMS",
      "IsActive": true,
      "IsImportEnabled": false,
      "ServiceURL": null,
      "IsDefaultEntity": false
    },
    {
      "EntityName": "ImpactTest",
      "DisplayName": "Impact Test",
      "Keys": null,
      "AssemblyName": null,
      "NamespaceName": null,
      "Operations": [
        "Create",
        "Edit",
        "Delete",
        "View"
      ],
      "EntityOperations": [
        {
          "EntityName": "ImpactTest",
          "Operation": "Create",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "ImpactTest",
          "Operation": "Edit",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "ImpactTest",
          "Operation": "Delete",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "ImpactTest",
          "Operation": "View",
          "DependentOperations": [
            
          ]
        }
      ],
      "ComponentCode": "PMS",
      "IsActive": true,
      "IsImportEnabled": false,
      "ServiceURL": null,
      "IsDefaultEntity": false
    },
    {
      "EntityName": "JobAQL",
      "DisplayName": "Set Job AQL",
      "Keys": null,
      "AssemblyName": null,
      "NamespaceName": null,
      "Operations": [
        "View",
        "Edit",
        "Delete"
      ],
      "EntityOperations": [
        {
          "EntityName": "JobAQL",
          "Operation": "View",
          "DependentOperations": [
            {
              "EntityName": "JobPlan",
              "Operation": "View",
              "DependentOperations": [
                {
                  "EntityName": "Product",
                  "Operation": "View",
                  "DependentOperations": [
                    {
                      "EntityName": "Machine",
                      "Operation": "View",
                      "DependentOperations": [
                        {
                          "EntityName": "OrganisationUnit",
                          "Operation": "View",
                          "DependentOperations": [
                            
                          ]
                        }
                      ]
                    },
                    {
                      "EntityName": "Configuration",
                      "Operation": "View",
                      "DependentOperations": [
                        
                      ]
                    }
                  ]
                }
              ]
            },
            {
              "EntityName": "AQL",
              "Operation": "View",
              "DependentOperations": [
                
              ]
            }
          ]
        },
        {
          "EntityName": "JobAQL",
          "Operation": "Edit",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "JobAQL",
          "Operation": "Delete",
          "DependentOperations": [
            
          ]
        }
      ],
      "ComponentCode": "PMS",
      "IsActive": true,
      "IsImportEnabled": false,
      "ServiceURL": null,
      "IsDefaultEntity": false
    },
    {
      "EntityName": "JobFeedback",
      "DisplayName": "Job Feedback",
      "Keys": null,
      "AssemblyName": null,
      "NamespaceName": null,
      "Operations": [
        "Create",
        "Edit",
        "Delete",
        "View"
      ],
      "EntityOperations": [
        {
          "EntityName": "JobFeedback",
          "Operation": "Create",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "JobFeedback",
          "Operation": "Edit",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "JobFeedback",
          "Operation": "Delete",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "JobFeedback",
          "Operation": "View",
          "DependentOperations": [
            
          ]
        }
      ],
      "ComponentCode": "PMS",
      "IsActive": true,
      "IsImportEnabled": false,
      "ServiceURL": null,
      "IsDefaultEntity": false
    },
    {
      "EntityName": "JobFeedbackRemark",
      "DisplayName": "Job Feedback Remark",
      "Keys": null,
      "AssemblyName": null,
      "NamespaceName": null,
      "Operations": [
        "QC",
        "Production",
        "Furnace"
      ],
      "EntityOperations": [
        {
          "EntityName": "JobFeedbackRemark",
          "Operation": "QC",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "JobFeedbackRemark",
          "Operation": "Production",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "JobFeedbackRemark",
          "Operation": "Furnace",
          "DependentOperations": [
            
          ]
        }
      ],
      "ComponentCode": "PMS",
      "IsActive": true,
      "IsImportEnabled": false,
      "ServiceURL": null,
      "IsDefaultEntity": false
    },
    {
      "EntityName": "JobPlan",
      "DisplayName": "JobPlan",
      "Keys": null,
      "AssemblyName": null,
      "NamespaceName": null,
      "Operations": [
        "Create",
        "View",
        "Delete",
        "Edit",
        "Start",
        "Stop",
        "Pause",
        "Resume"
      ],
      "EntityOperations": [
        {
          "EntityName": "JobPlan",
          "Operation": "Create",
          "DependentOperations": [
            {
              "EntityName": "Product",
              "Operation": "View",
              "DependentOperations": [
                {
                  "EntityName": "Machine",
                  "Operation": "View",
                  "DependentOperations": [
                    {
                      "EntityName": "OrganisationUnit",
                      "Operation": "View",
                      "DependentOperations": [
                        
                      ]
                    }
                  ]
                },
                {
                  "EntityName": "Configuration",
                  "Operation": "View",
                  "DependentOperations": [
                    
                  ]
                }
              ]
            }
          ]
        },
        {
          "EntityName": "JobPlan",
          "Operation": "View",
          "DependentOperations": [
            {
              "EntityName": "Product",
              "Operation": "View",
              "DependentOperations": [
                {
                  "EntityName": "Machine",
                  "Operation": "View",
                  "DependentOperations": [
                    {
                      "EntityName": "OrganisationUnit",
                      "Operation": "View",
                      "DependentOperations": [
                        
                      ]
                    }
                  ]
                },
                {
                  "EntityName": "Configuration",
                  "Operation": "View",
                  "DependentOperations": [
                    
                  ]
                }
              ]
            }
          ]
        },
        {
          "EntityName": "JobPlan",
          "Operation": "Delete",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "JobPlan",
          "Operation": "Edit",
          "DependentOperations": [
            {
              "EntityName": "Product",
              "Operation": "View",
              "DependentOperations": [
                {
                  "EntityName": "Machine",
                  "Operation": "View",
                  "DependentOperations": [
                    {
                      "EntityName": "OrganisationUnit",
                      "Operation": "View",
                      "DependentOperations": [
                        
                      ]
                    }
                  ]
                },
                {
                  "EntityName": "Configuration",
                  "Operation": "View",
                  "DependentOperations": [
                    
                  ]
                }
              ]
            }
          ]
        },
        {
          "EntityName": "JobPlan",
          "Operation": "Start",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "JobPlan",
          "Operation": "Stop",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "JobPlan",
          "Operation": "Pause",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "JobPlan",
          "Operation": "Resume",
          "DependentOperations": [
            
          ]
        }
      ],
      "ComponentCode": "PMS",
      "IsActive": true,
      "IsImportEnabled": false,
      "ServiceURL": null,
      "IsDefaultEntity": false
    },
    {
      "EntityName": "JobQCLabParameter",
      "DisplayName": "QC Lab Parameters",
      "Keys": null,
      "AssemblyName": null,
      "NamespaceName": null,
      "Operations": [
        "Create",
        "Edit",
        "Delete",
        "View"
      ],
      "EntityOperations": [
        {
          "EntityName": "JobQCLabParameter",
          "Operation": "Create",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "JobQCLabParameter",
          "Operation": "Edit",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "JobQCLabParameter",
          "Operation": "Delete",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "JobQCLabParameter",
          "Operation": "View",
          "DependentOperations": [
            
          ]
        }
      ],
      "ComponentCode": "PMS",
      "IsActive": true,
      "IsImportEnabled": false,
      "ServiceURL": null,
      "IsDefaultEntity": false
    },
    {
      "EntityName": "JobRunSummary",
      "DisplayName": "Job Run Summary",
      "Keys": null,
      "AssemblyName": null,
      "NamespaceName": null,
      "Operations": [
        "View"
      ],
      "EntityOperations": [
        {
          "EntityName": "JobRunSummary",
          "Operation": "View",
          "DependentOperations": [
            
          ]
        }
      ],
      "ComponentCode": "PMS",
      "IsActive": true,
      "IsImportEnabled": false,
      "ServiceURL": null,
      "IsDefaultEntity": false
    },
    {
      "EntityName": "LeakageTest",
      "DisplayName": "Leakage Test",
      "Keys": null,
      "AssemblyName": null,
      "NamespaceName": null,
      "Operations": [
        "Create",
        "Edit",
        "Delete",
        "View"
      ],
      "EntityOperations": [
        {
          "EntityName": "LeakageTest",
          "Operation": "Create",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "LeakageTest",
          "Operation": "Edit",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "LeakageTest",
          "Operation": "Delete",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "LeakageTest",
          "Operation": "View",
          "DependentOperations": [
            
          ]
        }
      ],
      "ComponentCode": "PMS",
      "IsActive": true,
      "IsImportEnabled": false,
      "ServiceURL": null,
      "IsDefaultEntity": false
    },
    {
      "EntityName": "Machine",
      "DisplayName": "Machine",
      "Keys": null,
      "AssemblyName": null,
      "NamespaceName": null,
      "Operations": [
        "Create",
        "View",
        "Delete",
        "Edit"
      ],
      "EntityOperations": [
        {
          "EntityName": "Machine",
          "Operation": "Create",
          "DependentOperations": [
            {
              "EntityName": "OrganisationUnit",
              "Operation": "View",
              "DependentOperations": [
                
              ]
            }
          ]
        },
        {
          "EntityName": "Machine",
          "Operation": "View",
          "DependentOperations": [
            {
              "EntityName": "OrganisationUnit",
              "Operation": "View",
              "DependentOperations": [
                
              ]
            }
          ]
        },
        {
          "EntityName": "Machine",
          "Operation": "Delete",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "Machine",
          "Operation": "Edit",
          "DependentOperations": [
            {
              "EntityName": "OrganisationUnit",
              "Operation": "View",
              "DependentOperations": [
                
              ]
            }
          ]
        }
      ],
      "ComponentCode": "PMS",
      "IsActive": true,
      "IsImportEnabled": false,
      "ServiceURL": null,
      "IsDefaultEntity": false
    },
    {
      "EntityName": "Meter",
      "DisplayName": "Meter",
      "Keys": null,
      "AssemblyName": null,
      "NamespaceName": null,
      "Operations": [
        "Create",
        "View",
        "Delete",
        "Edit"
      ],
      "EntityOperations": [
        {
          "EntityName": "Meter",
          "Operation": "Create",
          "DependentOperations": [
            {
              "EntityName": "Parameter",
              "Operation": "View",
              "DependentOperations": [
                
              ]
            }
          ]
        },
        {
          "EntityName": "Meter",
          "Operation": "View",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "Meter",
          "Operation": "Delete",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "Meter",
          "Operation": "Edit",
          "DependentOperations": [
            {
              "EntityName": "Parameter",
              "Operation": "View",
              "DependentOperations": [
                
              ]
            }
          ]
        }
      ],
      "ComponentCode": "PMS",
      "IsActive": true,
      "IsImportEnabled": false,
      "ServiceURL": null,
      "IsDefaultEntity": false
    },
    {
      "EntityName": "Mould",
      "DisplayName": "Mould Setup",
      "Keys": null,
      "AssemblyName": null,
      "NamespaceName": null,
      "Operations": [
        "Create",
        "Edit",
        "Delete",
        "View"
      ],
      "EntityOperations": [
        {
          "EntityName": "Mould",
          "Operation": "Create",
          "DependentOperations": [
            {
              "EntityName": "Defect",
              "Operation": "View",
              "DependentOperations": [
                
              ]
            },
            {
              "EntityName": "JobPlan",
              "Operation": "View",
              "DependentOperations": [
                {
                  "EntityName": "Product",
                  "Operation": "View",
                  "DependentOperations": [
                    {
                      "EntityName": "Machine",
                      "Operation": "View",
                      "DependentOperations": [
                        {
                          "EntityName": "OrganisationUnit",
                          "Operation": "View",
                          "DependentOperations": [
                            
                          ]
                        }
                      ]
                    },
                    {
                      "EntityName": "Configuration",
                      "Operation": "View",
                      "DependentOperations": [
                        
                      ]
                    }
                  ]
                }
              ]
            },
            {
              "EntityName": "User",
              "Operation": "View",
              "DependentOperations": [
                
              ]
            }
          ]
        },
        {
          "EntityName": "Mould",
          "Operation": "Edit",
          "DependentOperations": [
            {
              "EntityName": "Defect",
              "Operation": "View",
              "DependentOperations": [
                
              ]
            },
            {
              "EntityName": "JobPlan",
              "Operation": "View",
              "DependentOperations": [
                {
                  "EntityName": "Product",
                  "Operation": "View",
                  "DependentOperations": [
                    {
                      "EntityName": "Machine",
                      "Operation": "View",
                      "DependentOperations": [
                        {
                          "EntityName": "OrganisationUnit",
                          "Operation": "View",
                          "DependentOperations": [
                            
                          ]
                        }
                      ]
                    },
                    {
                      "EntityName": "Configuration",
                      "Operation": "View",
                      "DependentOperations": [
                        
                      ]
                    }
                  ]
                }
              ]
            },
            {
              "EntityName": "User",
              "Operation": "View",
              "DependentOperations": [
                
              ]
            }
          ]
        },
        {
          "EntityName": "Mould",
          "Operation": "Delete",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "Mould",
          "Operation": "View",
          "DependentOperations": [
            
          ]
        }
      ],
      "ComponentCode": "PMS",
      "IsActive": true,
      "IsImportEnabled": false,
      "ServiceURL": null,
      "IsDefaultEntity": false
    },
    {
      "EntityName": "OperatorAction",
      "DisplayName": "Operator Actions",
      "Keys": null,
      "AssemblyName": null,
      "NamespaceName": null,
      "Operations": [
        "View",
        "Edit",
        "Acknowledge"
      ],
      "EntityOperations": [
        {
          "EntityName": "OperatorAction",
          "Operation": "View",
          "DependentOperations": [
            {
              "EntityName": "JobPlan",
              "Operation": "View",
              "DependentOperations": [
                {
                  "EntityName": "Product",
                  "Operation": "View",
                  "DependentOperations": [
                    {
                      "EntityName": "Machine",
                      "Operation": "View",
                      "DependentOperations": [
                        {
                          "EntityName": "OrganisationUnit",
                          "Operation": "View",
                          "DependentOperations": [
                            
                          ]
                        }
                      ]
                    },
                    {
                      "EntityName": "Configuration",
                      "Operation": "View",
                      "DependentOperations": [
                        
                      ]
                    }
                  ]
                }
              ]
            },
            {
              "EntityName": "HESetOut",
              "Operation": "View",
              "DependentOperations": [
                {
                  "EntityName": "JobPlan",
                  "Operation": "View",
                  "DependentOperations": [
                    {
                      "EntityName": "Product",
                      "Operation": "View",
                      "DependentOperations": [
                        {
                          "EntityName": "Machine",
                          "Operation": "View",
                          "DependentOperations": [
                            {
                              "EntityName": "OrganisationUnit",
                              "Operation": "View",
                              "DependentOperations": [
                                
                              ]
                            }
                          ]
                        },
                        {
                          "EntityName": "Configuration",
                          "Operation": "View",
                          "DependentOperations": [
                            
                          ]
                        }
                      ]
                    }
                  ]
                }
              ]
            }
          ]
        },
        {
          "EntityName": "OperatorAction",
          "Operation": "Edit",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "OperatorAction",
          "Operation": "Acknowledge",
          "DependentOperations": [
            
          ]
        }
      ],
      "ComponentCode": "PMS",
      "IsActive": true,
      "IsImportEnabled": false,
      "ServiceURL": null,
      "IsDefaultEntity": false
    },
    {
      "EntityName": "OrganisationUnit",
      "DisplayName": "OrganisationUnit",
      "Keys": null,
      "AssemblyName": null,
      "NamespaceName": null,
      "Operations": [
        "Create",
        "View",
        "Delete",
        "Edit",
        "RunMultipleJob"
      ],
      "EntityOperations": [
        {
          "EntityName": "OrganisationUnit",
          "Operation": "Create",
          "DependentOperations": [
            {
              "EntityName": "User",
              "Operation": "View",
              "DependentOperations": [
                
              ]
            }
          ]
        },
        {
          "EntityName": "OrganisationUnit",
          "Operation": "View",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "OrganisationUnit",
          "Operation": "Delete",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "OrganisationUnit",
          "Operation": "Edit",
          "DependentOperations": [
            {
              "EntityName": "User",
              "Operation": "View",
              "DependentOperations": [
                
              ]
            }
          ]
        },
        {
          "EntityName": "OrganisationUnit",
          "Operation": "RunMultipleJob",
          "DependentOperations": [
            
          ]
        }
      ],
      "ComponentCode": "PMS",
      "IsActive": true,
      "IsImportEnabled": false,
      "ServiceURL": null,
      "IsDefaultEntity": false
    },
    {
      "EntityName": "Parameter",
      "DisplayName": "Parameter",
      "Keys": null,
      "AssemblyName": null,
      "NamespaceName": null,
      "Operations": [
        "Create",
        "View",
        "Delete",
        "Edit"
      ],
      "EntityOperations": [
        {
          "EntityName": "Parameter",
          "Operation": "Create",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "Parameter",
          "Operation": "View",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "Parameter",
          "Operation": "Delete",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "Parameter",
          "Operation": "Edit",
          "DependentOperations": [
            
          ]
        }
      ],
      "ComponentCode": "PMS",
      "IsActive": true,
      "IsImportEnabled": false,
      "ServiceURL": null,
      "IsDefaultEntity": false
    },
    {
      "EntityName": "PlanityTest",
      "DisplayName": "Planity Test",
      "Keys": null,
      "AssemblyName": null,
      "NamespaceName": null,
      "Operations": [
        "Create",
        "Edit",
        "Delete",
        "View"
      ],
      "EntityOperations": [
        {
          "EntityName": "PlanityTest",
          "Operation": "Create",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "PlanityTest",
          "Operation": "Edit",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "PlanityTest",
          "Operation": "Delete",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "PlanityTest",
          "Operation": "View",
          "DependentOperations": [
            
          ]
        }
      ],
      "ComponentCode": "PMS",
      "IsActive": true,
      "IsImportEnabled": false,
      "ServiceURL": null,
      "IsDefaultEntity": false
    },
    {
      "EntityName": "PlantSummaryReport",
      "DisplayName": "Plant Summary Report",
      "Keys": null,
      "AssemblyName": null,
      "NamespaceName": null,
      "Operations": [
        "View"
      ],
      "EntityOperations": [
        {
          "EntityName": "PlantSummaryReport",
          "Operation": "View",
          "DependentOperations": [
            
          ]
        }
      ],
      "ComponentCode": "PMS",
      "IsActive": true,
      "IsImportEnabled": false,
      "ServiceURL": null,
      "IsDefaultEntity": false
    },
    {
      "EntityName": "Product",
      "DisplayName": "Product",
      "Keys": null,
      "AssemblyName": null,
      "NamespaceName": null,
      "Operations": [
        "Create",
        "View",
        "Delete",
        "Edit"
      ],
      "EntityOperations": [
        {
          "EntityName": "Product",
          "Operation": "Create",
          "DependentOperations": [
            {
              "EntityName": "Machine",
              "Operation": "View",
              "DependentOperations": [
                {
                  "EntityName": "OrganisationUnit",
                  "Operation": "View",
                  "DependentOperations": [
                    
                  ]
                }
              ]
            },
            {
              "EntityName": "Configuration",
              "Operation": "View",
              "DependentOperations": [
                
              ]
            }
          ]
        },
        {
          "EntityName": "Product",
          "Operation": "View",
          "DependentOperations": [
            {
              "EntityName": "Machine",
              "Operation": "View",
              "DependentOperations": [
                {
                  "EntityName": "OrganisationUnit",
                  "Operation": "View",
                  "DependentOperations": [
                    
                  ]
                }
              ]
            },
            {
              "EntityName": "Configuration",
              "Operation": "View",
              "DependentOperations": [
                
              ]
            }
          ]
        },
        {
          "EntityName": "Product",
          "Operation": "Delete",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "Product",
          "Operation": "Edit",
          "DependentOperations": [
            {
              "EntityName": "Machine",
              "Operation": "View",
              "DependentOperations": [
                {
                  "EntityName": "OrganisationUnit",
                  "Operation": "View",
                  "DependentOperations": [
                    
                  ]
                }
              ]
            },
            {
              "EntityName": "Configuration",
              "Operation": "View",
              "DependentOperations": [
                
              ]
            }
          ]
        }
      ],
      "ComponentCode": "PMS",
      "IsActive": true,
      "IsImportEnabled": false,
      "ServiceURL": null,
      "IsDefaultEntity": false
    },
    {
      "EntityName": "ProductQCLabParameter",
      "DisplayName": "QC Lab Parameter Selection",
      "Keys": null,
      "AssemblyName": null,
      "NamespaceName": null,
      "Operations": [
        "Create",
        "Edit",
        "Delete",
        "View"
      ],
      "EntityOperations": [
        {
          "EntityName": "ProductQCLabParameter",
          "Operation": "Create",
          "DependentOperations": [
            {
              "EntityName": "JobPlan",
              "Operation": "View",
              "DependentOperations": [
                {
                  "EntityName": "Product",
                  "Operation": "View",
                  "DependentOperations": [
                    {
                      "EntityName": "Machine",
                      "Operation": "View",
                      "DependentOperations": [
                        {
                          "EntityName": "OrganisationUnit",
                          "Operation": "View",
                          "DependentOperations": [
                            
                          ]
                        }
                      ]
                    },
                    {
                      "EntityName": "Configuration",
                      "Operation": "View",
                      "DependentOperations": [
                        
                      ]
                    }
                  ]
                }
              ]
            },
            {
              "EntityName": "JobQCLabParameter",
              "Operation": "View",
              "DependentOperations": [
                
              ]
            }
          ]
        },
        {
          "EntityName": "ProductQCLabParameter",
          "Operation": "Edit",
          "DependentOperations": [
            {
              "EntityName": "JobPlan",
              "Operation": "View",
              "DependentOperations": [
                {
                  "EntityName": "Product",
                  "Operation": "View",
                  "DependentOperations": [
                    {
                      "EntityName": "Machine",
                      "Operation": "View",
                      "DependentOperations": [
                        {
                          "EntityName": "OrganisationUnit",
                          "Operation": "View",
                          "DependentOperations": [
                            
                          ]
                        }
                      ]
                    },
                    {
                      "EntityName": "Configuration",
                      "Operation": "View",
                      "DependentOperations": [
                        
                      ]
                    }
                  ]
                }
              ]
            },
            {
              "EntityName": "JobQCLabParameter",
              "Operation": "View",
              "DependentOperations": [
                
              ]
            }
          ]
        },
        {
          "EntityName": "ProductQCLabParameter",
          "Operation": "Delete",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "ProductQCLabParameter",
          "Operation": "View",
          "DependentOperations": [
            
          ]
        }
      ],
      "ComponentCode": "PMS",
      "IsActive": true,
      "IsImportEnabled": false,
      "ServiceURL": null,
      "IsDefaultEntity": false
    },
    {
      "EntityName": "QASampling",
      "DisplayName": "QA Sampling",
      "Keys": null,
      "AssemblyName": null,
      "NamespaceName": null,
      "Operations": [
        "View",
        "Edit",
        "Create",
        "Cancel",
        "SyncWithSAP"
      ],
      "EntityOperations": [
        {
          "EntityName": "QASampling",
          "Operation": "View",
          "DependentOperations": [
            {
              "EntityName": "JobPlan",
              "Operation": "View",
              "DependentOperations": [
                {
                  "EntityName": "Product",
                  "Operation": "View",
                  "DependentOperations": [
                    {
                      "EntityName": "Machine",
                      "Operation": "View",
                      "DependentOperations": [
                        {
                          "EntityName": "OrganisationUnit",
                          "Operation": "View",
                          "DependentOperations": [
                            
                          ]
                        }
                      ]
                    },
                    {
                      "EntityName": "Configuration",
                      "Operation": "View",
                      "DependentOperations": [
                        
                      ]
                    }
                  ]
                }
              ]
            },
            {
              "EntityName": "JobPlan",
              "Operation": "View",
              "DependentOperations": [
                {
                  "EntityName": "Product",
                  "Operation": "View",
                  "DependentOperations": [
                    {
                      "EntityName": "Machine",
                      "Operation": "View",
                      "DependentOperations": [
                        {
                          "EntityName": "OrganisationUnit",
                          "Operation": "View",
                          "DependentOperations": [
                            
                          ]
                        }
                      ]
                    },
                    {
                      "EntityName": "Configuration",
                      "Operation": "View",
                      "DependentOperations": [
                        
                      ]
                    }
                  ]
                }
              ]
            },
            {
              "EntityName": "AQL",
              "Operation": "View",
              "DependentOperations": [
                
              ]
            }
          ]
        },
        {
          "EntityName": "QASampling",
          "Operation": "Edit",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "QASampling",
          "Operation": "Create",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "QASampling",
          "Operation": "Cancel",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "QASampling",
          "Operation": "SyncWithSAP",
          "DependentOperations": [
            
          ]
        }
      ],
      "ComponentCode": "PMS",
      "IsActive": true,
      "IsImportEnabled": false,
      "ServiceURL": null,
      "IsDefaultEntity": false
    },
    {
      "EntityName": "QCHold",
      "DisplayName": "QC Hold",
      "Keys": null,
      "AssemblyName": null,
      "NamespaceName": null,
      "Operations": [
        "Create",
        "Edit",
        "Delete",
        "View"
      ],
      "EntityOperations": [
        {
          "EntityName": "QCHold",
          "Operation": "Create",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "QCHold",
          "Operation": "Edit",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "QCHold",
          "Operation": "Delete",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "QCHold",
          "Operation": "View",
          "DependentOperations": [
            
          ]
        }
      ],
      "ComponentCode": "PMS",
      "IsActive": true,
      "IsImportEnabled": false,
      "ServiceURL": null,
      "IsDefaultEntity": false
    },
    {
      "EntityName": "QCLabSampling",
      "DisplayName": "QC Lab Sampling",
      "Keys": null,
      "AssemblyName": null,
      "NamespaceName": null,
      "Operations": [
        "Create",
        "Edit",
        "Delete",
        "View",
        "SyncWithSAP"
      ],
      "EntityOperations": [
        {
          "EntityName": "QCLabSampling",
          "Operation": "Create",
          "DependentOperations": [
            {
              "EntityName": "JobPlan",
              "Operation": "View",
              "DependentOperations": [
                {
                  "EntityName": "Product",
                  "Operation": "View",
                  "DependentOperations": [
                    {
                      "EntityName": "Machine",
                      "Operation": "View",
                      "DependentOperations": [
                        {
                          "EntityName": "OrganisationUnit",
                          "Operation": "View",
                          "DependentOperations": [
                            
                          ]
                        }
                      ]
                    },
                    {
                      "EntityName": "Configuration",
                      "Operation": "View",
                      "DependentOperations": [
                        
                      ]
                    }
                  ]
                }
              ]
            },
            {
              "EntityName": "ProductQCLabParameter",
              "Operation": "View",
              "DependentOperations": [
                
              ]
            },
            {
              "EntityName": "Mould",
              "Operation": "View",
              "DependentOperations": [
                
              ]
            }
          ]
        },
        {
          "EntityName": "QCLabSampling",
          "Operation": "Edit",
          "DependentOperations": [
            {
              "EntityName": "JobPlan",
              "Operation": "View",
              "DependentOperations": [
                {
                  "EntityName": "Product",
                  "Operation": "View",
                  "DependentOperations": [
                    {
                      "EntityName": "Machine",
                      "Operation": "View",
                      "DependentOperations": [
                        {
                          "EntityName": "OrganisationUnit",
                          "Operation": "View",
                          "DependentOperations": [
                            
                          ]
                        }
                      ]
                    },
                    {
                      "EntityName": "Configuration",
                      "Operation": "View",
                      "DependentOperations": [
                        
                      ]
                    }
                  ]
                }
              ]
            },
            {
              "EntityName": "ProductQCLabParameter",
              "Operation": "View",
              "DependentOperations": [
                
              ]
            },
            {
              "EntityName": "Mould",
              "Operation": "View",
              "DependentOperations": [
                
              ]
            }
          ]
        },
        {
          "EntityName": "QCLabSampling",
          "Operation": "Delete",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "QCLabSampling",
          "Operation": "View",
          "DependentOperations": [
            {
              "EntityName": "JobPlan",
              "Operation": "View",
              "DependentOperations": [
                {
                  "EntityName": "Product",
                  "Operation": "View",
                  "DependentOperations": [
                    {
                      "EntityName": "Machine",
                      "Operation": "View",
                      "DependentOperations": [
                        {
                          "EntityName": "OrganisationUnit",
                          "Operation": "View",
                          "DependentOperations": [
                            
                          ]
                        }
                      ]
                    },
                    {
                      "EntityName": "Configuration",
                      "Operation": "View",
                      "DependentOperations": [
                        
                      ]
                    }
                  ]
                }
              ]
            }
          ]
        },
        {
          "EntityName": "QCLabSampling",
          "Operation": "SyncWithSAP",
          "DependentOperations": [
            
          ]
        }
      ],
      "ComponentCode": "PMS",
      "IsActive": true,
      "IsImportEnabled": false,
      "ServiceURL": null,
      "IsDefaultEntity": false
    },
    {
      "EntityName": "RampPressureTest",
      "DisplayName": "Ramp Pressure Test",
      "Keys": null,
      "AssemblyName": null,
      "NamespaceName": null,
      "Operations": [
        "Create",
        "Edit",
        "View",
        "Delete"
      ],
      "EntityOperations": [
        {
          "EntityName": "RampPressureTest",
          "Operation": "Create",
          "DependentOperations": [
            {
              "EntityName": "Mould",
              "Operation": "View",
              "DependentOperations": [
                
              ]
            },
            {
              "EntityName": "Shift",
              "Operation": "View",
              "DependentOperations": [
                {
                  "EntityName": "OrganisationUnit",
                  "Operation": "View",
                  "DependentOperations": [
                    
                  ]
                }
              ]
            }
          ]
        },
        {
          "EntityName": "RampPressureTest",
          "Operation": "Edit",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "RampPressureTest",
          "Operation": "View",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "RampPressureTest",
          "Operation": "Delete",
          "DependentOperations": [
            
          ]
        }
      ],
      "ComponentCode": "PMS",
      "IsActive": true,
      "IsImportEnabled": false,
      "ServiceURL": null,
      "IsDefaultEntity": false
    },
    {
      "EntityName": "Resorting",
      "DisplayName": "Resorting",
      "Keys": null,
      "AssemblyName": null,
      "NamespaceName": null,
      "Operations": [
        "View"
      ],
      "EntityOperations": [
        {
          "EntityName": "Resorting",
          "Operation": "View",
          "DependentOperations": [
            
          ]
        }
      ],
      "ComponentCode": "PMS",
      "IsActive": true,
      "IsImportEnabled": false,
      "ServiceURL": null,
      "IsDefaultEntity": false
    },
    {
      "EntityName": "ResortingStatusReport",
      "DisplayName": "Resorting Status Report",
      "Keys": null,
      "AssemblyName": null,
      "NamespaceName": null,
      "Operations": [
        "View"
      ],
      "EntityOperations": [
        {
          "EntityName": "ResortingStatusReport",
          "Operation": "View",
          "DependentOperations": [
            
          ]
        }
      ],
      "ComponentCode": "PMS",
      "IsActive": true,
      "IsImportEnabled": false,
      "ServiceURL": null,
      "IsDefaultEntity": false
    },
    {
      "EntityName": "Role",
      "DisplayName": "Role",
      "Keys": null,
      "AssemblyName": null,
      "NamespaceName": null,
      "Operations": [
        "Create",
        "View",
        "Edit",
        "Delete"
      ],
      "EntityOperations": [
        {
          "EntityName": "Role",
          "Operation": "Create",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "Role",
          "Operation": "View",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "Role",
          "Operation": "Edit",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "Role",
          "Operation": "Delete",
          "DependentOperations": [
            
          ]
        }
      ],
      "ComponentCode": "PMS",
      "IsActive": true,
      "IsImportEnabled": false,
      "ServiceURL": null,
      "IsDefaultEntity": false
    },
    {
      "EntityName": "RTMIAppConfiguration",
      "DisplayName": "RTMI App Configuration",
      "Keys": null,
      "AssemblyName": null,
      "NamespaceName": null,
      "Operations": [
        "Edit",
        "View"
      ],
      "EntityOperations": [
        {
          "EntityName": "RTMIAppConfiguration",
          "Operation": "Edit",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "RTMIAppConfiguration",
          "Operation": "View",
          "DependentOperations": [
            
          ]
        }
      ],
      "ComponentCode": "PMS",
      "IsActive": true,
      "IsImportEnabled": false,
      "ServiceURL": null,
      "IsDefaultEntity": false
    },
    {
      "EntityName": "RTMISAPIntegration",
      "DisplayName": "RTMI SAP Integration",
      "Keys": null,
      "AssemblyName": null,
      "NamespaceName": null,
      "Operations": [
        "View"
      ],
      "EntityOperations": [
        {
          "EntityName": "RTMISAPIntegration",
          "Operation": "View",
          "DependentOperations": [
            
          ]
        }
      ],
      "ComponentCode": "PMS",
      "IsActive": true,
      "IsImportEnabled": false,
      "ServiceURL": null,
      "IsDefaultEntity": false
    },
    {
      "EntityName": "SetOutComplianceReport",
      "DisplayName": "Set Out Compliance Report",
      "Keys": null,
      "AssemblyName": null,
      "NamespaceName": null,
      "Operations": [
        "View",
        "Edit"
      ],
      "EntityOperations": [
        {
          "EntityName": "SetOutComplianceReport",
          "Operation": "View",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "SetOutComplianceReport",
          "Operation": "Edit",
          "DependentOperations": [
            
          ]
        }
      ],
      "ComponentCode": "PMS",
      "IsActive": true,
      "IsImportEnabled": false,
      "ServiceURL": null,
      "IsDefaultEntity": false
    },
    {
      "EntityName": "ShakerTest",
      "DisplayName": "Shaker Test",
      "Keys": null,
      "AssemblyName": null,
      "NamespaceName": null,
      "Operations": [
        "Create",
        "Edit",
        "Delete",
        "View"
      ],
      "EntityOperations": [
        {
          "EntityName": "ShakerTest",
          "Operation": "Create",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "ShakerTest",
          "Operation": "Edit",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "ShakerTest",
          "Operation": "Delete",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "ShakerTest",
          "Operation": "View",
          "DependentOperations": [
            
          ]
        }
      ],
      "ComponentCode": "PMS",
      "IsActive": true,
      "IsImportEnabled": false,
      "ServiceURL": null,
      "IsDefaultEntity": false
    },
    {
      "EntityName": "Shift",
      "DisplayName": "Shift",
      "Keys": null,
      "AssemblyName": null,
      "NamespaceName": null,
      "Operations": [
        "Create",
        "View",
        "Delete",
        "Edit"
      ],
      "EntityOperations": [
        {
          "EntityName": "Shift",
          "Operation": "Create",
          "DependentOperations": [
            {
              "EntityName": "OrganisationUnit",
              "Operation": "View",
              "DependentOperations": [
                
              ]
            }
          ]
        },
        {
          "EntityName": "Shift",
          "Operation": "View",
          "DependentOperations": [
            {
              "EntityName": "OrganisationUnit",
              "Operation": "View",
              "DependentOperations": [
                
              ]
            }
          ]
        },
        {
          "EntityName": "Shift",
          "Operation": "Delete",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "Shift",
          "Operation": "Edit",
          "DependentOperations": [
            {
              "EntityName": "OrganisationUnit",
              "Operation": "View",
              "DependentOperations": [
                
              ]
            }
          ]
        }
      ],
      "ComponentCode": "PMS",
      "IsActive": true,
      "IsImportEnabled": false,
      "ServiceURL": null,
      "IsDefaultEntity": false
    },
    {
      "EntityName": "Station",
      "DisplayName": "Station",
      "Keys": null,
      "AssemblyName": null,
      "NamespaceName": null,
      "Operations": [
        "Create",
        "View",
        "Delete",
        "Edit"
      ],
      "EntityOperations": [
        {
          "EntityName": "Station",
          "Operation": "Create",
          "DependentOperations": [
            {
              "EntityName": "Meter",
              "Operation": "View",
              "DependentOperations": [
                
              ]
            }
          ]
        },
        {
          "EntityName": "Station",
          "Operation": "View",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "Station",
          "Operation": "Delete",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "Station",
          "Operation": "Edit",
          "DependentOperations": [
            
          ]
        }
      ],
      "ComponentCode": "PMS",
      "IsActive": true,
      "IsImportEnabled": false,
      "ServiceURL": null,
      "IsDefaultEntity": false
    },
    {
      "EntityName": "TestConfiguration",
      "DisplayName": "Test Configuration",
      "Keys": null,
      "AssemblyName": null,
      "NamespaceName": null,
      "Operations": [
        "Create",
        "Edit",
        "Delete",
        "View"
      ],
      "EntityOperations": [
        {
          "EntityName": "TestConfiguration",
          "Operation": "Create",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "TestConfiguration",
          "Operation": "Edit",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "TestConfiguration",
          "Operation": "Delete",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "TestConfiguration",
          "Operation": "View",
          "DependentOperations": [
            
          ]
        }
      ],
      "ComponentCode": "PMS",
      "IsActive": true,
      "IsImportEnabled": false,
      "ServiceURL": null,
      "IsDefaultEntity": false
    },
    {
      "EntityName": "ThermalShock",
      "DisplayName": "Thermal Shock Test Record",
      "Keys": null,
      "AssemblyName": null,
      "NamespaceName": null,
      "Operations": [
        "Create",
        "Edit",
        "Delete",
        "View"
      ],
      "EntityOperations": [
        {
          "EntityName": "ThermalShock",
          "Operation": "Create",
          "DependentOperations": [
            {
              "EntityName": "JobPlan",
              "Operation": "View",
              "DependentOperations": [
                {
                  "EntityName": "Product",
                  "Operation": "View",
                  "DependentOperations": [
                    {
                      "EntityName": "Machine",
                      "Operation": "View",
                      "DependentOperations": [
                        {
                          "EntityName": "OrganisationUnit",
                          "Operation": "View",
                          "DependentOperations": [
                            
                          ]
                        }
                      ]
                    },
                    {
                      "EntityName": "Configuration",
                      "Operation": "View",
                      "DependentOperations": [
                        
                      ]
                    }
                  ]
                }
              ]
            },
            {
              "EntityName": "Mould",
              "Operation": "View",
              "DependentOperations": [
                
              ]
            }
          ]
        },
        {
          "EntityName": "ThermalShock",
          "Operation": "Edit",
          "DependentOperations": [
            {
              "EntityName": "JobPlan",
              "Operation": "View",
              "DependentOperations": [
                {
                  "EntityName": "Product",
                  "Operation": "View",
                  "DependentOperations": [
                    {
                      "EntityName": "Machine",
                      "Operation": "View",
                      "DependentOperations": [
                        {
                          "EntityName": "OrganisationUnit",
                          "Operation": "View",
                          "DependentOperations": [
                            
                          ]
                        }
                      ]
                    },
                    {
                      "EntityName": "Configuration",
                      "Operation": "View",
                      "DependentOperations": [
                        
                      ]
                    }
                  ]
                }
              ]
            },
            {
              "EntityName": "Mould",
              "Operation": "View",
              "DependentOperations": [
                
              ]
            }
          ]
        },
        {
          "EntityName": "ThermalShock",
          "Operation": "Delete",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "ThermalShock",
          "Operation": "View",
          "DependentOperations": [
            {
              "EntityName": "JobPlan",
              "Operation": "View",
              "DependentOperations": [
                {
                  "EntityName": "Product",
                  "Operation": "View",
                  "DependentOperations": [
                    {
                      "EntityName": "Machine",
                      "Operation": "View",
                      "DependentOperations": [
                        {
                          "EntityName": "OrganisationUnit",
                          "Operation": "View",
                          "DependentOperations": [
                            
                          ]
                        }
                      ]
                    },
                    {
                      "EntityName": "Configuration",
                      "Operation": "View",
                      "DependentOperations": [
                        
                      ]
                    }
                  ]
                }
              ]
            }
          ]
        }
      ],
      "ComponentCode": "PMS",
      "IsActive": true,
      "IsImportEnabled": false,
      "ServiceURL": null,
      "IsDefaultEntity": false
    },
    {
      "EntityName": "TransferredToFG",
      "DisplayName": "Transferred To FG",
      "Keys": null,
      "AssemblyName": null,
      "NamespaceName": null,
      "Operations": [
        "Create",
        "Edit",
        "View",
        "Delete"
      ],
      "EntityOperations": [
        {
          "EntityName": "TransferredToFG",
          "Operation": "Create",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "TransferredToFG",
          "Operation": "Edit",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "TransferredToFG",
          "Operation": "View",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "TransferredToFG",
          "Operation": "Delete",
          "DependentOperations": [
            
          ]
        }
      ],
      "ComponentCode": "PMS",
      "IsActive": true,
      "IsImportEnabled": false,
      "ServiceURL": null,
      "IsDefaultEntity": false
    },
    {
      "EntityName": "UnitOfMeasurement",
      "DisplayName": "Unit Of Measurement",
      "Keys": null,
      "AssemblyName": null,
      "NamespaceName": null,
      "Operations": [
        "View"
      ],
      "EntityOperations": [
        {
          "EntityName": "UnitOfMeasurement",
          "Operation": "View",
          "DependentOperations": [
            
          ]
        }
      ],
      "ComponentCode": "PMS",
      "IsActive": true,
      "IsImportEnabled": false,
      "ServiceURL": null,
      "IsDefaultEntity": false
    },
    {
      "EntityName": "User",
      "DisplayName": "User",
      "Keys": null,
      "AssemblyName": null,
      "NamespaceName": null,
      "Operations": [
        "Create",
        "Edit",
        "Delete",
        "View",
        "Lock/Unlock",
        "ChangePassword"
      ],
      "EntityOperations": [
        {
          "EntityName": "User",
          "Operation": "Create",
          "DependentOperations": [
            {
              "EntityName": "Role",
              "Operation": "View",
              "DependentOperations": [
                
              ]
            }
          ]
        },
        {
          "EntityName": "User",
          "Operation": "Edit",
          "DependentOperations": [
            {
              "EntityName": "Role",
              "Operation": "View",
              "DependentOperations": [
                
              ]
            }
          ]
        },
        {
          "EntityName": "User",
          "Operation": "Delete",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "User",
          "Operation": "View",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "User",
          "Operation": "Lock/Unlock",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "User",
          "Operation": "ChangePassword",
          "DependentOperations": [
            
          ]
        }
      ],
      "ComponentCode": "PMS",
      "IsActive": true,
      "IsImportEnabled": false,
      "ServiceURL": null,
      "IsDefaultEntity": false
    },
    {
      "EntityName": "VerticalityTest",
      "DisplayName": "Verticality Test",
      "Keys": null,
      "AssemblyName": null,
      "NamespaceName": null,
      "Operations": [
        "Create",
        "Edit",
        "Delete",
        "View"
      ],
      "EntityOperations": [
        {
          "EntityName": "VerticalityTest",
          "Operation": "Create",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "VerticalityTest",
          "Operation": "Edit",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "VerticalityTest",
          "Operation": "Delete",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "VerticalityTest",
          "Operation": "View",
          "DependentOperations": [
            
          ]
        }
      ],
      "ComponentCode": "PMS",
      "IsActive": true,
      "IsImportEnabled": false,
      "ServiceURL": null,
      "IsDefaultEntity": false
    },
    {
      "EntityName": "VerticalLoadTest",
      "DisplayName": "Vertical Load Test",
      "Keys": null,
      "AssemblyName": null,
      "NamespaceName": null,
      "Operations": [
        "Create",
        "Edit",
        "Delete",
        "View"
      ],
      "EntityOperations": [
        {
          "EntityName": "VerticalLoadTest",
          "Operation": "Create",
          "DependentOperations": [
            {
              "EntityName": "JobPlan",
              "Operation": "View",
              "DependentOperations": [
                {
                  "EntityName": "Product",
                  "Operation": "View",
                  "DependentOperations": [
                    {
                      "EntityName": "Machine",
                      "Operation": "View",
                      "DependentOperations": [
                        {
                          "EntityName": "OrganisationUnit",
                          "Operation": "View",
                          "DependentOperations": [
                            
                          ]
                        }
                      ]
                    },
                    {
                      "EntityName": "Configuration",
                      "Operation": "View",
                      "DependentOperations": [
                        
                      ]
                    }
                  ]
                }
              ]
            },
            {
              "EntityName": "JobPlan",
              "Operation": "View",
              "DependentOperations": [
                {
                  "EntityName": "Product",
                  "Operation": "View",
                  "DependentOperations": [
                    {
                      "EntityName": "Machine",
                      "Operation": "View",
                      "DependentOperations": [
                        {
                          "EntityName": "OrganisationUnit",
                          "Operation": "View",
                          "DependentOperations": [
                            
                          ]
                        }
                      ]
                    },
                    {
                      "EntityName": "Configuration",
                      "Operation": "View",
                      "DependentOperations": [
                        
                      ]
                    }
                  ]
                }
              ]
            },
            {
              "EntityName": "Mould",
              "Operation": "View",
              "DependentOperations": [
                
              ]
            },
            {
              "EntityName": "Shift",
              "Operation": "View",
              "DependentOperations": [
                {
                  "EntityName": "OrganisationUnit",
                  "Operation": "View",
                  "DependentOperations": [
                    
                  ]
                }
              ]
            }
          ]
        },
        {
          "EntityName": "VerticalLoadTest",
          "Operation": "Edit",
          "DependentOperations": [
            {
              "EntityName": "JobPlan",
              "Operation": "View",
              "DependentOperations": [
                {
                  "EntityName": "Product",
                  "Operation": "View",
                  "DependentOperations": [
                    {
                      "EntityName": "Machine",
                      "Operation": "View",
                      "DependentOperations": [
                        {
                          "EntityName": "OrganisationUnit",
                          "Operation": "View",
                          "DependentOperations": [
                            
                          ]
                        }
                      ]
                    },
                    {
                      "EntityName": "Configuration",
                      "Operation": "View",
                      "DependentOperations": [
                        
                      ]
                    }
                  ]
                }
              ]
            },
            {
              "EntityName": "Mould",
              "Operation": "View",
              "DependentOperations": [
                
              ]
            },
            {
              "EntityName": "Shift",
              "Operation": "View",
              "DependentOperations": [
                {
                  "EntityName": "OrganisationUnit",
                  "Operation": "View",
                  "DependentOperations": [
                    
                  ]
                }
              ]
            }
          ]
        },
        {
          "EntityName": "VerticalLoadTest",
          "Operation": "Delete",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "VerticalLoadTest",
          "Operation": "View",
          "DependentOperations": [
            {
              "EntityName": "JobPlan",
              "Operation": "View",
              "DependentOperations": [
                {
                  "EntityName": "Product",
                  "Operation": "View",
                  "DependentOperations": [
                    {
                      "EntityName": "Machine",
                      "Operation": "View",
                      "DependentOperations": [
                        {
                          "EntityName": "OrganisationUnit",
                          "Operation": "View",
                          "DependentOperations": [
                            
                          ]
                        }
                      ]
                    },
                    {
                      "EntityName": "Configuration",
                      "Operation": "View",
                      "DependentOperations": [
                        
                      ]
                    }
                  ]
                }
              ]
            }
          ]
        }
      ],
      "ComponentCode": "PMS",
      "IsActive": true,
      "IsImportEnabled": false,
      "ServiceURL": null,
      "IsDefaultEntity": false
    },
    {
      "EntityName": "Weight",
      "DisplayName": "Weight Record",
      "Keys": null,
      "AssemblyName": null,
      "NamespaceName": null,
      "Operations": [
        "Create",
        "Edit",
        "Delete",
        "View"
      ],
      "EntityOperations": [
        {
          "EntityName": "Weight",
          "Operation": "Create",
          "DependentOperations": [
            {
              "EntityName": "JobPlan",
              "Operation": "View",
              "DependentOperations": [
                {
                  "EntityName": "Product",
                  "Operation": "View",
                  "DependentOperations": [
                    {
                      "EntityName": "Machine",
                      "Operation": "View",
                      "DependentOperations": [
                        {
                          "EntityName": "OrganisationUnit",
                          "Operation": "View",
                          "DependentOperations": [
                            
                          ]
                        }
                      ]
                    },
                    {
                      "EntityName": "Configuration",
                      "Operation": "View",
                      "DependentOperations": [
                        
                      ]
                    }
                  ]
                }
              ]
            }
          ]
        },
        {
          "EntityName": "Weight",
          "Operation": "Edit",
          "DependentOperations": [
            {
              "EntityName": "JobPlan",
              "Operation": "View",
              "DependentOperations": [
                {
                  "EntityName": "Product",
                  "Operation": "View",
                  "DependentOperations": [
                    {
                      "EntityName": "Machine",
                      "Operation": "View",
                      "DependentOperations": [
                        {
                          "EntityName": "OrganisationUnit",
                          "Operation": "View",
                          "DependentOperations": [
                            
                          ]
                        }
                      ]
                    },
                    {
                      "EntityName": "Configuration",
                      "Operation": "View",
                      "DependentOperations": [
                        
                      ]
                    }
                  ]
                }
              ]
            }
          ]
        },
        {
          "EntityName": "Weight",
          "Operation": "Delete",
          "DependentOperations": [
            
          ]
        },
        {
          "EntityName": "Weight",
          "Operation": "View",
          "DependentOperations": [
            {
              "EntityName": "JobPlan",
              "Operation": "View",
              "DependentOperations": [
                {
                  "EntityName": "Product",
                  "Operation": "View",
                  "DependentOperations": [
                    {
                      "EntityName": "Machine",
                      "Operation": "View",
                      "DependentOperations": [
                        {
                          "EntityName": "OrganisationUnit",
                          "Operation": "View",
                          "DependentOperations": [
                            
                          ]
                        }
                      ]
                    },
                    {
                      "EntityName": "Configuration",
                      "Operation": "View",
                      "DependentOperations": [
                        
                      ]
                    }
                  ]
                }
              ]
            }
          ]
        }
      ],
      "ComponentCode": "PMS",
      "IsActive": true,
      "IsImportEnabled": false,
      "ServiceURL": null,
      "IsDefaultEntity": false
    }
  ];