import { TemplateWidget } from './templateWidget';

export interface Template {
    "Identifier":number;
    "DisplayName":string;
    "Status":string;
    "PageTypeId":number;
    "PublishedBy":number;
    "PageOwner":number;
    "Version":string;
    "PublishedOn":Date;
    "IsActive":boolean;
    "IsDeleted":boolean;
    "PageOwnerName":string;
    "PagePublishedByUserName":string;
    "PageTypeName":string;
    "PageWidgets": TemplateWidget[];
}